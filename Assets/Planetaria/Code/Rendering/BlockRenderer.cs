using System.Collections.Generic;
using UnityEngine;

public static class BlockRenderer
{
    private static Dictionary<Arc, List<Discontinuity>> discontinuities;

    public static void render(Block block)
    {
        // For intersecting shapes:

        BlockRendererIterator.prepare(block);

        foreach (ArcIterator arc_iterator in BlockRendererIterator.arc_iterator())
        {
            subdivide(arc_iterator.arc, arc_iterator.begin, arc_iterator.end);
        }
        
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
            // TODO: implement
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