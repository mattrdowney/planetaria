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
                    // Look at this hot garbage:

                    // Implement https://en.wikibooks.org/wiki/Cg_Programming/Unity/Cookies
                    // With particular notice of "cookieAttenuation = tex2D(_LightTexture0, input.posLight.xy / input.posLight.w + float2(0.5, 0.5)).a"
                    // w = z / d
                    //     where d = 1/tan(FoV(y)/2)
                    // UV conversion, restated, is: (x,y)/w + (0.5,0.5) = (u,v) [ I should probably compute this via sampling, since that's simpler ] <-- Nevermind this comment, Wolfram Alpha equation solver did it (I was being lazy, I think it would have been easy enough to solve, and I should probably derive the equation as homework)
                    // we know x^2 + y^2 + z^2 = 1
                    // z^2 = 1 - x^2 - y^2
                    // z = +/-sqrt(1 - x^2 - y^2)
                    // [ u >= 0.5 --> x is positive ]
                    // [ v >= 0.5 --> y is positive ]
                    //
                    // thus:
                    // tan(FoV/2)*x/sqrt(1 - x^2 - y^2) = u and [I omitted the .5, a happy accident (the more-difficult equation might be required, though)]
                    // tan(FoV/2)*y/sqrt(1 - x^2 - y^2) = v
                    // solve for x,y (in Wolfram Alpha)
                    // returns:
                    // x = u/sqrt(tan^2(FoV/2) + u^2 + v^2) [you need to multiply by sign(u-.5) and "u" is actually `u - .5`]
                    // y = v/sqrt(tan^2(FoV/2) + u^2 + v^2)

                    // notably, as FOV approaches 180 degrees, only the centermost pixels are rendered; tan(90) approaches infinity, dividing by ~ infinity approaches 0, which is likely the root of the behavior
                    // Also, I am essentially doing this:
                    // use UV coordinates of entire texture:
                    // convert UV -> XY using above system of equations.
                    // calculate z = 1 - x^2 - y^2
                    // calculate Vector3.Angle(Vector3.forward, new Vector3(x,y,z));
                    // use angle to determine lighting using any function desired (the dot product probably isn't the way to go, I prefer finding the length of the circle at the given radius to compute how spread out (unintense) the light is).

                    // FIXME: this needs knowledge of the spotlight's field of view (this was "d", which I missed)

                    UVCoordinates uv = new UVCoordinates(u, v);
                    PolarCoordinates polar = PolarCoordinates.rectangular(uv.data - new Vector2(0.5f, 0.5f)); // Get polar coordinates relative to UV center
                    //polar = PolarCoordinates.polar(polar.radius, polar.angle + Mathf.PI/2); // Rotate 90 degrees counterclockwise.

                    // we are interested in points of radius [0, 0.5].
                    float angular_distance = Mathf.PI*polar.radius; // this range maps to [0, PI/2]
                    float dot_product = Mathf.Clamp(Mathf.Sin(angular_distance), Precision.just_above_zero, 1); // Sine range is [-1, +1] // dot product uses Cos, right? (I changed this at some point, I think)
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