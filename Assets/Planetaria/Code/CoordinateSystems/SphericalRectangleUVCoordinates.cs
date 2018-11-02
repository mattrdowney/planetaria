using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct SphericalRectangleUVCoordinates // similar to picture here: https://math.stackexchange.com/questions/1205927/how-to-calculate-the-area-covered-by-any-spherical-rectangle
    {
        public Vector2 uv
        {
            get { return uv_variable; }
        }

        public Rect canvas
        {
            get { return canvas_variable; }
        }

        public bool valid()
        {
            return 0 <= uv.x && uv.x <= 1 &&
                    0 <= uv.y && uv.y <= 1;
        }

        /// <summary>
        /// Constructor - Stores the spherical rectangle's UV coordinates in a wrapper class. // FIXME: this should be really easy now: use TessellateTriangle using closest corner, closest axis, and the canvas center (and associated UV points)
        /// </summary>
        /// <param name="uv">The UV coordinates relative to the canvas center (0.5, 0.5). U/V Range: (-INF, +INF).</param> // FIXME: TessellateTriangle assumes the point is contained in the Barycentric coordinates
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        public SphericalRectangleUVCoordinates(Vector2 uv, Rect canvas)
        {
            uv_variable = uv;
            canvas_variable = canvas;
        }

        /// <summary>
        /// Inspector - Converts spherical rectangle UV coordinates into normalized cartesian coordinates.
        /// </summary>
        /// <param name="uv">The coordinates in spherical rectangle UV space that will be converted.</param>
        /// <returns>The normalized cartesian coordinates.</returns>
        public static implicit operator NormalizedCartesianCoordinates(SphericalRectangleUVCoordinates uv)
        {
            return spherical_rectangle_to_cartesian(uv.uv_variable, uv.canvas_variable);
        }

        /// <summary>
        /// Inspector - Creates a spherical rectangle UV coordinate set from a point on a unit sphere and a rectangle representing the x/y angles.
        /// </summary>
        /// <param name="cartesian">The point on the surface of a unit sphere to be converted.</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>UV Coordinates for a spherical rectangle. Valid X/Y Range: [0, 1], although one axis may be in the (-INF, +INF) range.</returns>
        public static SphericalRectangleUVCoordinates cartesian_to_spherical_rectangle(Vector3 cartesian, Rect canvas)
        {
            // Find closest axis (4 possibilities)
            // Find closest corner (x2 possibilities)
            // Fetch arc from axis -> corner (this could be done in 2 arc intersection checks using a lazy algorithm and closest result - likely choice)

            // Find center point attractor (i.e. Bearing.attractor(canvas_center, cartesian))
            // Project attractor onto arc connecting axis and corner (intersection of the two)
            
            // Find ratio of projected point to total arc length
            // Find ratio of point to projected point relative to canvas_center

            // Map the 8 arcs to the boarder of the UV square (2 per side)
            // Linearly interpolate square to get boarder-projected point (radial interpolation is a bad idea I think)
            // Interpolate from the UV (0.5, 0.5) (i.e. canvas_center) to the boarder-projected point using the center-relative ratio.
            // That should be the final UV.
        }

        /// <summary>
        /// Inspector - Creates a cartesian point on the surface of a sphere coordinate set from a uv coordinate and a rectangle.
        /// </summary>
        /// <param name="uv">UV Coordinates for a spherical rectangle. Valid X/Y Range: [0, 1], although one axis may be in the (-INF, +INF) range.</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>The cartesian point on the surface of a unit sphere the uv represents.</returns>
        public static SphericalRectangleUVCoordinates spherical_rectangle_to_cartesian(Vector2 uv, Rect canvas)
        {
            // Take UV
            // Calculate point intersection with square boarder.
            // Find ratio of point relative to boarder-projected point relative to UV center (i.e. (0.5, 0.5)).
            // Find ratio of intersected point along square boarder axis -> center.
            
            // Find 8ths sector arc (should be a lookup table based on UV coordinate).
            // Interpolate along arc based on UV square boarder ratio.
            // Interpolate from canvas_center to arc-projected point based on center ratio.
            // That should be the final cartesian point.
        }

        /// <summary>
        /// Inspector (Cache mutator) - Updates the cache so that spherical rectangle calculations avoid recomputing old values.
        /// </summary>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        public static void cache_spherical_rectangle(Rect canvas)
        {
            Debug.LogError("Not Implemented"); // FIXME:
        }

        private Vector3 closest_axis(float angular_width, float angular_height) // FIXME: Rect canvas
        {
            Arc equator = ArcFactory.curve(Vector3.forward, Vector3.right, Vector3.forward);
            Arc meridian = ArcFactory.curve(Vector3.forward, Vector3.up, Vector3.forward);
            float x_fraction = Mathf.Abs(uv.x)/angular_width;
            float y_fraction = Mathf.Abs(uv.y)/angular_height;
            if (x_fraction > y_fraction)
            {
                return equator.position(Mathf.Sign(uv.x)*angular_width/2);
            }
            return meridian.position(Mathf.Sign(uv.y)*angular_height/2);
        }

        private Vector3 closest_diagonal(float angular_width, float angular_height) // FIXME: Rect canvas
        {
            Arc equator = ArcFactory.curve(Vector3.forward, Vector3.right, Vector3.forward);
            Arc meridian = ArcFactory.curve(Vector3.forward, Vector3.up, Vector3.forward);
            Arc x_boundary = ArcFactory.curve(Vector3.down, equator.position(Mathf.Sign(uv.x)*angular_width/2), Vector3.up);
            Arc y_boundary = ArcFactory.curve(Vector3.left, meridian.position(Mathf.Sign(uv.y)*angular_height/2), Vector3.right);
            Vector3 corner = PlanetariaIntersection.arc_arc_intersection(x_boundary, y_boundary, 0).data;
            return corner;
        }

        // CONSIDER: re-add normalize() for scaling width/height?

        [SerializeField] private Vector2 uv_variable; // FIXME: UVCoordinates would be normalized - TODO: should probably refactor UVCoordinates
        [SerializeField] private Rect canvas_variable;

        // private static // FIXME: CACHE: canvas_center, upper_arc, lower_arc, left_arc, right_arc
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