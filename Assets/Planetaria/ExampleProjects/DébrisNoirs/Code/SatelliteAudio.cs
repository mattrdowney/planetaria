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
        private void Start()
        {
            satellite = this.GetComponent<PlanetariaRigidbody>();
        }

        private void Update()
        {
            Vector2 player_velocity = satellite.relative_velocity;
            Vector2 player_acceleration = DebrisNoirsInput.movement();
            float player_acceleration_angle = Mathf.Atan2(player_acceleration.y, player_acceleration.x);
            Vector2 partial_velocity = player_velocity/3;
            if (partial_velocity.magnitude > 1)
            {
                partial_velocity.Normalize();
            }


            thruster_force = (1 - Vector2.Dot(partial_velocity, player_acceleration))/2; // moving forward implies 0 force; backwards implies 1 force
            thruster_force *= player_acceleration.magnitude; // FIXME: almost right, but part wrong // idea: if the thrusters aren't being used then don't assume 0.5 force
        }

        // use dot product of velocity and input to determine if the player is fighting their momentum.
        // if they are, add extra noise so it sounds like the thrusters are working harder (harsher noise).

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // Procedural engine audio based on: https://www.gamasutra.com/blogs/JoeStrout/20170223/292317/Procedural_Audio_in_Unity.php
            System.Random random_number_generator = new System.Random();
            // go through the audio clip that needs to be generated
            for (int sample = 0; sample < data.Length; sample += 1)
            {
                float random_number;
                // play a sound for multiple frames (e.g. a "square wave")
                if (repeats_left <= 0) // but randomly generate new samples as needed
                {
                    random_number = (float) random_number_generator.NextDouble();
                    // this determines the local amplitude (volume)
                    wave_value = PlanetariaMath.triangular_distribution(random_number, -volume, 0, +volume);
                    random_number = (float) random_number_generator.NextDouble();
                    // this approximately determines the pitch
                    repeats_left = Mathf.FloorToInt(PlanetariaMath.triangular_distribution(random_number, 100, 150, 200)); // FIXME: MAGIC NUMBER:
                }
                random_number = (float) random_number_generator.NextDouble();
                float thruster_multiplier = 1 + thruster_variance_multiplier*random_number*thruster_force;
                data[sample] = wave_value * thruster_multiplier;
                repeats_left -= 1;
            }
        }

        private PlanetariaRigidbody satellite;

        private const float volume = 0.004f;
        private const float thruster_variance_multiplier = 2f;
        private float thruster_force;
        private float wave_value;
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