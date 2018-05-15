using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    [CustomEditor(typeof(Arc))]
    public class ArcEditor : Editor
    {
        /// <summary>
        /// Inspector - Draw an arc (basic)
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="transformation">The Transform to rotate by.</param>
        public static void draw_arc(Arc arc,
                optional<Transform> transformation = new optional<Transform>())
        {
            Color violet = new Color(127, 0, 255);

            const float diameter = .1f;

            draw_arc(arc, 0.0f, Color.black, transformation);
            draw_arc(arc, diameter/2, Color.gray, transformation);
            draw_arc(arc, diameter, Color.white, transformation);

            draw_radial(arc, 0, diameter, Color.red, transformation);
            draw_radial(arc, arc.angle(), diameter, violet, transformation);
        }

        /// <summary>
        /// Inspector - Draw an arc (extruded by a radius)
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="extrusion">The distance to extrude the arc.</param>
        /// <param name="color">The color of the drawn arc.</param>
        /// <param name="transformation">The Transform to rotate by.</param>
        public static void draw_arc(Arc arc, float extrusion, Color color,
                optional<Transform> transformation = new optional<Transform>())
        {
            if (extrusion != 0 || arc.floor().offset < 1 - Precision.threshold) // for arcs larger than a single pixel
            {
                Arc rotated_arc = shifted_arc(arc, extrusion, transformation);
                RendererUtility.draw_arc(rotated_arc, 0, color);
            }
        }

        /// <summary>
        /// Inspector - Draw an extruded radial arc at a particular angle of the specified arc (i.e. the extrusion is its own arc).
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="angle">The angle at which the extruded radius is drawn.</param>
        /// <param name="extrusion">The distance to extrude the radial arc.</param>
        /// <param name="color">The color of the drawn radial arc.</param>
        /// <param name="transformation">The Transform to rotate by.</param>
        public static void draw_radial(Arc arc, float angle, float extrusion, Color color,
                optional<Transform> transformation = new optional<Transform>())
        {
            Vector3 from = arc.position(angle, 0);
            Vector3 to = arc.position(angle, extrusion);
            if (transformation.exists)
            {
                from = transformation.data.rotation * from;
                to = transformation.data.rotation * to;
            }
            Vector3 from_tangent = Vector3.ProjectOnPlane(to, from).normalized;
            RendererUtility.draw_arc(from, from_tangent, to, color);
        }

        /// <summary>
        /// Constructor - rotates arc so that it is relative to the input Transform.
        /// </summary>
        /// <param name="arc">The arc to be rotated.</param>
        /// <param name="transformation">The Transform to rotate by.</param>
        /// <returns>An arc rotated by Transform.</returns>
        public static Arc shifted_arc(Arc arc, float extrusion, optional<Transform> transformation = new optional<Transform>())
        {
            Vector3 from = arc.position(0, extrusion);
            Vector3 from_normal = arc.normal(0, extrusion);
            Vector3 from_tangent = Bearing.right(from, from_normal);
            Vector3 to = arc.position(arc.angle(), extrusion);

            if (transformation.exists)
            {
                from = transformation.data.rotation * from;
                from_tangent = transformation.data.rotation * from_tangent;
                to = transformation.data.rotation * to;
            }

            return Arc.curve(from, from_tangent, to);
        }

        // public static void draw_radial(Arc arc, float angle, float local_angle, float radius, Color color) // TODO: implement

        // public static void draw_tangent(Arc arc, float angle, float radius, Color color) // TODO: implement and add to draw_arc(Arc)
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