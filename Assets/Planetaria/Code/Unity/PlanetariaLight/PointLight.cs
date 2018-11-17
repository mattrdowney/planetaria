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
                    // Implement https://en.wikibooks.org/wiki/Cg_Programming/Unity/Cookies
                    // With particular notice of "cookieAttenuation = tex2D(_LightTexture0, input.posLight.xy / input.posLight.w + float2(0.5, 0.5)).a"
                    // w = z / d, where d = 1 (since this is a unit sphere of distance=1), so w = z
                    // thus UV conversion is: (x,y)/z + (0.5,0.5) = (u,v)
                    // thus the inverse is ((u,v) - (0.5,0.5))*z = (x,y)
                    // we do not know z, but we know x^2 + y^2 + z^2 = 1
                    // derivative knowledge:
                    // z^2 = 1 - x^2 - y^2
                    // z = +/-sqrt(1 - x^2 - y^2)
                    // therefore:
                    // ((u,v) - (0.5,0.5)) = +/-(x,y)/sqrt(1 - x^2 - y^2)
                    // breaking this onto two lines:
                    // u - 0.5 = +/-x/sqrt(1 - x^2 - y^2) [ u >= 0.5 --> x is positive ]
                    // v - 0.5 = +/-y/sqrt(1 - x^2 - y^2) [ v >= 0.5 --> y is positive ]
                    // I think there should be another equation based on spotAngle, namely you can find the x/y/z coordinates based on how projection of cucoloris works

                    // FIXME: this needs knowledge of the spotlight's field of view

                    UVCoordinates uv = new UVCoordinates(u, v);
                    PolarCoordinates polar = PolarCoordinates.rectangular(uv.data - new Vector2(0.5f, 0.5f)); // Get polar coordinates relative to UV center
                    //polar = PolarCoordinates.polar(polar.radius, polar.angle + Mathf.PI/2); // Rotate 90 degrees counterclockwise.

                    // we are interested in points of radius [0, 0.5].
                    float angular_distance = Mathf.PI*polar.radius; // this range maps to [0, PI/2]
                    float dot_product = Mathf.Clamp(Mathf.Sin(angular_distance), Precision.just_above_zero, 1); // Sine range is [-1, +1]
                    //float inverse_square = dot_product*dot_product; // Inverse squared lighting (without the inverse!)
                    byte intensity = (byte) Mathf.CeilToInt(dot_product*byte.MaxValue);
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