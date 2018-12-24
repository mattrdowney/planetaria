using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    /// <summary>
    /// Procedurally generates engine noises.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SatelliteAudio : PlanetariaMonoBehaviour
    {
        private void Start()
        {
            satellite = this.GetComponent<Satellite>();
            satellite_rigidbody = this.GetComponent<PlanetariaRigidbody>();
        }

        private void Update()
        {
            if (satellite.life() == 0)
            {
                //thruster_force = Mathf.Max(0, thruster_force - Time.deltaTime*2f);
                thruster_force *= Mathf.Pow(0.1f, Time.deltaTime); // works better with logarithmically-perceived scale for hearing
                return;
            }
            player_life = satellite.life();
            Vector2 player_velocity = satellite_rigidbody.relative_velocity;
            Vector2 player_acceleration = DebrisNoirsInput.get_axes();
            float next_thruster_force = Mathf.Lerp(minimum_volume_multiplier, 1, player_acceleration.magnitude);
            if (next_thruster_force > thruster_force)
            {
                // immediately increasing volume is okay
                thruster_force = next_thruster_force;
            }
            else
            {
                // when changing acceleration into reverse, there are sometime small sound gaps when you don't want them.
                thruster_force = Mathf.Lerp(thruster_force, next_thruster_force, Time.deltaTime / Mathf.Abs(next_thruster_force - thruster_force)); // when changing acceleration (there are sometime small sound gaps when you don't want them).
            }
            thruster_resistance = 0; // force is more precisely representing how much "change" is happening (in angle/direction of satellite)
            if (player_acceleration != Vector2.zero) // if thrusters are on, there is likely a change in direction
            {
                Vector2 partial_velocity = player_velocity / Mathf.PI;
                if (partial_velocity.magnitude > 1)
                {
                    partial_velocity.Normalize();
                }
                thruster_resistance = (player_acceleration.magnitude - Vector2.Dot(partial_velocity, player_acceleration)) / 2; // moving forward implies 0 force; backwards implies 1 force
            }
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
                    random_number = (float)random_number_generator.NextDouble();
                    // this determines the local amplitude (volume)
                    wave_value = PlanetariaMath.triangular_distribution(random_number, -volume, 0, +volume);
                    random_number = (float)random_number_generator.NextDouble();
                    // this approximately determines the pitch
                    repeats_left = Mathf.FloorToInt(PlanetariaMath.triangular_distribution(random_number, 172, 259, 518)); // FIXME: MAGIC NUMBER:
                }
                random_number = (float)random_number_generator.NextDouble();
                float resistance_multiplier = 1 + thruster_variance_multiplier * random_number * thruster_resistance;
                data[sample] = wave_value * resistance_multiplier * thruster_force * player_life;
                repeats_left -= 1;
            }
        }

        private Satellite satellite;
        private PlanetariaRigidbody satellite_rigidbody;
        private bool player_dead;

        private const float volume = 0.025f;
        private const float minimum_volume_multiplier = 0.5f;
        private const float thruster_variance_multiplier = 2f; // does not include original (1)
        private float thruster_force;
        private float thruster_resistance;
        private float player_life;
        private float acceleration;
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