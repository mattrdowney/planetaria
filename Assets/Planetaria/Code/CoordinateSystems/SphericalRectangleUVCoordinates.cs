using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct SphericalRectangleUVCoordinates // similar to picture here: https://math.stackexchange.com/questions/1205927/how-to-calculate-the-area-covered-by-any-spherical-rectangle
    {
        // NOTE: I forgot to mention that (perhaps) the ideal solution for this is spherical Barycentric coordinates (quadrilateral, not triangle), since it is extensible and reusable and almost certainly exact
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
            // cache the canvas and derivative arcs (so they are not recomputed every time the function is called)
            cache_spherical_rectangle(canvas);

            Vector3 equator_projection = PlanetariaMath.project_onto_equator(cartesian, cached_north_hemisphere);
            Vector3 prime_meridian_projection = PlanetariaMath.project_onto_equator(cartesian, cached_east_hemisphere);

            float u; // FIXME: keep it DRY (Do not Repeat Yourself)
            if (Vector3.Dot(cartesian, cached_east_hemisphere) >= 0) // right (East)
            {
                float angle;
                if (Vector3.Dot(cartesian, cached_right_positive_partition) >= 0)
                {
                    angle = Vector3.Angle(cached_right_biangle_focus, equator_projection) * Mathf.Deg2Rad;
                }
                else
                {
                    angle = Vector3.Angle(-cached_right_biangle_focus, equator_projection) * Mathf.Deg2Rad + Mathf.PI;
                }
                angle -= cached_right_start_angle;
                u = angle / (cached_right_end_angle - cached_right_start_angle);
                u = 0.5f + u/2;
            }
            else // if (Vector3.Dot(cartesian, cached_east_hemisphere) < 0) // left (West)
            {
                float angle;
                if (Vector3.Dot(cartesian, cached_left_positive_partition) >= 0)
                {
                    angle = Vector3.Angle(cached_left_biangle_focus, equator_projection) * Mathf.Deg2Rad;
                }
                else
                {
                    angle = Vector3.Angle(-cached_left_biangle_focus, equator_projection) * Mathf.Deg2Rad + Mathf.PI;
                }
                angle -= cached_left_start_angle;
                u = angle / (cached_left_end_angle - cached_left_start_angle);
                u = 0.5f - u/2;
            }

            float v;
            if (Vector3.Dot(cartesian, cached_north_hemisphere) >= 0) // upper (North)
            {
                float angle;
                if (Vector3.Dot(cartesian, cached_upper_positive_partition) >= 0)
                {
                    angle = Vector3.Angle(cached_upper_biangle_focus, prime_meridian_projection) * Mathf.Deg2Rad;
                }
                else
                {
                    angle = Vector3.Angle(-cached_upper_biangle_focus, prime_meridian_projection) * Mathf.Deg2Rad + Mathf.PI;
                }
                angle -= cached_upper_start_angle;
                v = angle / (cached_upper_end_angle - cached_upper_start_angle);
                v = 0.5f + v/2;
            }
            else // if (Vector3.Dot(cartesian, cached_north_hemisphere) < 0) // lower (South)
            {
                float angle;
                if (Vector3.Dot(cartesian, cached_lower_positive_partition) >= 0)
                {
                    angle = Vector3.Angle(cached_lower_biangle_focus, prime_meridian_projection) * Mathf.Deg2Rad;
                }
                else
                {
                    angle = Vector3.Angle(-cached_lower_biangle_focus, prime_meridian_projection) * Mathf.Deg2Rad + Mathf.PI;
                }
                angle -= cached_lower_start_angle;
                v = angle / (cached_lower_end_angle - cached_lower_start_angle);
                v = 0.5f - v/2;
            }

            return new SphericalRectangleUVCoordinates(new Vector2(u, v), canvas);
        }

        /// <summary>
        /// Inspector - Creates a cartesian point on the surface of a sphere coordinate set from a uv coordinate and a rectangle.
        /// </summary>
        /// <param name="uv">UV Coordinates for a spherical rectangle. Valid X/Y Range: [0-, 1+].</param>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        /// <returns>The cartesian point on the surface of a unit sphere the uv represents.</returns>
        public static NormalizedCartesianCoordinates spherical_rectangle_to_cartesian(Vector2 uv, Rect canvas)
        {
            Vector2 longitude_latitude = canvas.min + Vector2.Scale(canvas.size, uv); // it's possible the issue here was with uniform UV coverage, but I'll try it.
            return new NormalizedCartesianCoordinates(intersection(longitude_latitude.x, longitude_latitude.y));
        }

        
        /// <summary>
        /// Inspector (Cache Mutator) - Updates the cache so that spherical rectangle calculations avoid recomputing old values.
        /// </summary>
        /// <param name="canvas">A Rect (measuring radians) representing the start and stop angles relative to Quaternion.identity. X/Y Range: (-2PI, +2PI).</param>
        public static void cache_spherical_rectangle(Rect canvas)
        {
            if (cached_canvas != canvas)
            {
                Vector3 lower_left = intersection(canvas.xMin, canvas.yMin);
                Vector3 lower_center = intersection(canvas.center.x, canvas.yMin);
                Vector3 lower_right = intersection(canvas.xMax, canvas.yMin);

                Vector3 middle_left = intersection(canvas.xMin, canvas.center.y);
                Vector3 middle_center = intersection(canvas.center.x, canvas.center.y);
                Vector3 middle_right = intersection(canvas.xMax, canvas.center.y);

                Vector3 upper_left = intersection(canvas.xMin, canvas.yMax);
                Vector3 upper_center = intersection(canvas.center.x, canvas.yMax);
                Vector3 upper_right = intersection(canvas.xMax, canvas.yMax);

                Arc biangle_segment1 = ArcFactory.curve(upper_center, upper_right, -upper_center);
                Arc biangle_segment2 = ArcFactory.curve(lower_center, lower_right, -lower_center);
                cached_left_biangle_focus = PlanetariaIntersection.arc_arc_intersection(biangle_segment1, biangle_segment2, 0).data;
                cached_left_positive_partition = Bearing.attractor(cached_left_biangle_focus, middle_left); // used for a dot product to determine if the angle applied for UV is +/-
                if (Vector3.Dot(cached_left_positive_partition, middle_center) >= 0)
                {
                    cached_left_start_angle = Vector3.Angle(cached_left_biangle_focus, middle_center) * Mathf.Deg2Rad;
                    cached_left_end_angle = Vector3.Angle(cached_left_biangle_focus, middle_left) * Mathf.Deg2Rad;
                }
                else
                {
                    cached_left_start_angle = Vector3.Angle(-cached_left_biangle_focus, middle_center) * Mathf.Deg2Rad + Mathf.PI;
                    cached_left_end_angle = Vector3.Angle(-cached_left_biangle_focus, middle_left) * Mathf.Deg2Rad + Mathf.PI;
                }

                biangle_segment1 = ArcFactory.curve(upper_center, upper_left, -upper_center);
                biangle_segment2 = ArcFactory.curve(lower_center, lower_left, -lower_center);
                cached_right_biangle_focus = PlanetariaIntersection.arc_arc_intersection(biangle_segment1, biangle_segment2, 0).data;
                cached_right_positive_partition = Bearing.attractor(cached_right_biangle_focus, middle_right); // used for a dot product to determine if the angle applied for UV is +/-
                if (Vector3.Dot(cached_right_positive_partition, middle_center) >= 0)
                {
                    cached_right_start_angle = Vector3.Angle(cached_right_biangle_focus, middle_center) * Mathf.Deg2Rad;
                    cached_right_end_angle = Vector3.Angle(cached_right_biangle_focus, middle_right) * Mathf.Deg2Rad;
                }
                else
                {
                    cached_right_start_angle = Vector3.Angle(-cached_right_biangle_focus, middle_center) * Mathf.Deg2Rad + Mathf.PI;
                    cached_right_end_angle = Vector3.Angle(-cached_right_biangle_focus, middle_right) * Mathf.Deg2Rad + Mathf.PI;
                }

                biangle_segment1 = ArcFactory.curve(middle_left, upper_left, -middle_left);
                biangle_segment2 = ArcFactory.curve(middle_right, upper_right, -middle_right);
                cached_lower_biangle_focus = PlanetariaIntersection.arc_arc_intersection(biangle_segment1, biangle_segment2, 0).data;
                cached_lower_positive_partition = Bearing.attractor(cached_lower_biangle_focus, lower_center); // used for a dot product to determine if the angle applied for UV is +/-
                if (Vector3.Dot(cached_lower_positive_partition, middle_center) >= 0)
                {
                    cached_lower_start_angle = Vector3.Angle(cached_lower_biangle_focus, middle_center) * Mathf.Deg2Rad;
                    cached_lower_end_angle = Vector3.Angle(cached_lower_biangle_focus, lower_center) * Mathf.Deg2Rad;
                }
                else
                {
                    cached_lower_start_angle = Vector3.Angle(-cached_lower_biangle_focus, middle_center) * Mathf.Deg2Rad + Mathf.PI;
                    cached_lower_end_angle = Vector3.Angle(-cached_lower_biangle_focus, lower_center) * Mathf.Deg2Rad + Mathf.PI;
                }

                biangle_segment1 = ArcFactory.curve(middle_left, lower_left, -middle_left);
                biangle_segment2 = ArcFactory.curve(middle_right, lower_right, -middle_right);
                cached_upper_biangle_focus = PlanetariaIntersection.arc_arc_intersection(biangle_segment1, biangle_segment2, 0).data;
                cached_upper_positive_partition = Bearing.attractor(cached_upper_biangle_focus, upper_center); // used for a dot product to determine if the angle applied for UV is +/-
                if (Vector3.Dot(cached_upper_positive_partition, middle_center) >= 0)
                {
                    cached_upper_start_angle = Vector3.Angle(cached_upper_biangle_focus, middle_center) * Mathf.Deg2Rad;
                    cached_upper_end_angle = Vector3.Angle(cached_upper_biangle_focus, upper_center) * Mathf.Deg2Rad;
                }
                else
                {
                    cached_upper_start_angle = Vector3.Angle(-cached_upper_biangle_focus, middle_center) * Mathf.Deg2Rad + Mathf.PI;
                    cached_upper_end_angle = Vector3.Angle(-cached_upper_biangle_focus, upper_center) * Mathf.Deg2Rad + Mathf.PI;
                }

                cached_north_hemisphere = Bearing.attractor(middle_center, upper_center);
                cached_east_hemisphere = Bearing.attractor(middle_center, middle_right);

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
        private static Vector3 cached_north_hemisphere; // this should be relative to canvas center for new version (can't tell if it's a bug that it wasn't relative to middle rail.
        private static Vector3 cached_east_hemisphere;

        private static Vector3 cached_left_biangle_focus; // possibility 1: array[4], possibility 2: struct of data (or both)
        private static Vector3 cached_right_biangle_focus;
        private static Vector3 cached_lower_biangle_focus;
        private static Vector3 cached_upper_biangle_focus;
        private static Vector3 cached_left_positive_partition;
        private static Vector3 cached_right_positive_partition;
        private static Vector3 cached_lower_positive_partition;
        private static Vector3 cached_upper_positive_partition;
        private static float cached_left_start_angle;
        private static float cached_right_start_angle;
        private static float cached_lower_start_angle;
        private static float cached_upper_start_angle;
        private static float cached_left_end_angle;
        private static float cached_right_end_angle;
        private static float cached_lower_end_angle;
        private static float cached_upper_end_angle;
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