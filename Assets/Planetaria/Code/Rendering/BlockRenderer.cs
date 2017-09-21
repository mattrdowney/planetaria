using System.Collections.Generic;
using UnityEngine;

public static class BlockRenderer
{
    public static void render(Block block)
    {
        // For intersecting shapes:

        VectorGraphicsWriter.begin_shape();

        BlockRendererIterator.prepare(block);

        foreach (ArcIterator arc_iterator in BlockRendererIterator.arc_iterator())
        {
            render_arc(arc_iterator.arc, arc_iterator.begin, arc_iterator.end);
        }

        VectorGraphicsWriter.end_shape();
        
        // 2: take any point from intersections
        // 3: if a point exists, draw that point until it reaches the next point in the set, delete last point, repeat #3
        // 4: if there were no intersections, use simple draw to create whole shape.
    }

    private static float max_error_location(Arc arc, float begin_point_angle, float end_point_angle)
    {
        OctahedralUVCoordinates begin_point = new NormalizedCartesianCoordinates(arc.position(begin_point_angle));
        OctahedralUVCoordinates end_point = new NormalizedCartesianCoordinates(arc.position(end_point_angle));

        float begin = begin_point_angle;
        float end = end_point_angle;

        // 2) use binary / bisection search to find the point of maximal error
        while (end - begin > Precision.delta)
        {
            float midpoint = (begin + end) / 2;

            OctahedralUVCoordinates left_midpoint = new NormalizedCartesianCoordinates(arc.position(midpoint - Precision.delta));
            OctahedralUVCoordinates right_midpoint = new NormalizedCartesianCoordinates(arc.position(midpoint + Precision.delta));

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

    private static void render_arc(Arc arc, float begin, float end)
    {
        float range_begin = begin;
        float range_end = end;
        float range_mid = (begin + end) / 2;

        int[,] xyz_signs_begin = new int[2, 3]; //positional, derivative; x, y, z
        int[,] xyz_signs_end   = new int[2, 3];
        int[,] xyz_signs_range_begin = new int[2, 3]; //immediately before the first detected change in sign
        int[,] xyz_signs_range_end = new int[2, 3]; //the first detected change in any sign
        int[,] xyz_signs_range_mid = new int[2, 3]; //middle of begin and range_end

        // get signs for beginning and end
        update_signs(arc, ref xyz_signs_begin, 0, Precision.delta);
        update_signs(arc, ref xyz_signs_end, arc.angle(), -Precision.delta);

        // process new lines until the signs match
        while (!same_signs(ref xyz_signs_begin, ref xyz_signs_end))
        {
            xyz_signs_range_begin = xyz_signs_begin;
            xyz_signs_range_end = xyz_signs_end;

            // binary search and discard ranges with matching slope signs and position signs at ends; and then update the slope signs.
            while (range_end - range_begin > Precision.delta)
            {
                range_mid = (range_begin + range_end) / 2; //guaranteed not to overflow since numbers are in range [0, 2pi]
                update_signs(arc, ref xyz_signs_range_mid, range_mid, Precision.delta);
                if (same_signs(ref xyz_signs_range_begin, ref xyz_signs_range_mid))
                {
                    range_begin = range_mid;
                    //xyz_signs_begin = xyz_signs_range_mid; //not necessary, the signs are the same
                }
                else
                {
                    range_end = range_mid;
                    xyz_signs_range_end = xyz_signs_range_mid;
                }
            }
            // when you find a sign that switches, log the exact position of the switch with as much precision as possible
                
            subdivide(arc, begin, range_begin);
            // when you find that position, you must then switch the x, y, z signs at the new beginning of the arc and the slope signs xyz at the beginning of the arc
            begin = range_end;
            xyz_signs_begin = xyz_signs_range_end;
        }
        // draw the last line
        subdivide(arc, begin, end);
    }

    private static void subdivide(Arc arc, float begin_point_angle, float end_point_angle)
    {
        float middle_point_angle = max_error_location(arc, begin_point_angle, end_point_angle);

        OctahedralUVCoordinates begin_point = new NormalizedCartesianCoordinates(arc.position(begin_point_angle));
        OctahedralUVCoordinates end_point = new NormalizedCartesianCoordinates(arc.position(end_point_angle));
        OctahedralUVCoordinates middle_point = new NormalizedCartesianCoordinates(arc.position(middle_point_angle));

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

    private static bool same_signs(ref int[,] data_A, ref int[,] data_B) // for the purpose of this function, zero is not considered a sign
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

    private static void update_signs(Arc arc, ref int[,] data, float location, float delta)
    {
        //assert data's size is [2, 3]

        for (int dimension = 0; dimension < 3; ++dimension)
        {
            data[0, dimension] = System.Math.Sign(arc.position(location)[dimension]);
        }
        for (int dimension = 0; dimension < 3; ++dimension)
        {
            data[1, dimension] = System.Math.Sign(arc.position(location + delta)[dimension] - arc.position(location)[dimension]) * System.Math.Sign(delta);
        }
    }

    private static Dictionary<Arc, List<Discontinuity>> discontinuities;
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