using UnityEngine;

public static class PlanetariaMath
{
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
    /// <param name="cartesian">The Cartesian coordinates.</param>
    /// <returns>The Manhattan distance (magnitude).</returns>
    public static float manhattan_distance(Vector3 cartesian)
    {
        return Mathf.Abs(cartesian.x) + Mathf.Abs(cartesian.y) + Mathf.Abs(cartesian.z);
    }

    /// <summary>
    /// The result of the modulo operation when using Euclidean division. The remainder will always be positive.
    /// </summary>
    /// <param name="dividend">The element that will be used to calculate the remainder.</param>
    /// <param name="divisor">The interval upon which the dividend will be divided.</param>
    /// <returns>The remainder.</returns>
    public static float modolo_using_euclidean_division(float dividend, float divisor)
    {
        divisor = Mathf.Abs(divisor);

        float quotient = dividend / divisor;

        float result = dividend - divisor*Mathf.Floor(quotient);

        if (result < 0f)
        {
            Debug.Log("ERROR: PlanetariaMath: EuclideanDivisionModulo(" + dividend + ", " + divisor + ") = " + result);
        }

        return result;
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
    /// Get the position on a circle defined by x_axis and y_axis.
    /// </summary>
    /// <param name="x_axis">The x-axis.</param>
    /// <param name="y_axis">The y-axis.</param>
    /// <param name="radians">The angle in radians.</param>
    /// <returns>The position on a circle defined by interpolating between x_axis and y_axis.</returns>
    public static Vector3 slerp(Vector3 x_axis, Vector3 y_axis, float radians)
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