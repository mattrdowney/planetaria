#if UNITY_EDITOR 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class ArcBuilder : MonoBehaviour
    {
        public static ArcBuilder arc_builder(Vector3 original_point, bool is_field, bool allow_self_intersections)
        {
            GameObject game_object = new GameObject("Arc Builder");
            ArcBuilder result = game_object.AddComponent<ArcBuilder>();
            result.point = result.original_point = original_point;
            result.arcs.Add(Arc.line(original_point, original_point));
            result.is_field = is_field;
            result.must_be_convex = is_field; // Fields must be a "convex hull"
            result.allow_self_intersections = allow_self_intersections && !is_field;
            return result;
        }

        public void receive_vector(Vector3 vector)
        {
            if (state == CreationState.SetSlope)
            {
                slope = vector;
                arcs[arcs.Count-1] = Arc.line(point, slope);
            }
            else // CreationState.SetPoint
            {
                if (arcs.Count != 0)
                {
                    arcs[arcs.Count-1] = Arc.curve(arcs[arcs.Count-1].begin(), slope, point);
                }
                point = vector;
            }
        }

        public void next()
        {
            if (state == CreationState.SetSlope)
            {
                finalize();
            }
            state = (state == CreationState.SetSlope ? CreationState.SetPoint : CreationState.SetSlope); // state = !state;
        }

        public void finalize()
        {
            curves.Add(GeospatialCurve.curve(point, slope));
            arcs.Add(arcs[arcs.Count-1]);
            UnityEditor.EditorUtility.SetDirty(this.gameObject);
            point = slope; // point should always be defined
        }

        public void close_shape()
        {
            if (state == CreationState.SetSlope)
            {
                curves.Add(GeospatialCurve.curve(point, original_point));
            }

            if (is_field)
            {
                GameObject field = Field.field(curves);
                Debug.Log(field);
            }
            else
            {
                GameObject shape = Block.block(curves);
                Block block = shape.GetComponent<Block>();
                optional<TextAsset> svg_file = BlockRenderer.render(block, 0);

                if (svg_file.exists)
                {
                    //PlanetariaRenderer renderer = shape.AddComponent<PlanetRenderer>();
                    //renderer.material = RenderVectorGraphics.render(svg_file.data);
                }
            }
            
            DestroyImmediate(this.gameObject);
        }

        public bool valid()
        {
            if (must_be_convex)
            {
                if (!convex_hull())
                {
                    Debug.Log("Concave!");
                    return false;
                }
            }
            if (!allow_self_intersections)
            {
                if (!zero_intersections())
                {
                    Debug.Log("Intersects!");
                    return false;
                }
            }
            Debug.Log("All Good!");
            return true;
        }

        private bool convex_hull()
        {
            if (arcs.Count > 1)
            {
                Arc closing_edge = Arc.line(arcs[arcs.Count - 1].end(), arcs[0].begin());
                Arc last_arc = closing_edge;
                foreach (Arc arc in arcs)
                {
                    if (!Arc.is_convex(last_arc, arc))
                    {
                        Debug.Log("We found a concave corner =(");
                        return false;
                    }
                    last_arc = arc;
                }
                if (!Arc.is_convex(last_arc, closing_edge))
                {
                    Debug.Log("We found a concave corner =(");
                    return false;
                }
            }
            Debug.Log("Convex~!");
            return true;
        }

        private bool zero_intersections()
        {
            for (int left = 0; left < arcs.Count; ++left)
            {
                for (int right = left+1; right < arcs.Count; ++right)
                {
                    optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(arcs[left], arcs[right], 0);
                    if (intersection.exists)
                    {
                        Debug.Log("Unfortunately, two arcs intersect somewhere.");
                        return false;
                    }
                }
            }
            Debug.Log("There were no intersections~!");
            return true;
        }
    
        public List<Arc> arcs = new List<Arc>();
        private List<GeospatialCurve> curves = new List<GeospatialCurve>();
        private CreationState state = CreationState.SetSlope;
        private Vector3 point { get; set; }
        private Vector3 slope { get; set; }
        private Vector3 original_point;
        private bool is_field;
        private bool must_be_convex;
        private bool allow_self_intersections;

        private enum CreationState
        {
            SetSlope = 0,
            SetPoint = 1,
        }
    }
}

#endif

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
