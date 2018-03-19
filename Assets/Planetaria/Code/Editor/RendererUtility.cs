using UnityEngine;

namespace Planetaria
{
    public static class RendererUtility
    {
        /// <summary>
        /// Inspector - Draws an arc on the surface of a unit sphere.
        /// </summary>
        /// <param name="arc">The arc that will be rendered.</param>
        /// <param name="color">The color of the drawn arc.</param>
        public static void draw_arc(Arc arc, float extrusion, Color color)
        {
            //if (LevelCreatorEditor.gizmos)
            //{
                GeospatialCircle circle = arc.circle(extrusion);
                Vector3 from = arc.position(0, extrusion);
                Vector3 normal = circle.center;
                Vector3 center = Vector3.Project(from, normal);
                normal *= -Mathf.Sign(circle.radius);
                float angle = arc.angle()*Mathf.Rad2Deg;
                float radius = (from - center).magnitude;

                UnityEditor.Handles.color = color;
                UnityEditor.Handles.DrawWireArc(center, normal, from - center, angle, radius);
            //}
        }

        /// <summary>
        /// Inspector - Draws an arc on the surface of a unit sphere.
        /// </summary>
        /// <param name="from">The starting point of the arc.</param>
        /// <param name="from_tangent">The rightward tangent/gradient/slope along the sphere at "from".</param>
        /// <param name="to">The ending point of the arc.</param>
        /// <param name="color">The color of the drawn arc.</param>
        public static void draw_arc(Vector3 from, Vector3 slope, Vector3 to, Color color)
        {
            Arc arc = Arc.curve(from, slope, to);

            draw_arc(arc, 0, color);
        }
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