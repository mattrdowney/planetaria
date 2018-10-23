using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    [Serializable]
	public sealed class ArcLight : PlanetariaLight
	{
        // Properties (Public)

        public Arc arc
        {
            get
            {
                return arc_variable;
            }
            set
            {
                arc_variable = value;
                initialize();
            }
        }
        
		// Methods (non-Public)

        private void initialize()
        {
            ArcPlanetarium arc_light = new ArcPlanetarium(arc, color, intensity, range);
            CubePlanetarium cube = new CubePlanetarium(256, arc_light, 1);
            arc_light_cucoloris = cube.to_cubemap();
            internal_light.cookie = arc_light_cucoloris;
        }

		// Messages (non-Public)

        protected override sealed void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override sealed void Reset()
        {
            base.Reset();
            initialize();
        }

		// Variables (non-Public)
		
        [SerializeField] private Arc arc_variable;
        [NonSerialized] [HideInInspector] private Cubemap arc_light_cucoloris;
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