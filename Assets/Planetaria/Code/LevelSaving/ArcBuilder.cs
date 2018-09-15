#if UNITY_EDITOR 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class ArcBuilder : MonoBehaviour  // TODO: PlanetariaComponent
    {
        public static ArcBuilder arc_builder(Vector3 first_point, bool is_field, bool allow_self_intersections)
        {
            GameObject game_object = new GameObject("Arc Builder");
            ArcBuilder result = game_object.AddComponent<ArcBuilder>();
            result.point = result.original_point = first_point;
            GeospatialCurve curve = GeospatialCurve.curve(first_point, first_point);
            bool has_corners = !is_field;
            result.final_shape = new PlanetariaShape(false, has_corners);
            result.debug_shape = result.final_shape.append(curve);
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
                debug_shape = final_shape.append(GeospatialCurve.curve(point, slope)).append(GeospatialCurve.curve(slope, original_point));
            }
            else // CreationState.SetPoint
            {
                point = vector;
                debug_shape = final_shape.append(GeospatialCurve.curve(point, original_point));
            }
        }

        public void next()
        {
            if (state == CreationState.SetSlope)
            {
                final_shape = final_shape.append(GeospatialCurve.curve(point, slope));
            }
            state = (state == CreationState.SetSlope ? CreationState.SetPoint : CreationState.SetSlope); // state = !state;
        }

        public void close_shape()
        {
            if (state == CreationState.SetSlope)
            {
                final_shape = final_shape.append(GeospatialCurve.curve(point, original_point));
            }

            GameObject shape = new GameObject("CustomGeometry");
            PlanetariaCollider collider = shape.AddComponent<PlanetariaCollider>();
            collider.shape = new PlanetariaShape(final_shape.to_curves(), true, true);
            collider.is_field = is_field;
            DestroyImmediate(this.gameObject);

            /*
            optional<TextAsset> svg_file = BlockRenderer.render(block, 0);
            if (svg_file.exists)
            {
                PlanetariaRenderer renderer = shape.AddComponent<PlanetRenderer>();
                renderer.material = RenderVectorGraphics.render(svg_file.data);
            }
            */
        }

        public bool valid()
        {
            if (must_be_convex)
            {
                if (!debug_shape.is_convex_hull())
                {
                    Debug.LogWarning("Not a convex shape! Turn off the LevelEditor setting if you want to ignore this.");
                    return false;
                }
            }
            if (!allow_self_intersections)
            {
                if (debug_shape.is_self_intersecting())
                {
                    Debug.LogWarning("Shape intersects self! Turn off the LevelEditor setting if you want to ignore this.");
                    return false;
                }
            }
            return true;
        }

        private PlanetariaShape final_shape;
        public PlanetariaShape debug_shape;
        private CreationState state = CreationState.SetSlope;
        private Vector3 point { get; set; }
        private Vector3 slope { get; set; }
        private Vector3 original_point { get; set; }
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