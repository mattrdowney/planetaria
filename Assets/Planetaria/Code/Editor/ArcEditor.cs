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
        public static void draw_arc(Arc arc)
        {
            Color violet = new Color(127, 0, 255);

            const float diameter = .1f;

            draw_arc(arc, 0.0f, Color.black);
            draw_arc(arc, diameter/2, Color.gray);
            draw_arc(arc, diameter, Color.white);

            draw_radial(arc, 0, diameter, Color.red);
            draw_radial(arc, arc.angle(), diameter, violet);
        }

        /// <summary>
        /// Inspector - Draw an arc (extruded by a radius)
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="extrusion">The distance to extrude the arc.</param>
        /// <param name="color">The color of the drawn arc.</param>
        public static void draw_arc(Arc arc, float extrusion, Color color)
        {
            RendererUtility.draw_arc(arc, extrusion, color);
        }

        /// <summary>
        /// Inspector - Draw an extruded radial arc at a particular angle of the specified arc (i.e. the extrusion is its own arc).
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="angle">The angle at which the extruded radius is drawn.</param>
        /// <param name="radius">The radius to extrude the arc.</param>
        /// <param name="color">The color of the drawn radial arc.</param>
        public static void draw_radial(Arc arc, float angle, float radius, Color color)
        {
            Vector3 from = arc.position(angle, 0);
            Vector3 to = arc.position(angle, radius);
            Vector3 from_tangent = Vector3.ProjectOnPlane(to, from).normalized;
            RendererUtility.draw_arc(from, from_tangent, to, color);
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