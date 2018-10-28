using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// A PlanetariaLight is of type PointLight, ArcLight, SectorLight, or WorldLight.
    /// </summary>
    [Serializable]
	public class PlanetariaLight : PlanetariaComponent
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

        /// <summary>The cucoloris texture projected by the light.</summary>
        public PlanetariaCucoloris cucoloris // TODO: figure out how to do this, since I will be using Unity's cucoloris system for the different light types.
        {
            get
            {
                return cucoloris_variable;
            }
            set
            {
                cucoloris_variable = value;
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

        /// <summary>The intensity of the light is multiplied by the light color and cucoloris (if present).</summary>
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
    	
        // Methods (non-Public)

        private void initialize()
        {
            GameObject game_object = this.GetOrAddChild("InternalLight");
            if (internal_light == null)
            {
                internal_light = Miscellaneous.GetOrAddComponent<Light>(game_object);
            }
            internal_light.range = 10f; // FIXME: magic number: Setting light ranges to float.MaxValue does not work; the max range for SpotLights is a lot worse than PointLights.
            internal_light.shadows = LightShadows.None;
            internal_light.type = LightType.Spot;
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
        
        [SerializeField] [HideInInspector] protected Light internal_light; // CONSIDER: use an array of UnityEngine.Light? - most likely no, since that could be added in the child class if necessary

		[SerializeField] private Color color_variable;
        [SerializeField] private PlanetariaCucoloris cucoloris_variable;
        [SerializeField] private int culling_mask_variable;
        [SerializeField] private float intensity_variable;
        [SerializeField] private float range_variable;
        [SerializeField] private PlanetariaLightType type_variable;
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