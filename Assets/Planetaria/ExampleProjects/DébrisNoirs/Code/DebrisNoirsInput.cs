using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    /// <summary>
    /// Encapsulate movement (layer of abstraction)
    /// </summary>
    public static class DebrisNoirsInput
    {
        public static bool firing() // Assumes firing() is called every frame
        {
            time_until_bullet += Time.deltaTime;
            if (time_until_bullet >= bullet_interval)
            {
                time_until_bullet -= bullet_interval;
                return true;
            }
            return false; // CONSIDER: manual implementation if button/trigger pressed
        }
        
        /// <summary>
        /// Inspector - Gets primative controller and keyboard axes (horizontal and vertical).
        /// </summary>
        /// <returns>The horizontal and verical component of any controller input, normalized.</returns>
        public static Vector2 get_primative_axes()
        {
            float horizontal = Input.GetAxis("PlanetariaUniversalInputHorizontal");
            float vertical = Input.GetAxis("PlanetariaUniversalInputVertical");
            Vector2 input_axes = new Vector2(horizontal, vertical);
            if (input_axes.SqrMagnitude() > 1)
            {
                input_axes.Normalize();
            }
            return input_axes;
        }

        public static float get_primative_rotate()
        {
            float horizontal = Input.GetAxisRaw("PlanetariaUniversalInputHorizontal");
            if (Mathf.Abs(horizontal) < 0.3f)
            {
                return 0; // do not rotate
            }
            return horizontal > 0 ? +1 : -1;
        }

        public static float get_primative_accelerate()
        {
            float vertical = Input.GetAxisRaw("PlanetariaUniversalInputVertical");
            float acceleration = Mathf.Abs(vertical); // pressing up/down both accelerate ship (in part, so that inverted axis doesn't matter)
            return acceleration > 0.3f ? 1 : 0; // acceleration is all or nothing
        }

        /// <summary>
        /// Inspector - Get the input direction based on the virtual reality headset's view direction (or mouse position in Editor mode).
        /// </summary>
        /// <returns>The position/direction the player is looking relative to the satellite.</returns>
        public static Vector2 get_compound_axes() // FIXME: the respawn zone in ghost mode shifts in direction of velocity
        {
            if (!main_character || !main_controller)
            {
                main_character = GameObject.FindObjectOfType<Satellite>().gameObject.transform;
                main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            }
            
            // Supposed to be called if no controller exists (or hasn't been used for 20 seconds).
            // In which case: use the head's orientation (or mouse) as a controller.
            
            // The direction from the satellite to the controller (which represents either the head's view direction or the mouse position).
            Vector3 direction = Bearing.attractor(main_character.forward, main_controller.forward);
            float target_angle = Vector3.SignedAngle(main_character.up, direction, main_character.forward) * Mathf.Deg2Rad;
            // Distance from satellite to controller (which represents either the head's view direction or the mouse position).
            float target_distance = Vector3.Angle(main_character.forward, main_controller.forward) * Mathf.Deg2Rad;
            target_distance = Mathf.Clamp01((target_distance - 0.03f /*satellite radius, not divided by 2 because Actuator goes twice as far as head movement*/)/0.4f);
            // Return the composite input direction.
            Vector2 input_direction = new Vector2(-Mathf.Sin(target_angle), Mathf.Cos(target_angle)) * target_distance;
            return input_direction;
        }

        public static float get_compound_rotate()
        {
            if (!main_character || !main_controller)
            {
                main_character = GameObject.FindObjectOfType<Satellite>().gameObject.transform;
                main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            }
            Quaternion character_to_controller_rotation = Quaternion.Inverse(main_character.rotation) * main_controller.rotation;
            if (character_to_controller_rotation.eulerAngles.y <= 5*2) // head rotation less than 5 degrees
            {
                return 0; // make rotation continuous (with no explicit zero)
            }
            else if (character_to_controller_rotation.eulerAngles.y >= 360 - 5*2)
            {
                return 0;
            }
            return character_to_controller_rotation.eulerAngles.y > 180 ? +1 : -1; 
        }

        // I had some thoughts on z, x, y unity rotations for Euler angles.
        // I think this means that if the player's head is tilted, the directions are relative to that (e.g. if you were laying on your side).
        // For large (or complex) rotations, you might want to remove the x rotation from the acceleration calculations using y Euler angles, but I really don't think it actually matters
        // I don't think you can play this laying down (unless the Quaternion.Inverse(main_character.rotation) * main_controller.rotation magic works), but I'll try to fix that eventually.
        public static float get_compound_accelerate()
        {
            if (!main_character || !main_controller)
            {
                main_character = GameObject.FindObjectOfType<Satellite>().gameObject.transform;
                main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            }
            Quaternion character_to_controller_rotation = Quaternion.Inverse(main_character.rotation) * main_controller.rotation;
            if (character_to_controller_rotation.eulerAngles.x <= 5*2) // head rotation less than 5 degrees
            {
                return 0;
            }
            else if (character_to_controller_rotation.eulerAngles.x >= 360 - 5*2)
            {
                return 0;
            }
            return +1; // acceleration is all or nothing, and inverted axes don't matter (-1 / reverse isn't possible)
        }

        public static float get_rotate()
        {
            if (time_since_primative_input <= seconds_until_head_control) // using_primative_axis() without accidental double counting (technically one runs in Update(), the other FixedUpdate())
            {
                return get_primative_rotate();
            }
            return get_compound_rotate();
        }

        public static float get_accelerate()
        {
            if (time_since_primative_input <= seconds_until_head_control) // using_primative_axis() without accidental double counting (technically one runs in Update(), the other FixedUpdate())
            {
                return get_primative_accelerate();
            }
            return get_compound_accelerate();
        }

        public static Vector2 get_axes() // NOTE: This shouldn't be a bottleneck, so the redundancy/inefficiency is fine.
        {
            // traditional movement e.g. with a controller or keyboard
            if (using_primative_axis())
            {
                return get_primative_axes();
            }
            // movement with either neck control (virtual reality) or mouse
            return get_compound_axes();
        }

        /// <summary>
        /// Inspector - Is the player using a primative movement device (e.g. controller, keyboard).
        /// </summary>
        /// <returns>Is a primative controller being used?</returns>
        public static bool using_primative_axis()
        {
            time_since_primative_input += Time.deltaTime;
            if (get_primative_axes() != Vector2.zero) // if any overloaded axis is being used at all (ignoring a deadzone of ~0.001f)
            {
                time_since_primative_input = 0;
            }
            return time_since_primative_input <= seconds_until_head_control;
        }

        private static Transform main_character;
        private static Transform main_controller;
        private static float time_since_primative_input = 100;
        private static float time_until_bullet = 0;

        private const float bullet_interval = 0.5f/7; // kill debris and its spawn (1+2+4 total) in half second; almost 4x original fire rate (original: 4 bullets at 1.2s, but you could fire every frame (~0.0166666667f) in theory)
        private const float seconds_until_head_control = 20f;
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.