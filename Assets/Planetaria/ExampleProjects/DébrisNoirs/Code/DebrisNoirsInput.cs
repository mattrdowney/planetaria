using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// Encapsulate movement (layer of abstraction)
    /// </summary>
	public static class DebrisNoirsInput
	{
        public static Vector2 movement()
        {
            // traditional movement with a controller
            float horizontal = Input.GetAxis("PlanetariaUniversalInputHorizontal");
            float vertical = Input.GetAxis("PlanetariaUniversalInputVertical");
            if (horizontal != 0 || vertical != 0)
            {
                last_input_frame = Time.frameCount;
            }
            if (last_input_frame + (int)(seconds_until_head_control/Time.fixedDeltaTime) > Time.frameCount)
            {
                return new Vector2(horizontal, vertical).normalized;
            }

            // if no controller exists (or hasn't been used for 20 seconds), then use the head's orientation as a controller.
            if (!main_character || !main_character)
            {
                main_character = GameObject.FindObjectOfType<Ship>().gameObject.internal_game_object.transform;
                main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            }
            Vector3 direction = Bearing.attractor(main_character.forward, main_controller.forward);
            float target_angle = Vector3.SignedAngle(main_character.up, direction, main_character.forward) * Mathf.Deg2Rad;
            float target_distance = Vector3.Angle(main_character.forward, main_controller.forward) * Mathf.Deg2Rad;
            target_distance *= 3;
            if (target_distance > 1) // FIXME: doesn't work for unbounded input types
            {
                target_distance = 1;
            }
            // add velocity based on input
            Vector2 input_direction = new Vector2(-Mathf.Sin(target_angle), Mathf.Cos(target_angle)) * target_distance;
            return input_direction;
        }
        
        private static Transform main_character;
        private static Transform main_controller;

        private const float seconds_until_head_control = 20f;
        private static int last_input_frame = int.MinValue;
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