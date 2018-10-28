using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    [Serializable]
	public sealed class PointLight : PlanetariaLight
	{
        // Methods (non-Public)

        private void initialize()
        {
            if (internal_cucoloris == null)
            {
                internal_cucoloris = lighting_function();
            }
            internal_light.cookie = internal_cucoloris;
            internal_light.spotAngle = range * Mathf.Rad2Deg;
        }

        public Texture2D lighting_function()
        {
            const int resolution = 1024;
            Texture2D light_cucoloris = new Texture2D(resolution, resolution);
            Color32[] pixels = new Color32[light_cucoloris.width*light_cucoloris.height];
            float pixel_width = 1f/light_cucoloris.width;
            float pixel_height = 1f/light_cucoloris.height;

            int pixel = 0;
            for (float v = pixel_height/2; v < 1f; v += pixel_height)
            {
                for (float u = pixel_width/2; u < 1f; u += pixel_width)
                {
                    UVCoordinates uv = new UVCoordinates(u, v);
                    PolarCoordinates polar = PolarCoordinates.rectangular(uv.data - new Vector2(0.5f, 0.5f)); // Get polar coordinates relative to UV center
                    //polar = PolarCoordinates.polar(polar.radius, polar.angle + Mathf.PI/2); // Rotate 90 degrees counterclockwise.

                    // we are interested in points of radius [0, 0.5].
                    float angular_distance = Mathf.PI*polar.radius; // this range maps to [0, PI/2]
                    float dot_product = Mathf.Clamp01(Mathf.Cos(angular_distance)); // Cosine range is [-1, +1]
                    float inverse_square = dot_product*dot_product; // Inverse squared lighting (without the inverse!)
                    byte intensity = (byte) Mathf.CeilToInt(inverse_square*byte.MaxValue);
                    pixels[pixel] = new Color32(intensity, intensity, intensity, intensity);
                    pixel += 1;
                }
            }
            light_cucoloris.SetPixels32(pixels);
            light_cucoloris.Apply();
            return light_cucoloris;
        }

		// Messages (non-Public)

        protected override void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override void Reset()
        {
            base.Reset();
            initialize();
        }

		// Variables (non-Public)

        private static Texture2D internal_cucoloris;
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