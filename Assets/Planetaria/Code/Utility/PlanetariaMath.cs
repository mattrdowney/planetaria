using UnityEngine;

namespace Planetaria
{
    public static class PlanetariaMath
    {
        public static Vector3 barycentric_coordinates(Vector3 coordinate, Vector3 vertex_a, Vector3 vertex_b, Vector3 vertex_c) // FIXME: ideally remove this altogether, otherwise make elegant
        {
            Vector3 left_edge = vertex_b - vertex_a;
            Vector3 right_edge = vertex_c - vertex_a;
            Vector3 relative_coordinate = coordinate - vertex_a;

            float left_edge_length_squared = Vector3.Dot(left_edge, left_edge);
            float right_edge_length_squared = Vector3.Dot(right_edge, right_edge);
            float edge_similarity = Vector3.Dot(left_edge, right_edge);
            float left_edge_component = Vector3.Dot(relative_coordinate, left_edge);
            float right_edge_component = Vector3.Dot(relative_coordinate, right_edge);
            float denominator = left_edge_length_squared * right_edge_length_squared - edge_similarity * edge_similarity;

            Vector3 uvw = Vector3.zero;
            uvw[1] = (right_edge_length_squared * left_edge_component - edge_similarity * right_edge_component) / denominator;
            uvw[2] = (left_edge_length_squared * right_edge_component - edge_similarity * left_edge_component) / denominator;
            uvw[0] = 1 - uvw[1] - uvw[2];

            return uvw;
        }

        /// <summary>
        /// Finds the area of a circle along surface of a sphere. Not PI*r^2 (the equation for a plane).
        /// </summary>
        /// <param name="radius">The radius of the sphere in radians. Range: [0, PI].</param>
        /// <returns>The area of the circle i.e. 2*PI*(sphere_radius^2)*(1-cosine(radius/sphere_radius)).</returns>
        /// <seealso cref="https://math.stackexchange.com/questions/1832110/area-of-a-circle-on-sphere"/>
        public static float area_of_circle_on_sphere(float radius, float sphere_radius = 1f)
        {
            return 2*Mathf.PI*(sphere_radius*sphere_radius)*(1-Mathf.Cos(radius/sphere_radius));
        }

        /// <summary>
        /// Find the radius of the base of a cone. Useful for determining sizes for camera shutter effects.
        /// </summary>
        /// <param name="cone_height">The height of the cone (e.g. the distance from the camera's focus (e.g. near/far clipping planes)).</param>
        /// <param name="cone_angle">The angle of the cone (e.g. the field of view of the camera (vertical, horizontal, or diagonal)).</param>
        /// <returns>The radius of the base of the cone.</returns>
        public static float cone_radius(float cone_height, float cone_angle)
        {
            // Derivation: cone_radius / sin(cone_angle / 2) = cone_height / sin(PI/2 - cone_angle / 2) [from Law of Sines]
            //             cone_radius = cone_height * sin(cone_angle / 2) / sin(PI/2 - cone_angle / 2)
            //             cone_radius = cone_height * sin(cone_angle / 2) / cos(cone_angle / 2)
            //             cone_radius = cone_height * tan(cone_angle / 2)

            return cone_height * Mathf.Tan(cone_angle / 2);
        }

        /// <summary>
        /// Returns the vertical field of view of a camera, given it's horizontal field of view and aspect ratio
        /// </summary>
        /// <param name="horizontal_field_of_view">The horizontal field of view (for the width of the screen).</param>
        /// <param name="aspect_ratio">The aspect ratio of the screen.</param>
        /// <returns>The vertical field of view (for the height of the screen).</returns>
        public static float horizontal_to_vertical_field_of_view(float horizontal_field_of_view, float aspect_ratio)
        {
            return 2 * Mathf.Atan(Mathf.Tan(horizontal_field_of_view / 2) / aspect_ratio);
        }

        /// <summary>
        /// Inspector - finds the Manhattan distance between two points
        /// </summary>
        /// <param name="left">The first point. (Order does not matter.)</param>
        /// <param name="right">The second point. (Order does not matter.)</param>
        /// <returns>The Manhattan distance (magnitude).</returns>
        public static float manhattan_distance(Vector3 left, Vector3 right)
        {
            return Mathf.Abs(right.x - left.x) + Mathf.Abs(right.y - left.y) + Mathf.Abs(right.z - left.z);
        }

        /// <summary>
        /// Inspector - returns the median of any three arbitrary numbers.
        /// </summary>
        /// <param name="first">Any number (order does not matter).</param>
        /// <param name="inner">Any number (order does not matter).</param>
        /// <param name="last">Any number (order does not matter).</param>
        /// <returns>The median of the three numbers. (Does not handle NaNs.)</returns>
        public static float median(float first, float inner, float last) // probably first learned this somewhere else but: https://stackoverflow.com/questions/23392321/most-efficient-way-to-find-median-of-three-integers
        {
            float left_minimum = Mathf.Min(first, inner);
            float left_maximum = Mathf.Max(first, inner);
            float minimum_or_median = Mathf.Min(left_maximum, last);
            float median_value = Mathf.Max(left_minimum, minimum_or_median);
            return median_value;
        }

        /// <summary>
        /// The result of the modulo operation when using Euclidean division. The remainder will always be positive.
        /// </summary>
        /// <param name="dividend">The element that will be used to calculate the remainder.</param>
        /// <param name="divisor">The interval upon which the dividend will be divided.</param>
        /// <returns>The remainder.</returns>
        public static float modolo_using_euclidean_division(float dividend, float divisor) // TODO: test/verify
        {
            divisor = Mathf.Abs(divisor);

            float quotient = dividend / divisor;

            float result = dividend - divisor*Mathf.Floor(quotient); // underflow possible on divisor*Mathf.Floor(quotient)?

            return result;
        }

        /// <summary>
        /// Inspector - move a position by its velocity.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="velocity">A velocity vector perpendicular to the position. The magnitude is the radians to rotate (can be zero or greater).</param>
        /// <returns>The mext position after adjusting by velocity.</returns>
        public static Vector3 move(Vector3 position, Vector3 velocity)
        {
            float speed_in_radians = velocity.magnitude;
            Vector3 direction = velocity.normalized;
            return PlanetariaMath.spherical_linear_interpolation(position, direction, speed_in_radians);
        }

        /// <summary>
        /// Inspector - checks the distance between an infinitely long line and a point using perpendicular distance.
        /// </summary>
        /// <param name="line_segment_start">A point on the line.</param>
        /// <param name="line_segment_end">A different point on the line.</param>
        /// <param name="point">The point whose distance will be checked against the line (not line segment).</param>
        /// <returns>The perpendicular distance between the line (not line segment) and a point.</returns>
        public static float point_line_distance(Vector2 line_segment_start, Vector2 line_segment_end, Vector2 point)
        {
            float numerator = Mathf.Abs((line_segment_end.y - line_segment_start.y) * point.x - (line_segment_end.x - line_segment_start.x) * point.y + line_segment_end.x * line_segment_start.y - line_segment_end.y * line_segment_start.x);
            float denominator = Mathf.Sqrt(Mathf.Pow((line_segment_end.y - line_segment_start.y), 2) + Mathf.Pow((line_segment_end.x - line_segment_start.x), 2));

            return numerator / denominator;
        }

        /// <summary>
        /// Inspector - project a point onto the given equator axis (the direction a raindrop would fall along a globe).
        /// </summary>
        /// <param name="point">The point that shall be projected.</param>
        /// <param name="equator_axis">The axis of the equator line (onto which the point shall be projected).</param>
        /// <returns>A projected point along the equator axis.</returns>
        public static Vector3 project_onto_equator(Vector3 point, Vector3 equator_axis)
        {
            if (Vector3.Dot(point, equator_axis) < 0) // make sure you are repelling from the pole closer to the point (so the point moves towards the equator).
            {
                equator_axis *= -1;
            }
            Vector3 gradient = Bearing.repeller(point, equator_axis); // push the point away from the axis towards the equator
            float angle = Mathf.PI/2 - Vector3.Angle(point, equator_axis) * Mathf.Deg2Rad; // find the angle to the equator

            Vector3 projected_point = spherical_linear_interpolation(point, gradient, angle);
            Debug.Assert(Vector3.Dot(projected_point, equator_axis) < Precision.threshold);
            return projected_point;
        }

        /// <summary>
        /// Get the position on a circle defined by x_axis and y_axis.
        /// </summary>
        /// <param name="x_axis">The x-axis.</param>
        /// <param name="y_axis">The y-axis.</param>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The position on a circle defined by interpolating between x_axis and y_axis.</returns>
        public static Vector3 spherical_linear_interpolation(Vector3 x_axis, Vector3 y_axis, float radians)
        {
            return x_axis * Mathf.Cos(radians) + y_axis * Mathf.Sin(radians);
        }

        /// <summary>
        /// Returns the horizontal field of view of a camera, given it's vertical field of view and aspect ratio
        /// </summary>
        /// <param name="vertical_field_of_view">The vertical field of view (for the height of the screen).</param>
        /// <param name="aspect_ratio">The aspect ratio of the screen.</param>
        /// <returns>The horizontal field of view (for the width of the screen).</returns>
        public static float vertical_to_horizontal_field_of_view(float vertical_field_of_view, float aspect_ratio)
        {
            return horizontal_to_vertical_field_of_view(vertical_field_of_view, 1/aspect_ratio);
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