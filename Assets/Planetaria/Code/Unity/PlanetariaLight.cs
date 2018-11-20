using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// A PlanetariaLight is of type PointLight, ArcLight, SectorLight, or WorldLight.
    /// </summary>
    [Serializable]
	public sealed class PlanetariaLight : PlanetariaComponent
	{
		// Properties (Public)

        /// <summary>The color of the light.</summary>
        public Color color
        {
            get
            {
                return color_variable;
            }
            set
            {
                color_variable = value;
            }
        }

        /// <summary>The cuculoris texture projected by the light.</summary>
        public PlanetariaCuculoris cuculoris // TODO: figure out how to do this, since I will be using Unity's cuculoris system for the different light types.
        {
            get
            {
                return cuculoris_variable;
            }
            set
            {
                cuculoris_variable = value;
            }
        }

        /// <summary>This is used to light certain objects in the scene selectively.</summary>
        public int cullingMask
        {
            get
            {
                return culling_mask_variable;
            }
            set
            {
                culling_mask_variable = value;
            }
        }

        /// <summary>The intensity of the light is multiplied by the light color and cuculoris (if present).</summary>
        public float intensity
        {
            get
            {
                return intensity_variable;
            }
            set
            {
                intensity_variable = value; // TODO: clamp to range
            }
        }

        /// <summary>The radius (as an angle in radians) of the light. Range: [0, PI].</summary>
        public float range
        {
            get
            {
                return range_variable;
            }
            set
            {
                range_variable = value;
            }
        }

        /// <summary>The angle of the circular sector (in radians) of the sector light. Range: [0, 2PI].</summary>
        public float sectorAngle
        {
            get
            {
                return sector_angle_variable;
            }
            set
            {
                sector_angle_variable = value;
            }
        }

        // While shadows can conceptually exist (I think I mis-stated that before), they should have substantial research and development (for runtime variants)
        // Conceptually, PointLight, ArcLight, and SectorLight can have shadows (WorldLight - perhaps better titled AmbientLight - cannot)
        // Shadows should have the option of rendering in different colors (at least for debugging purposes).
        // While shadow color might not seem too relevant, virtual reality defaults to black rendering for unknown pixels, so this feature has both developer and game uses.
        
        /// <summary>The type of the light (e.g. PointLight, ArcLight, SectorLight, WorldLight).</summary>
        public PlanetariaLightType type
        {
            get
            {
                return type_variable;
            }
            set
            {
                type_variable = value;
            }
        }

            // FIXME: missing common UnityEngine.Light properties
		
		// Methods (Public)

            // FIXME: missing all UnityEngine.Light public methods

        public static Texture2D lighting_function(float light_field_of_view, int resolution = 1024)
        {
            Texture2D light_cuculoris = new Texture2D(resolution, resolution);
            Color32[] pixels = new Color32[light_cuculoris.width*light_cuculoris.height];
            float pixel_width = 1f/light_cuculoris.width;
            float pixel_height = 1f/light_cuculoris.height;
            float field_of_view_constant = Mathf.Pow(Mathf.Tan(light_field_of_view/2*Mathf.Deg2Rad), -2); // FIXME: recompute on resized camera fieldOfView

            int pixel = 0;
            for (float v = pixel_height/2; v < 1f; v += pixel_height)
            {
                for (float u = pixel_width/2; u < 1f; u += pixel_width)
                {
                    // Implement https://en.wikibooks.org/wiki/Cg_Programming/Unity/Cookies
                    // With particular notice of "cookieAttenuation = tex2D(_LightTexture0, input.posLight.xy / input.posLight.w + float2(0.5, 0.5)).a"
                    // w = z / d
                    //     where d = 1/tan(FoV(y)/2)
                    // UV conversion, restated, is: (x,y)/w + (0.5,0.5) = (u,v) 
                    // we know x^2 + y^2 + z^2 = 1 (unit sphere definition, which is a given in Planetaria)
                    // therefore: z = +/-sqrt(1 - x^2 - y^2)
                    // [ u >= 0.5 --> x is positive ]
                    // [ v >= 0.5 --> y is positive ]
                    //
                    // thus:
                    // (1/tan(FoV/2))*x/sqrt(1 - x^2 - y^2) = u and [I omitted the .5, a happy accident]
                    // (1/tan(FoV/2))*y/sqrt(1 - x^2 - y^2) = v
                    // solve for x,y (in Wolfram Alpha)
                    // returns:
                    // x = u/sqrt(1/tan^2(FoV/2) + u^2 + v^2) ["u"/"v" is actually `u - .5`/`v - .5`]
                    // y = v/sqrt(1/tan^2(FoV/2) + u^2 + v^2)
                    float signed_u = u - 0.5f;
                    float signed_v = v - 0.5f;
                    float signed_u_squared = signed_u*signed_u;
                    float signed_v_squared = signed_v*signed_v;
                    float x = signed_u/Mathf.Sqrt(field_of_view_constant/4 + signed_u_squared + signed_v_squared);
                    float y = signed_v/Mathf.Sqrt(field_of_view_constant/4 + signed_u_squared + signed_v_squared);
                    
                    // calculate z = sqrt(1 - x^2 - y^2)
                    float z = Mathf.Sqrt(1 - x*x - y*y);

                    // calculate distance to pixel (relative to field of view)
                    Vector3 cartesian = new Vector3(x,y,z);
                    float angle = Vector3.Angle(Vector3.forward, cartesian) * Mathf.Deg2Rad;
                    
                    // CONSIDER: find the length of the circle at given angle to compute how spread out (unintense) the light is i.e. Mathf.Pos(Mathf.Sin(ratio*Mathf.PI/2), -2) but a cursory check shows that I would need to improve the function significantly).
                    float ratio = Mathf.Clamp(angle/(light_field_of_view/2*Mathf.Deg2Rad), 0, 1); // TODO: different function
                    byte intensity_color = (byte) Mathf.CeilToInt((1f-ratio)*byte.MaxValue);
                    pixels[pixel] = new Color32(0, 0, 0, intensity_color);
                    pixel += 1;
                }
            }
            light_cuculoris.SetPixels32(pixels);
            light_cuculoris.Apply();
            return light_cuculoris;
        }
    	
        // Methods (non-Public)

        private void initialize()
        {
            GameObject game_object = this.GetOrAddChild("InternalLight");
            if (internal_light == null)
            {
                internal_light = Miscellaneous.GetOrAddComponent<Light>(game_object);
            }
            internal_light.range = 1000f; // FIXME: magic number: Setting light ranges to float.MaxValue does not work; the max range for SpotLights is a lot worse than PointLights.
            internal_light.shadows = LightShadows.None;
            internal_light.type = LightType.Spot;
            internal_light.spotAngle = (range * 2) * Mathf.Rad2Deg;
            if (internal_cuculoris == null)
            {
                internal_cuculoris = lighting_function(internal_light.spotAngle);
                cuculoris.apply_to(ref internal_cuculoris, sectorAngle);
            }
            internal_light.cookie = internal_cuculoris;
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

        // Variables (Public)
        
        [SerializeField] private PlanetariaCuculoris cuculoris_variable; // this is technically a part of the public interface (because it is shown in the editor)
		[SerializeField] private Color color_variable = Color.white;
        [SerializeField] private int culling_mask_variable = 0;
        [SerializeField] private float intensity_variable = 1;
        [SerializeField] private float range_variable = 0.5f;
        [SerializeField] private float sector_angle_variable = 2*Mathf.PI;
        [SerializeField] private PlanetariaLightType type_variable = PlanetariaLightType.SectorLight; // FIXME: this does nothing

		// Variables (non-Public)
        
        [SerializeField] [HideInInspector] private Texture2D internal_cuculoris;
        [SerializeField] [HideInInspector] private Light internal_light; // CONSIDER: use an array of UnityEngine.Light? - most likely no, since that could be added in the child class if necessary
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