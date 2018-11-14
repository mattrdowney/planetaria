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
        /// Inspector (Cache Mutator) - Creates a spherical rectangle UV coordinate set from a point on a unit sphere and a rectangle representing the x/y angles.
        /// </summary>
        /// <param name="cartesian">The point on the surface of a unit sphere to be converted.</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>UV Coordinates for a spherical rectangle. Valid X/Y Range: [0, 1], although one axis may be in the (-INF, +INF) range.</returns>
        public static SphericalRectangleUVCoordinates cartesian_to_spherical_rectangle(Vector3 cartesian, Rect canvas)
        {
            // cache the canvas center point, and the direction of "right" and "up" relative to this center point (for UV calculations)
            cache_spherical_rectangle(canvas);
            
            // project cartesian point onto a local "equator" and meridian
            Vector3 horizontal_projection = PlanetariaMath.project_onto_equator(cartesian, cached_up); // it makes sense this doesn't work, yet I can't figure out what it should be instead.
            Vector3 vertical_projection = PlanetariaMath.project_onto_equator(cartesian, cached_right);

            float relative_longitude = Vector3.SignedAngle(cached_center, horizontal_projection, cached_up) * Mathf.Deg2Rad; // angle along a "equator" (might not be the real equator)
            float relative_latitude = -Vector3.SignedAngle(cached_center, vertical_projection, cached_right) * Mathf.Deg2Rad; // angle along a meridian (might not be the great meridian)

            Vector2 absolute_longitude_latitude = new Vector2(canvas.center.x + relative_longitude, canvas.center.y + relative_latitude);
            Vector2 uv = (absolute_longitude_latitude - canvas.min)/canvas.size;
            return new SphericalRectangleUVCoordinates(uv, canvas);
        }

        /// <summary>
        /// Inspector - Creates a cartesian point on the surface of a sphere coordinate set from a uv coordinate and a rectangle.
        /// </summary>
        /// <param name="uv">UV Coordinates for a spherical rectangle. Valid X/Y Range: [0, 1], although one axis may be in the (-INF, +INF) range.</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>The cartesian point on the surface of a unit sphere the uv represents.</returns>
        public static NormalizedCartesianCoordinates spherical_rectangle_to_cartesian(Vector2 uv, Rect canvas) // I have been pretty certain since the beginning that this is the ideal function, but I cannot figure out its inverse for the life of me
        {
            Vector2 longitude_latitude = canvas.min + Vector2.Scale(canvas.size, uv);
            return new NormalizedCartesianCoordinates(intersection(longitude_latitude.x, longitude_latitude.y));
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
                cached_right = Bearing.attractor(cached_center, intersection(canvas.xMax, canvas.center.y));
                cached_up = Bearing.attractor(cached_center, intersection(canvas.center.x, canvas.yMax));
                cached_canvas = canvas;
            }
        }

        private static Vector3 equator_longitude(float longitude)
        {
            Arc equator = ArcFactory.curve(Vector3.back, Vector3.left, Vector3.back); // The center of the arc is between begin and endpoint hence Vector3.back (not Vector3.forward) and Vector3.left (not Vector3.right)
            return equator.position(longitude);
        }

        private static Vector3 prime_meridian_latitude(float latitude)
        {
            Arc prime_meridian = ArcFactory.curve(Vector3.back, Vector3.down, Vector3.back); // same as equator_longitude comment
            return prime_meridian.position(latitude);
        }

        private static Vector3 intersection(float longitude, float latitude) // FIXME: Rect canvas
        {
            Arc x_boundary = ArcFactory.curve(Vector3.down, equator_longitude(longitude), Vector3.up); // full-circle
            Arc y_boundary = ArcFactory.curve(Vector3.left, prime_meridian_latitude(latitude), Vector3.left); // semi-circle
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

        // cache (to avoid recomputation every frame)
        private static Rect cached_canvas = new Rect(float.NaN, float.NaN, float.NaN, float.NaN); // will never equal any Rect (so data will always be re-cached)
        private static Vector3 cached_center = Vector3.forward;
        private static Vector3 cached_right = Vector3.right;
        private static Vector3 cached_up = Vector3.up;
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