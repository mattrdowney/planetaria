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

        public bool valid() // instead, create a UV clamp/wrap mechanism based on an input parameter e.g. UVMode.Clamp, UVMode.Wrap
        {
            return 0 <= uv.x && uv.x <= 1 &&
                    0 <= uv.y && uv.y <= 1;
        }

        /// <summary>
        /// Constructor - Stores the spherical rectangle's UV coordinates in a wrapper class.
        /// </summary>
        /// <param name="uv">The UV coordinates relative to the canvas center (0.5, 0.5). U/V Range: (-INF, +INF).</param>
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
        /// Inspector (Cache mutator) - Creates a spherical rectangle UV coordinate set from a point on a unit sphere and a rectangle representing the x/y angles.
        /// </summary>
        /// <param name="cartesian">The point on the surface of a unit sphere to be converted.</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>UV Coordinates for a spherical rectangle. Valid X/Y Range: [0, 1], although one axis may be in the (-INF, +INF) range.</returns>
        public static SphericalRectangleUVCoordinates cartesian_to_spherical_rectangle(Vector3 cartesian, Rect canvas)
        {
            cache_spherical_rectangle(canvas); // ensure correct canvas is cached
            
            foreach (Arc arc in cached_arcs) // visualize
            {
                ArcEditor.draw_simple_arc(arc);
            }

            // Find center point attractor (i.e. the direction of the pixel relative to the canvas)
            Vector3 direction = Bearing.attractor(cached_center, cartesian); 
            Arc path = ArcFactory.curve(cached_center, direction, -cached_center);

            // Find nearest spherical rectangle Arc edge (either upper, lower, left, or right boundary)
            cache_closest_arc(path);

            // Find arc leading from canvas center along attractor towards arc boundary intersection point.
            path = ArcFactory.line(cached_center, cached_closest_intersection);

            // Find ratio of projected point to total arc length
            float border_interpolator = cached_closest_arc.position_to_angle(cached_closest_intersection)/cached_closest_arc.angle() + 0.5f;
            // Find ratio of point to projected point relative to canvas_center       
            float center_interpolator = path.position_to_angle(cartesian)/path.angle() + 0.5f;
            
            Vector2 square_secant_point; // position on UV square
            // Map the 4 arcs to the boarder of the UV square
            if (cached_boundary <= SquareEdge.LowerBoundary) // upper/lower side of UV square (right-side-up hourglass portion)
            {
                // Linearly interpolate square to get boarder-projected point
                square_secant_point = new Vector2(border_interpolator, cached_boundary == SquareEdge.LowerBoundary ? 0 : 1);
            }
            else // left/right side of UV square (tipped-over hourglass portion)
            {
                // Linearly interpolate square to get boarder-projected point
                square_secant_point = new Vector2(cached_boundary == SquareEdge.LeftBoundary ? 0 : 1, border_interpolator);
            }
            // Interpolate from the UV (0.5, 0.5) (i.e. canvas_center) to the boarder-projected point using the center-relative ratio.
            Vector2 uv_point = Vector3.Lerp(new Vector3(0.5f, 0.5f), square_secant_point, center_interpolator);
            return new SphericalRectangleUVCoordinates(uv_point, canvas);
        }

        /// <summary>
        /// Inspector (Cache mutator) - Creates a cartesian point on the surface of a sphere coordinate set from a uv coordinate and a rectangle.
        /// </summary>
        /// <param name="uv">UV Coordinates for a spherical rectangle. Valid X/Y Range: [0, 1], although one axis may be in the (-INF, +INF) range.</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>The cartesian point on the surface of a unit sphere the uv represents.</returns>
        public static NormalizedCartesianCoordinates spherical_rectangle_to_cartesian(Vector2 uv, Rect canvas)
        {
            cache_spherical_rectangle(canvas); // ensure correct canvas is cached
            
            // expand UV coordinates [0,1] to [-1, +1]
            Vector2 xy = (uv - new Vector2(0.5f, 0.5f))*2;
            Vector2 square_secant_point;
            float border_interpolator;

            // find UV square boarder
            if (Mathf.Abs(xy.x) <= Mathf.Abs(xy.y)) // upper/lower side of UV square (right-side-up hourglass portion)
            {
                cached_boundary = xy.y >= 0 ? SquareEdge.UpperBoundary : SquareEdge.LowerBoundary;
                // find UV square boundary
                square_secant_point = xy / Mathf.Abs(xy.y); // project onto upper/lower edge of UV square
                // Find ratio of intersected point along square boarder
                border_interpolator = uv.y;
            }
            else // left/right side of UV square (tipped-over hourglass portion)
            {
                cached_boundary = xy.x >= 0 ? SquareEdge.RightBoundary : SquareEdge.LeftBoundary;
                // find UV square boundary
                square_secant_point = xy / Mathf.Abs(xy.x); // project onto left/right edge of UV square
                // Find ratio of intersected point along square boarder
                border_interpolator = uv.x;
            }

            // Find ratio of point relative to boarder-projected point relative to UV center (i.e. (0.5, 0.5)).
            float center_interpolator = xy.magnitude / square_secant_point.magnitude;
            
            // Find 4ths arc based on UV coordinate sector (lookup table).
            cached_closest_arc = cached_arcs[(int)cached_boundary];

            // Interpolate along arc based on UV square boarder ratio.
            cached_closest_intersection = cached_closest_arc.position(Mathf.Lerp(-cached_closest_arc.half_angle, +cached_closest_arc.half_angle, border_interpolator));

            // Interpolate from canvas_center to arc-projected point based on center ratio.
            Arc path = ArcFactory.line(cached_center, cached_closest_intersection);
            Vector3 cartesian = path.position(Mathf.Lerp(-path.half_angle, +path.half_angle, center_interpolator));

            // That should be the final cartesian point.
            return new NormalizedCartesianCoordinates(cartesian);
        }

        /// <summary>
        /// Inspector (Cache mutator) - Updates the cache so that spherical rectangle calculations avoid recomputing old values.
        /// </summary>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        public static void cache_spherical_rectangle(Rect canvas)
        {
            if (cached_canvas != canvas)
            {
                cached_center = intersection(canvas.center.x, canvas.center.y);
                cached_arcs = new Arc[4];
                cached_arcs[(int)SquareEdge.UpperBoundary] = boundary(new Vector2(canvas.xMin, canvas.yMax), new Vector2(canvas.xMax, canvas.yMax));
                cached_arcs[(int)SquareEdge.LowerBoundary] = boundary(new Vector2(canvas.xMin, canvas.yMin), new Vector2(canvas.xMax, canvas.yMin));
                cached_arcs[(int)SquareEdge.LeftBoundary] = boundary(new Vector2(canvas.xMin, canvas.yMin), new Vector2(canvas.xMin, canvas.yMax));
                cached_arcs[(int)SquareEdge.RightBoundary] = boundary(new Vector2(canvas.xMax, canvas.yMin), new Vector2(canvas.xMax, canvas.yMax));
                cached_canvas = canvas;
            }
        }

        /// <summary>
        /// Inspector (Cache mutator) - Finds the closest spherical rectangle arc boundary.
        /// </summary>
        private static void cache_closest_arc(Arc path) // CONSIDER: This can be sped up, but it's probably not worth it.
        {
            cached_closest_arc = cached_arcs[0];
            cached_closest_intersection = -cached_center;
            float closest_distance = 180f;
            for (int arc = 0; arc < cached_arcs.Length; ++arc)
            {
                optional<Vector3> current_intersection = PlanetariaIntersection.arc_arc_intersection(path, cached_arcs[arc], 0);
                if (current_intersection.exists)
                {
                    float current_distance = Vector3.Angle(cached_center, current_intersection.data) * Mathf.Deg2Rad;
                    if (current_distance < closest_distance)
                    {
                        cached_closest_arc = cached_arcs[arc];
                        cached_closest_intersection = current_intersection.data;
                        cached_boundary = (SquareEdge) arc; // Find SquareEdge.UpperBoundary/LeftBoundary/etc
                        closest_distance = current_distance;
                    }
                }
            }
        }

        private static Vector3 equator_longitude(float longitude)
        {
            Arc equator = ArcFactory.curve(Vector3.forward, Vector3.right, Vector3.forward);
            return equator.position(longitude);
        }

        private static Vector3 prime_meridian_latitude(float latitude)
        {
            Arc prime_meridian = ArcFactory.curve(Vector3.forward, Vector3.up, Vector3.forward);
            return prime_meridian.position(latitude);
        }

        private static Vector3 intersection(float longitude, float latitude) // FIXME: Rect canvas
        {
            Arc x_boundary = ArcFactory.curve(Vector3.down, equator_longitude(longitude), Vector3.down); // full-circle
            Arc y_boundary = ArcFactory.curve(Vector3.left, prime_meridian_latitude(latitude), Vector3.right); // semi-circle
            Vector3 corner = PlanetariaIntersection.arc_arc_intersection(x_boundary, y_boundary, 0).data;
            return corner;
        }

        private static Arc boundary(Vector2 start_longitude_latitude, Vector2 end_longitude_latitude)
        {
            Vector3 first_corner = intersection(start_longitude_latitude.x, start_longitude_latitude.y);
            Vector3 end_corner = intersection(end_longitude_latitude.x, end_longitude_latitude.y);
            Vector2 middle_longitude_latitude = (start_longitude_latitude + end_longitude_latitude)/2;
            Vector3 axis = intersection(middle_longitude_latitude.x, middle_longitude_latitude.y);
            return ArcFactory.curve(first_corner, axis, end_corner);
        }

        // CONSIDER: re-add normalize() for scaling width/height? Clamp vs Wrap

        [SerializeField] private Vector2 uv_variable; // FIXME: UVCoordinates would be normalized - TODO: should probably refactor UVCoordinates
        [SerializeField] private Rect canvas_variable;

        private enum SquareEdge { UpperBoundary = 0, LowerBoundary = 1, LeftBoundary = 2, RightBoundary = 3 }

        // Rect cache
        private static Rect cached_canvas; // FIXME: first iteration will have uninitialized cache - to be fair this is undefined behaviour anyway
        private static Vector3 cached_center = Vector3.forward;
        private static Arc[] cached_arcs;

        // point cache
        private static Arc cached_closest_arc;
        private static Vector3 cached_closest_intersection;
        private static SquareEdge cached_boundary;
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