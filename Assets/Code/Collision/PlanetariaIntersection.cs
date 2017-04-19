using UnityEngine;

public static class PlanetariaIntersection
{
    /// <summary>
    /// Inspector - Returns intersection information between two circles on a sphere.
    /// </summary>
    /// <param name="coordinate_A">Circle A's Cartesian coordinates.</param> // TODO: use Circle struct
    /// <param name="coordinate_B">Circle B's Cartesian coordinates.</param>
    /// <param name="radius_A">Circle A's radius.</param>
    /// <param name="radius_B">Circle B's radius.</param>
    /// <returns>
    /// If there are zero or infinite solutions, returns an empty array;
    /// If there are one or two solutions, returns an array with two Cartesian coordinates.
    /// </returns>
    public static NormalizedCartesianCoordinates[] circle_circle_intersection(NormalizedCartesianCoordinates coordinate_A, NormalizedCartesianCoordinates coordinate_B, float radius_A, float radius_B)
    {
        radius_A = Mathf.Abs(radius_A);
        radius_B = Mathf.Abs(radius_B);

        if (radius_A > Mathf.PI || radius_B > Mathf.PI) // ignore invalid input
        {
            return new NormalizedCartesianCoordinates[0];
        }

        float similarity = Vector3.Dot(coordinate_A.data, coordinate_B.data);

        if (Mathf.Abs(similarity) > 1f - Precision.tolerance) // ignore points that are 1) equal or 2) opposite (within an error margin) because they will have infinite solutions
        {
            return new NormalizedCartesianCoordinates[0];
        }

        float arc_distance = Mathf.Acos(similarity);
        float radii_sum = radius_A + radius_B;
        float radii_difference = Mathf.Abs(radius_A - radius_B);

        bool bAdjacent = arc_distance < radii_sum - Precision.tolerance;
        bool bDoesNotEngulf = radii_difference + Precision.tolerance < arc_distance;
        bool bInBoundary = bDoesNotEngulf && bAdjacent;

        if (!bInBoundary) // ignore circles that do not intersect
        {
            return new NormalizedCartesianCoordinates[0];
        }

        // Instead of thinking of a circle on a globe, it's easier to think of the circle as the intersection of a sphere against the surrounding globe
        float distance_from_origin_A = Mathf.Cos(radius_A);
        float distance_from_origin_B = Mathf.Cos(radius_B);
        
        // The center should be distance_from_origin away iff similarity = 0, otherwise you have to push the center closer or further
        float center_fraction_A = (distance_from_origin_A - distance_from_origin_B * similarity) / (1 - similarity * similarity);
        float center_fraction_B = (distance_from_origin_B - distance_from_origin_A * similarity) / (1 - similarity * similarity);

        Vector3 intersection_center = center_fraction_A*coordinate_A.data + center_fraction_B*coordinate_B.data;

        Vector3 binormal = Vector3.Cross(coordinate_A.data, coordinate_B.data);

        float midpoint_distance = Mathf.Sqrt((1 - Vector3.Dot(intersection_center, intersection_center)) / Vector3.Dot(binormal, binormal)); //CONSIDER: rename?

        // Note: tangential circles (i.e. midpoint_distance = 0) return two intersections (i.e. a secant line)
        NormalizedCartesianCoordinates[] result = new NormalizedCartesianCoordinates[2];
        result[0] = new NormalizedCartesianCoordinates(intersection_center + midpoint_distance*binormal);
        result[1] = new NormalizedCartesianCoordinates(intersection_center - midpoint_distance*binormal);

        return result;
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