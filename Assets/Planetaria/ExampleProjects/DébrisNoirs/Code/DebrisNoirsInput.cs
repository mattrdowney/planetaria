using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    /// <summary>
    /// Encapsulate movement (layer of abstraction)
    /// </summary>
    public static class DebrisNoirsInput
    {
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

        /// <summary>
        /// Inspector - Get the input direction based on the virtual reality headset's view direction (or mouse position in Editor mode).
        /// </summary>
        /// <returns>The position/direction the player is looking relative to the satellite.</returns>
        public static Vector2 get_compound_axes() // FIXME: the respawn zone in ghost mode shifts in direction of velocity
        {
            // Supposed to be called if no controller exists (or hasn't been used for 20 seconds).
            // In which case: use the head's orientation or mouse as a controller.
            if (!main_character || !main_controller)
            {
                main_character = GameObject.FindObjectOfType<Satellite>().gameObject.internal_game_object.transform;
                main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            }
            
            // The direction from the satellite to the controller (which represents either the head's view direction or the mouse position).
            Vector3 direction = Bearing.attractor(main_character.forward, main_controller.forward);
            float target_angle = Vector3.SignedAngle(main_character.up, direction, main_character.forward) * Mathf.Deg2Rad;
            // Distance from satellite to controller (which represents either the head's view direction or the mouse position).
            float target_distance = Vector3.Angle(main_character.forward, main_controller.forward) * Mathf.Deg2Rad;
            target_distance = Mathf.Clamp01(target_distance/Mathf.PI); // Normalize a distance [0,PI] to range [0,1].
            // Return the composite input direction.
            Vector2 input_direction = new Vector2(-Mathf.Sin(target_angle), Mathf.Cos(target_angle)) * target_distance;
            return input_direction;
        }

        public static Vector2 get_axes() // NOTE: This shouldn't be a bottleneck, so the redundancy/inefficiency is fine.
        {
            Vector2 result;
            // traditional movement e.g. with a controller or keyboard
            if (using_primative_axis())
            {
                result = get_primative_axes();
                if (result.sqrMagnitude < 0.01f)
                {
                    last_non_input_frame = Time.frameCount;
                    return Vector2.zero;
                }
                // I would generally prefer to interpolate back and forth (not one-directional) but whatever.
                return result.normalized * Mathf.Clamp01((Time.frameCount-last_non_input_frame+1) * Time.fixedDeltaTime / seconds_until_full_acceleration);
            }
            // movement with either neck control (virtual reality) or mouse
            result = get_compound_axes();
            if (result.magnitude < 0.03926991f/2) // hardcoded ship radius
            {
                last_non_input_frame = Time.frameCount;
                return Vector2.zero;
            }
            return result.normalized * Mathf.Clamp01((Time.frameCount-last_non_input_frame+1) * Time.fixedDeltaTime / seconds_until_full_acceleration);
        }

        public static Vector2 get_direction()
        {
            Vector2 result;
            // traditional movement e.g. with a controller or keyboard
            if (using_primative_axis())
            {
                result = get_primative_axes();
                return result.normalized;
            }
            // movement with either neck control (virtual reality) or mouse
            result = get_compound_axes();
            return result.normalized;
        }

        /// <summary>
        /// Inspector - Is the player using a primative movement device (e.g. controller, keyboard).
        /// </summary>
        /// <returns>Is a primative controller being used?</returns>
        public static bool using_primative_axis()
        {
            if (get_primative_axes() != Vector2.zero) // if any overloaded axis is being used at all (ignoring a deadzone of ~0.001f)
            {
                last_primative_input_frame = Time.frameCount;
            }
            int allowed_frames_without_input = Mathf.CeilToInt(seconds_until_head_control / Time.fixedDeltaTime);
            return last_primative_input_frame + allowed_frames_without_input >= Time.frameCount;
        }

        private static Transform main_character;
        private static Transform main_controller;

        private const float seconds_until_head_control = 20f;
        private const float seconds_until_full_acceleration = 0.5f;
        private static int last_primative_input_frame = int.MinValue;
        private static int last_non_input_frame = 0;
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