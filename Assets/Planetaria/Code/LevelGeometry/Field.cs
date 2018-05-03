using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public class Field : MonoBehaviour
    {   
        public static GameObject field(List<GeospatialCurve> curves) // TODO: add convex check asserts.
        {
            GameObject result = new GameObject("Field");
            Field field = result.AddComponent<Field>(); // FIXME: implement
            field.curve_list = curves;

            return result;
        }

        /// <summary>
        /// Checks if the center of mass (position) is below all of the planes.
        /// Special note: if a portion of the extruded volume is inside all planes, this function might still return false.
        /// </summary>
        /// <param name="position">A position on a unit-sphere.</param>
        /// <param name="radius">The radius [0,PI/2] to extrude.</param>
        /// <returns>True if position is below (inside) all of the planes; false otherwise.</returns>
        public bool contains(Vector3 position, float radius = 0f)
        {
            if (!active)
            {
                return false;
            }

            // likely redundant with PlanetariaCollider to do extra checks

            return true;
        }

        private void Start()
        {
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            PlanetariaCache.cache(this);
        }

        private void OnDestroy()
        {
            PlanetariaCache.uncache(this);
        }

        public IEnumerable<GeospatialCurve> iterator()
        {
            return new List<GeospatialCurve>(curve_list);
        }

        public bool active { get; set; }
        
        public bool is_dynamic;
        [System.NonSerialized] public new PlanetariaTransform transform;
        [SerializeField] private List<GeospatialCurve> curve_list = new List<GeospatialCurve>();
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/