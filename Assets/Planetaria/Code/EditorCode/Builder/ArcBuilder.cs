#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class ArcBuilder : MonoBehaviour  // TODO: PlanetariaComponent
    {
        public static ArcBuilder arc_builder(Vector3 first_point, bool convex, bool allow_self_intersections)
        {
            GameObject game_object = new GameObject("ArcBuilder");
            ArcBuilder result = game_object.AddComponent<ArcBuilder>();
            result.point = result.previous_point = result.original_point = first_point;
            result.shape = PlanetariaShape.Create();
            result.must_be_convex = convex; // must be a "convex hull"
            result.allow_self_intersections = allow_self_intersections;
            return result;
        }

        public void receive_vector(Vector3 vector)
        {
            if (state == CreationState.SetSlope)
            {
                slope = vector;
                shape.append(ArcFactory.line(point, slope), PlanetariaShape.AppendMode.OverwriteWithEphemeral);
            }
            else // CreationState.SetPoint
            {
                point = vector;
                shape.append(ArcFactory.curve(previous_point, slope, point), PlanetariaShape.AppendMode.OverwriteWithEphemeral);
            }
        }

        public void next()
        {
            if (state == CreationState.SetPoint)
            {
                shape.append(ArcFactory.curve(previous_point, slope, point));
                previous_point = point;
            }
            state = (state == CreationState.SetSlope ? CreationState.SetPoint : CreationState.SetSlope); // state = !state;
        }

        public void close_shape()
        {
            if (state == CreationState.SetSlope)
            {
                shape.close(new optional<Vector3>());
            }
            else
            {
                shape.close(slope);
            }
            shape.save();
            DestroyImmediate(this.gameObject);
        }

        public bool valid()
        {
            if (must_be_convex)
            {
                if (!shape.is_convex_hull())
                {
                    Debug.LogWarning("Not a convex shape! Turn off the LevelEditor setting if you want to ignore this.");
                    return false;
                }
            }
            if (!allow_self_intersections)
            {
                if (shape.is_self_intersecting())
                {
                    Debug.LogWarning("Shape intersects self! Turn off the LevelEditor setting if you want to ignore this.");
                    return false;
                }
            }
            return true;
        }

        public PlanetariaShape shape;
        private CreationState state = CreationState.SetSlope;
        private Vector3 original_point { get; set; }
        private Vector3 previous_point { get; set; }
        private Vector3 point { get; set; }
        private Vector3 slope { get; set; }
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