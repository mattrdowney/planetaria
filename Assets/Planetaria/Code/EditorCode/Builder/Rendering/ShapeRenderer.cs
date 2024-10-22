﻿#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public static class ShapeRenderer
    {
        public static void partition_arc(Arc arc, float begin, float end)
        {
            float absolute_begin = begin;
            float absolute_end = end;

            float[,] xyz_signs_begin = get_signs(arc, begin, Precision.delta); //immediately before the first detected change in sign
            float[,] xyz_signs_end = get_signs(arc, end, -Precision.delta); //the first detected change in any sign

            if (!same_signs(ref xyz_signs_begin, ref xyz_signs_end))
            {
                // recursive case: draw line and recursively render remainder

                // binary search and discard ranges with matching slope signs and position signs at ends; and then update the slope signs.
                // when you find a sign that switches, log the exact position of the switch with as much precision as possible
                while (end - begin > Precision.delta)
                {
                    float mid = (begin + end) / 2; //guaranteed not to overflow since numbers are in range [0, 2pi]

                    float[,] xyz_signs_mid = get_signs(arc, mid, Precision.delta); //middle of begin and range_end

                    if (same_signs(ref xyz_signs_begin, ref xyz_signs_mid))
                    {
                        begin = mid;
                        //xyz_signs_begin = xyz_signs_mid; //not necessary, the signs are the same
                    }
                    else
                    {
                        end = mid;
                        xyz_signs_end = xyz_signs_mid;
                    }
                }

                //Debug.Log(xyz_signs_begin[0,0] + " " + xyz_signs_begin[0,1] + " " + xyz_signs_begin[0,2] + " " +
                //        xyz_signs_end[0,0] + " " + xyz_signs_end[0,1] + " " + xyz_signs_end[0,2]);
            }

            // always draw arc segment
            subdivide(arc, absolute_begin, end);

            if (end != absolute_end) // base case to prevent infinite recursion
            {
                partition_arc(arc, end, absolute_end);
            }
        }

        private static float max_error_location(Arc arc, float begin_point_angle, float end_point_angle)
        {
            OctahedronUVCoordinates begin_point = new NormalizedCartesianCoordinates(arc.position(begin_point_angle));
            OctahedronUVCoordinates end_point = new NormalizedCartesianCoordinates(arc.position(end_point_angle));

            float begin = begin_point_angle;
            float end = end_point_angle;

            // 2) use binary / bisection search to find the point of maximal error
            while (end - begin > Precision.delta)
            {
                float midpoint = (begin + end) / 2;

                OctahedronUVCoordinates left_midpoint = new NormalizedCartesianCoordinates(arc.position(midpoint - Precision.delta));
                OctahedronUVCoordinates right_midpoint = new NormalizedCartesianCoordinates(arc.position(midpoint + Precision.delta));

                float error_left  = PlanetariaMath.point_line_distance(begin_point.data, end_point.data, left_midpoint.data);
                float error_right = PlanetariaMath.point_line_distance(begin_point.data, end_point.data, right_midpoint.data);

                if (error_left < error_right) //error begin should be replaced since it has less error
                {
                    begin = midpoint;
                }
                else //error end should be replaced since it has less error
                {
                    end = midpoint;
                }
            }

            return (begin + end) / 2; // return location of max error
        }

        private static void subdivide(Arc arc, float begin_point_angle, float end_point_angle)
        {
            float middle_point_angle = max_error_location(arc, begin_point_angle, end_point_angle);

            Vector3 begin = arc.position(begin_point_angle);
            Vector3 middle = arc.position(middle_point_angle);
            Vector3 end = arc.position(end_point_angle);
        
            OctahedronUVCoordinates begin_point = new NormalizedCartesianCoordinates(begin);
            OctahedronUVCoordinates middle_point = new NormalizedCartesianCoordinates(middle);
            OctahedronUVCoordinates end_point = new NormalizedCartesianCoordinates(end);

            if (PlanetariaMath.point_line_distance(begin_point.data, end_point.data, middle_point.data) > Precision.threshold) // if the max error is greater than a threshold, recursively add the left and right halves into the list of lines
            {
                subdivide(arc, begin_point_angle, middle_point_angle);
                subdivide(arc, middle_point_angle, end_point_angle);
            }
            else
            {
                QuadraticBezierCurve curve = new QuadraticBezierCurve(begin_point.data, middle_point.data, end_point.data);
                VectorGraphicsWriter.set_edge(curve);
            }
        }

        private static bool same_signs(ref float[,] data_A, ref float[,] data_B) // for the purpose of this function, zero is not considered a sign
        {
            for (int derivative = 0; derivative < 2; ++derivative)
            {
                for (int dimension = 0; dimension < 3; ++dimension)
                {
                    if (data_A[derivative, dimension] != data_B[derivative, dimension])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static float[,] get_signs(Arc arc, float location, float delta)
        {
            float[,] result = new float[2,3];

            for (int dimension = 0; dimension < 3; ++dimension)
            {
                result[0, dimension] = Mathf.Sign(arc.position(location)[dimension]);
            }
            for (int dimension = 0; dimension < 3; ++dimension)
            {
                result[1, dimension] = 1f;
                //data[1, dimension] = Mathf.Sign(arc.position(location + delta)[dimension] - arc.position(location)[dimension]) * Mathf.Sign(delta);
            }

            return result;
        }

        private static Dictionary<Arc, List<Discontinuity>> discontinuities;
    }
}

#endif

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
