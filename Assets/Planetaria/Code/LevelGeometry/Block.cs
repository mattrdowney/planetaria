using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class Block : MonoBehaviour
    {
        /// <summary>
        /// Constructor - Creates a block matching a curves list.
        /// </summary>
        /// <returns>A block matching its blueprint.</returns>
        public static GameObject block(List<GeospatialCurve> curves)
        {
            GameObject result = new GameObject("Block");
            Block block = result.AddComponent<Block>();
            block.curve_list = curves;
            block.generate_arcs();
            block.ignore.Add(block);

            return result;
        }

        /// <summary>
        /// Returns the index of any existing arc within the block that matches the external reference. Null arcs are never found.
        /// </summary>
        /// <param name="arc">The reference to the external arc that will be compared to the block's arc list.</param>
        /// <returns>The index of the match if the arc exists in the container and is not null; a nonexistent index otherwise.</returns>
        public optional<ArcVisitor> arc_visitor(Arc arc)
        {
            int arc_list_index = arc_list.IndexOf(arc);

            if (arc_list_index == -1)
            {
                return new optional<ArcVisitor>();
            }

            return ArcVisitor.arc_visitor(arc_list, arc_list_index);
        }

        public IEnumerable<optional<Arc>> iterator() // TODO: check
        {
            return arc_list;
        }

        public bool active { get; set; }

        public Vector3 center_of_mass() // FIXME: proper volume integration (for convex hulls)
        {
            Vector3 result = Vector3.zero;
            foreach (GeospatialCurve curve in curve_list)
            {
                result += curve.point;
            }
            result.Normalize();
            return result; // FIXME: Vector3.zero can be returned
        }

        public bool empty()
        {
            return curve_list.Count == 0;
        }

        private void generate_arcs()
        {
            arc_list = new List<optional<Arc>>();
            for (int edge = 0; edge < curve_list.Count; ++edge)
            {
                GeospatialCurve[] curves = new GeospatialCurve[3];
                for (int curve = 0; curve < 3; ++curve)
                {
                    curves[curve] = curve_list[(edge+curve)%curve_list.Count];
                }
                Arc left_arc = Arc.curve(curves[0].point, curves[0].slope, curves[1].point);
                Arc right_arc = Arc.curve(curves[1].point, curves[1].slope, curves[2].point);
                arc_list.Add(left_arc);
                if (Vector3.Angle(left_arc.normal(left_arc.angle()), right_arc.normal(0)) > Precision.tolerance)
                {
                    arc_list.Add(Arc.corner(left_arc, right_arc));
                }
            }
        }

        private void Start()
        {
            active = true;
            generate_arcs();
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            PlanetariaCache.cache(this);
        }

        private void Reset()
        {
            generate_arcs();
        }

        private void OnDestroy()
        {
            PlanetariaCache.uncache(this);
        }
        
        public bool is_dynamic;
        public bool is_platform;
        public static PlanetariaPhysicMaterial fallback;
        public PlanetariaPhysicMaterial material = fallback;
        [System.NonSerialized] public new PlanetariaTransform transform;
        [System.NonSerialized] private List<optional<Arc>> arc_list = new List<optional<Arc>>();
        [SerializeField] private List<GeospatialCurve> curve_list = new List<GeospatialCurve>();
        [SerializeField] public List<Block> ignore = new List<Block>();
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