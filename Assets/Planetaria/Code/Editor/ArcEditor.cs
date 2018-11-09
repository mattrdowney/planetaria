using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public class ArcEditor : Editor
    {
        /// <summary>
        /// Inspector - Draw an arc (basic)
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="orientation">The Transform's rotation (for moving platforms). For static objects, use Quaternion.identity.</param>
        public static void draw_arc(Arc arc, Quaternion orientation)
        {
            float diameter = .1f; // FIXME: magic number
            diameter = (arc.type == ArcType.ConcaveCorner ? -diameter : diameter);

            // draw vertical lines representing seams between arcs
            draw_ray(arc, -arc.angle()/2, Mathf.PI/2, diameter, Color.gray, orientation);

            draw_arc(arc, 0, Color.white, orientation); // draw white floor
            draw_arc(arc, diameter/2, Color.gray, orientation); // draw gray midline
            draw_arc(arc, diameter, Color.black, orientation); // draw black ceiling
        }

        /// <summary>
        /// Inspector - Draw an arc (extruded by a radius)
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="extrusion">The distance to extrude the arc.</param>
        /// <param name="color">The color of the drawn arc.</param>
        /// <param name="orientation">The Transform's rotation (for moving platforms). For static objects, use Quaternion.identity.</param>
        private static void draw_arc(Arc arc, float extrusion, Color color, Quaternion orientation)
        {
            Vector3 from = arc.begin(extrusion);
            Vector3 normal = -arc.floor().normal;
            normal = arc.type == ArcType.ConcaveCorner ? -normal : normal;
            Vector3 to = arc.end(extrusion);

            if (orientation != Quaternion.identity)
            {
                from = orientation * from;
                normal = orientation * normal;
                to = orientation * to;
            }

            Vector3 center = Vector3.Project(from, normal);
            float angle = arc.angle()*Mathf.Rad2Deg;
            float radius = (from - center).magnitude;

            Handles.color = color;
            Handles.DrawWireArc(center, normal, from - center, angle, radius);
        }

        /// <summary>
        /// Inspector - Draw an extruded radial arc at a particular angle of the specified arc (i.e. the extrusion is its own arc).
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="angle">The angle along the original Arc at which the extruded radius is drawn.</param>
        /// <param name="local_angle">The angle at which the ray is "refracting" from the surface. 0=>right; PI/2=>up.</param>
        /// <param name="extrusion">The distance (radius) to extrude the radial arc.</param>
        /// <param name="color">The color of the drawn radial arc.</param>
        /// <param name="orientation">The Transform's rotation (for moving platforms). For static objects, use Quaternion.identity.</param>
        public static void draw_ray(Arc arc, float angle, float local_angle, float extrusion, Color color, Quaternion orientation)
        {
            Vector3 from = arc.position(angle);
            Vector3 to = ArcUtility.relative_point(arc, angle, local_angle, extrusion);

            Arc composite = ArcFactory.line(from, to);
            draw_arc(composite, 0.0f, color, orientation);
        }

        /// <summary>
        /// Inspector - Draw an arc with lines only (basic)
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        public static void draw_simple_arc(Arc arc)
        {
            float start_angle = -arc.half_angle;

            int segments = 50;
            for (int segment = 1; segment <= segments; ++segment)
            {
                float end_angle = -arc.half_angle + segment/(float)segments*arc.angle();
                Debug.DrawLine(arc.position(start_angle), arc.position(end_angle), Color.yellow, 5f);
                start_angle = end_angle;
            }
        }

        /// <summary>
        /// Inspector - Draw a fraction of the given arc segment.
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="begin_angle">The angle (along the arc) at which the partial arc begins.</param>
        /// <param name="end_angle">The angle (along the arc) at which the partial arc ends.</param>
        /// <param name="extrusion">The distance (radius) to extrude the radial arc.</param>
        /// <param name="color">The color of the drawn radial arc.</param>
        /// <param name="orientation">The Transform's rotation (for moving platforms). For static objects, use Quaternion.identity.</param>
        public static void draw_partial_arc(Arc arc, float begin_angle, float end_angle, float extrusion, Color color, Quaternion orientation)
        {
            if (arc.curvature >= ArcType.ConvexCorner && extrusion == 0)
            {
                return;
            }
            Vector3 from = arc.position(begin_angle, extrusion);
            Vector3 to = arc.position(end_angle, extrusion);

            Arc composite = ArcFactory.line(from, to);
            draw_arc(composite, 0.0f, color, orientation);
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