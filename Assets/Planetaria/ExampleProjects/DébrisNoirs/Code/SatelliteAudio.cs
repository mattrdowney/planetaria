using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// Procedurally generates engine noises.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
	public class SatelliteAudio : PlanetariaMonoBehaviour
    {
        private void Update()
        {
            player_velocity = DebrisNoirsInput.movement();
            float player_velocity_angle = Mathf.Atan2(player_velocity.y, player_velocity.x);

            // angle = PI/2 --> ramp 1 to 1.5
            // angles = (0, PI) --> ramp 1 to 1 (no ramp)
            // angles = -PI --> ramp 1.5 to 1
            // this means the values are approximately Sin(angle) or velocity.y, but you want even distribution
            if (player_velocity_angle >= Mathf.PI/2) // Quadrant II
            {
                velocity_interpolator = Mathf.Lerp(1, 0, (player_velocity_angle - (Mathf.PI/2))/(Mathf.PI/2));
            }
            else if (player_velocity_angle >= 0) // Quadrant I
            {
                velocity_interpolator = Mathf.Lerp(0, 1, player_velocity_angle/(Mathf.PI/2));
            }
            else if (player_velocity_angle >= -Mathf.PI/2) // Quadrant IV
            {
                velocity_interpolator = Mathf.Lerp(0, -1, -player_velocity_angle/(Mathf.PI/2));
            }
            else // if (player_velocity_angle < -Mathf.PI/2) // Quadrant III
            {
                velocity_interpolator = Mathf.Lerp(-1, 0, -(player_velocity_angle + (Mathf.PI/2))/(Mathf.PI/2));
            }

            // CONSIDER: two controls for ramp height - left and right (one based on sine, other based on cosine).
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // Procedural engine audio based on: https://www.gamasutra.com/blogs/JoeStrout/20170223/292317/Procedural_Audio_in_Unity.php
            System.Random random_number_generator = new System.Random();
            // go through the audio clip that needs to be generated
            for (int sample = 0; sample < data.Length; sample += 1)
            {
                // play a sound for multiple frames (e.g. a "square wave")
                if (repeats_left <= 0) // but randomly generate new samples as needed
                {
                    // TODO: figure out how to change the noise based on the direction of movement e.g. North/South (without changing too many audio properties).
                    // most likely, the sound should come from in front, but if you changed the position to the direction extruded PI/2 it might have the best effect
                    // e.g. heading down would make the sound come from below, up from above, left from the left and right from the right.
                    // alternatively, you could make the pitch lower for heading south, higher for heading north
                    // you could also make the wave_value (-0.1f, 0.1f*velocity.x, +0.1f) - doesn't seem to work (as I'd expect).
                    // get random numbers to make the sound less predictable
                    float random_number = (float) random_number_generator.NextDouble();
                    // this determines the local amplitude (volume)
                    wave_value = PlanetariaMath.triangular_distribution(random_number, -volume, 0, +volume);
                    random_number = (float) random_number_generator.NextDouble();
                    // this approximately determines the pitch
                    total_repeats = Mathf.FloorToInt(PlanetariaMath.triangular_distribution(random_number, 100, 150, 200)); // FIXME: MAGIC NUMBER:
                    repeats_left = total_repeats;
                }
                // actually create the sound
                if (velocity_interpolator > 0) // moving up
                {
                    data[sample] = wave_value * (1 + (1-(repeats_left / (float)total_repeats)) * velocity_interpolator * velocity_multiplier);
                }
                else // moving down
                {
                    data[sample] = wave_value * (1 + (repeats_left / (float)total_repeats) * velocity_interpolator * velocity_multiplier);
                }
                repeats_left -= 1;
            }
            for (int sample = 0; sample < data.Length; sample += 1)
            {
                data[sample] *= (1+(player_velocity.x+1)*velocity_multiplier*velocity_multiplier);
            }
        }

        private const float volume = 0.03f;
        private Vector2 player_velocity;
        private float velocity_interpolator;
        private float velocity_multiplier = 0.4f;
        private float wave_value;
        private int total_repeats = 0;
        private int repeats_left = 0;

        protected override void OnConstruction() { }
        protected override void OnDestruction() { }
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