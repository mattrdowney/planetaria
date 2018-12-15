using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// Procedurally generates engine noises.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
	public class ShipAudio : MonoBehaviour // FIXME: PlanetariaMonoBehaviour doesn't work?
    {
        private void OnAudioFilterRead(float[] data, int channels)
        {
            // I really like the starting point here:
            // https://www.gamasutra.com/blogs/JoeStrout/20170223/292317/Procedural_Audio_in_Unity.php
            // although, I will try to eliminate the low-pass filter (by multiplying by ~0 when engine is off and ~1 when engine is on)
            // I want to use a "square wave" with durations defined by either poisson or exponential distributions
            // and values defined by a normal distribution with high variance.
            // multiplying by a low value (e.g. 0.1 for idle) would make the engine a lower-pitched rumble (nevermind, I'm dumb.)

            // I would have used a package (I even tried installing), but there are both licensing and compatibility issues. 
            // Probably for the better since I'm not the only developer Planetaria is intended for.

            // this should be fast enough to generate, anyway, even if it's in a parallel thread, so using a simple algorithm like .-^-.

            System.Random random_number_generator = new System.Random();
            for (int sample = 0; sample < data.Length; sample += 1)
            {
                if (repeats_left <= 0)
                {
                    float random_number = (float) random_number_generator.NextDouble();
                    wave_value = PlanetariaMath.triangular_distribution(random_number, -0.1f, 0, +0.1f);
                    random_number = (float) random_number_generator.NextDouble();
                    repeats_left = Mathf.FloorToInt(PlanetariaMath.triangular_distribution(random_number, 172, 255, 518)); // FIXME: MAGIC NUMBER: I used human voice frequency as a template
                }
                data[sample] = wave_value;
                repeats_left -= 1;
            }
        }

        private float wave_value;
        private int repeats_left = 0;

        //protected override void OnConstruction() { }
        //protected override void OnDestruction() { }
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