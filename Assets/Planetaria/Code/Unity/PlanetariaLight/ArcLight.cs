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
            ArcPlanetarium arc_light = new ArcPlanetarium(arc, color, range);
            cubemap_generator.convert(arc_light);
            internal_light.cookie = cubemap_generator.get_cubemap();
            //Debug.LogError("Pausing");
        }

		// Messages (non-Public)

        protected override sealed void Awake()
        {
            base.Awake();
            cubemap_generator = new CubePlanetarium(256);
            initialize();
        }

        protected override sealed void Reset()
        {
            base.Reset();
            cubemap_generator = new CubePlanetarium(256);
            initialize();
        }

		// Variables (non-Public)
		
        [SerializeField] private Arc arc_variable;
        [NonSerialized] [HideInInspector] private CubePlanetarium cubemap_generator;
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