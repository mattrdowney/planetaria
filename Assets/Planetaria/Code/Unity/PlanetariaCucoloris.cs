using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// Similar to UnityEngine.Cookie, this is an overlay across a light region, but instead of being 2-dimensional, it is 1-dimensional.
    /// </summary>
    [Serializable]
	public class PlanetariaCucoloris
	{
		// Properties (Public)

        /// <summary>A 1-dimensional Texture (only the full width of the first row used). The texture represents the 360 degrees or sector angle's pixel colors. The center pixel (i.e. width/2) represents forward relative to the light.</summary>
        public Texture2D cucoloris
        {
            get
            {
                return user_cucoloris;
            }
            set
            {
                user_cucoloris = value;
                recalculate();
            }
        }
		
		// Methods (Public)
		
		// Static Methods (Public)
		
		// Properties (non-Public)
		
		// Methods (non-Public)
		
        private void recalculate()
        {
            // create ~ Texture2D(1024,1024) [user-specified resolution? or inferred from user_cucoloris?]
            // go through width and height:
            //     get pixel coordinates, convert to polar coordinates using UVCoordinate conversion
            //     from polar coordinates, fetch closest user_cucoloris pixel (with special considerations for sector light)
            //         point light: 90 degrees maps to width/2, 0 degrees maps to 3width/4, 271 degrees maps to ~width
            //                 180 degrees maps to width/4, 269 degrees maps to ~0
            //         sector light: 90 degrees maps to width/2, 90 - sector_angle/2 maps to 0, and 90 + sector_angle/2 maps to width
            //     get pixel lighting at position (no lighting outside of radius - because of how intensity multiplication works)
            //     multiply pixel lighting by user_cucoloris to get final color at pixel
        }

		// Static Methods (non-Public)
		
		// Messages (non-Public)
				
		// Variables (Public)
		
		// Variables (non-Public)
		
        [SerializeField] private Texture2D user_cucoloris; // this is a functionally-1D (width only of 1st row) 
        [NonSerialized] [HideInInspector] private Texture2D internal_cucoloris; // This is the procedural Unity "Cookie" used for the Unity Spot Light.
        // CONSIDER: static private Texture2D internal_particle_image? (or similar reference)
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