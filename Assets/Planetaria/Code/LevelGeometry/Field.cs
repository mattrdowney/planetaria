using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public class Field : MonoBehaviour
    {   
        public static GameObject field(List<GeospatialCurve> curves) // TODO: add convex check asserts.
        {
            GameObject result = new GameObject("Field");
            Field field = result.AddComponent<Field>(); // FIXME: implement
            field.curve_list = curves;

            return result;
        }

        private void Start()
        {
            initialize();
            PlanetariaCache.instance().cache(this);
        }

        private void Reset()
        {
            initialize();
        }

        private void initialize()
        {
            if (transform == null)
            {
                transform = this.GetOrAddComponent<PlanetariaTransform>();
            }
        }

        private void OnDestroy()
        {
            PlanetariaCache.instance().uncache(this);
        }

        public IEnumerable<Arc> iterator() // TODO: check
        {
            List<Arc> arcs = new List<Arc>();

            GeospatialCurve last_curve = Enumerable.Last(curve_list);
            foreach (GeospatialCurve curve in curve_list)
            {
                arcs.Add(Arc.curve(last_curve.point, last_curve.slope, curve.point));
                last_curve = curve;
            }

            return arcs;
        }

        public bool active { get; set; }
        
        public bool is_dynamic;
        [NonSerialized] public new PlanetariaTransform transform;
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