using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public static class ArcUtility
    {
        public static List<Arc> get_all_arcs()
        {
            List<Arc> result = new List<Arc>();
            Debug.LogError("Broken: add Block + Field as PlanetariaCollider"); // FIXME:
            return result;
        }

        /// <summary>
        /// Inspector - Finds the closest vertex (arc boundary) to a input point.
        /// </summary>
        /// <param name="arc">The Arc whose boundaries will be checked.</param>
        /// <param name="point">A normalized point in 3D space.</param>
        /// <returns>Either arc.begin() or arc.end() depending on which is closer to the input point.</returns>
        public static Vector3 snap_to_vertex(Arc arc, Vector3 point)
        {
            Vector3 begin = arc.begin();
            Vector3 end = arc.end();
            float begin_similarity = Vector3.Dot(point, begin);
            float end_similarity = Vector3.Dot(point, end);
            if (begin_similarity > end_similarity)
            {
                return begin;
            }
            return end;
        }

        /// <summary>
        /// Inspector - Finds the closest point (along the arc edge) to a input point.
        /// </summary>
        /// <param name="arc">The Arc whose edge will be checked.</param>
        /// <param name="point">A normalized point in 3D space.</param>
        /// <returns>The closest point along the arc's edge to the input point.</returns>
        public static Vector3 snap_to_edge(Arc arc, Vector3 point)
        {
            float angle = arc.position_to_angle(point);
            if (Mathf.Abs(angle) <= arc.angle()/2) // valid - within boundaries
            {
                return arc.position(angle);
            }
            return snap_to_vertex(arc, point);
        }

        /// <summary>
        /// Inspector - Finds the furthest vertex (arc boundary) from a input point.
        /// </summary>
        /// <param name="arc">The Arc whose boundaries will be checked.</param>
        /// <param name="point">A normalized point in 3D space.</param>
        /// <returns>Either arc.begin() or arc.end() depending on which is farther from the input point.</returns>
        public static Vector3 furthest_vertex(Arc arc, Vector3 point) // TODO: verify
        {
            return snap_to_vertex(arc, -point);
        }

        /// <summary>
        /// Inspector - Finds the furthest point (along the arc edge) from a input point.
        /// </summary>
        /// <param name="arc">The Arc whose edge will be checked.</param>
        /// <param name="point">A normalized point in 3D space.</param>
        /// <returns>The furthest point along the arc's edge from the input point.</returns>
        public static Vector3 furthest_point(Arc arc, Vector3 point) // TODO: verify
        {
            return snap_to_edge(arc, -point);
        }

        /// <summary>
        /// Inspector - Finds the point at "angle" extruded "extrusion" along "local_angle" direction.
        /// </summary>
        /// <param name="arc">The arc (used to determine positions and relative angles).</param>
        /// <param name="angle">The angle along the arc path. Range: [-arc.angle()/2, +arc.angle()/2]</param>
        /// <param name="local_angle">The secant angle relative to the arc at position("angle"). Range: [0, 2PI]</param>
        /// <param name="extrusion">The distance along "local_angle" to extrude.</param>
        /// <returns>The relative position after extruding the point at "angle" by "extrusion" along "local_angle".</returns>
        public static Vector3 relative_point(Arc arc, float angle, float local_angle, float extrusion)
        {
            Vector3 from = arc.position(angle);
            Vector3 local_direction = Bearing.bearing(arc.position(angle), arc.normal(angle), local_angle);
            Vector3 to = PlanetariaMath.spherical_linear_interpolation(from, local_direction, extrusion);
            return to;
        }
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