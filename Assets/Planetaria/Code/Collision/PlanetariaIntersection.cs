using UnityEngine;

public static class PlanetariaIntersection
{
    /// <summary>
    /// Inspector - Returns intersection information between two circles on a sphere.
    /// </summary>
    /// <param name="coordinate_a">Circle A's Cartesian coordinates.</param> // TODO: use Circle struct
    /// <param name="coordinate_b">Circle B's Cartesian coordinates.</param>
    /// <param name="radius_a">Circle A's radius.</param>
    /// <param name="radius_b">Circle B's radius.</param>
    /// <returns>
    /// If there are zero or infinite solutions, returns an empty array;
    /// If there are one or two solutions, returns an array with two Cartesian coordinates.
    /// </returns>
    public static NormalizedCartesianCoordinates[] circle_circle_intersections(NormalizedCartesianCoordinates coordinate_a, NormalizedCartesianCoordinates coordinate_b, float radius_a, float radius_b)
    {
        radius_a = Mathf.Abs(radius_a);
        radius_b = Mathf.Abs(radius_b);

        if (radius_a > Mathf.PI/2 || radius_b > Mathf.PI/2) // ignore invalid input
        {
            return new NormalizedCartesianCoordinates[0];
        }

        float similarity = Vector3.Dot(coordinate_a.data, coordinate_b.data);

        if (Mathf.Abs(similarity) > 1f - Precision.tolerance) // ignore points that are 1) equal or 2) opposite (within an error margin) because they will have infinite solutions
        {
            return new NormalizedCartesianCoordinates[0];
        }

        float arc_distance = Mathf.Acos(similarity);
        float radii_sum = radius_a + radius_b;
        float radii_difference = Mathf.Abs(radius_a - radius_b);

        bool adjacent = arc_distance < radii_sum - Precision.tolerance;
        bool does_not_engulf = radii_difference + Precision.tolerance < arc_distance;
        bool intersects = does_not_engulf && adjacent;

        if (!intersects) // solution set will be null
        {
            return new NormalizedCartesianCoordinates[0];
        }

        // Instead of thinking of a circle on a globe, it's easier to think of the circle as the intersection of a sphere against the surrounding globe
        float distance_from_origin_a = Mathf.Cos(radius_a);
        float distance_from_origin_b = Mathf.Cos(radius_b);
        
        // The center should be distance_from_origin away iff similarity = 0, otherwise you have to push the center closer or further
        float center_fraction_a = (distance_from_origin_a - distance_from_origin_b * similarity) / (1 - similarity * similarity);
        float center_fraction_b = (distance_from_origin_b - distance_from_origin_a * similarity) / (1 - similarity * similarity);

        Vector3 intersection_center = center_fraction_a*coordinate_a.data + center_fraction_b*coordinate_b.data;

        Vector3 binormal = Vector3.Cross(coordinate_a.data, coordinate_b.data);

        float midpoint_distance = Mathf.Sqrt((1 - intersection_center.sqrMagnitude) / binormal.sqrMagnitude); //CONSIDER: rename?

        // Note: tangential circles (i.e. midpoint_distance = 0) return two intersections (i.e. a secant line)
        NormalizedCartesianCoordinates[] intersections = new NormalizedCartesianCoordinates[2];
        intersections[0] = new NormalizedCartesianCoordinates(intersection_center + midpoint_distance*binormal);
        intersections[1] = new NormalizedCartesianCoordinates(intersection_center - midpoint_distance*binormal);
        return intersections;
    }

    public static NormalizedCartesianCoordinates[] arc_path_intersections(Arc arc, NormalizedCartesianCoordinates begin, NormalizedCartesianCoordinates end, float extrusion)
    {
        NormalizedCartesianCoordinates arc_center = new NormalizedCartesianCoordinates(arc.pole());
        NormalizedCartesianCoordinates path_center = new NormalizedCartesianCoordinates(Vector3.Cross(begin.data, end.data).normalized);
        float arc_radius = arc.elevation() + extrusion;
        const float path_radius = Mathf.PI/2;

        NormalizedCartesianCoordinates[] intersections = circle_circle_intersections(arc_center, path_center, arc_radius, path_radius);
        NormalizedCartesianCoordinates[] results = new NormalizedCartesianCoordinates[0];

        for (int intersection_index = 0; intersection_index < intersections.Length; ++intersection_index)
        {
            float angle = arc.position_to_angle(intersections[intersection_index].data);
            angle = angle >= 0 ? angle : angle + 2*Mathf.PI;
            if (angle <= arc.angle())
            {
                System.Array.Resize(ref results, results.Length + 1);
                results[results.Length-1] = intersections[intersection_index];
            }
        }

        return results;
    }

    // TODO: consider arc_arc_intersection()

    public static optional<Vector3> arc_path_intersection(Arc arc, NormalizedCartesianCoordinates begin, NormalizedCartesianCoordinates end, float extrusion)
    {
        NormalizedCartesianCoordinates[] intersections = arc_path_intersections(arc, begin, end, extrusion);
        if (intersections.Length == 0)
        {
            return new optional<Vector3>();
        }

        if (intersections.Length == 1)
        {
            return intersections[0].data;
        }

        float similarity_a = Vector3.Dot(begin.data, intersections[0].data);
        float similarity_b = Vector3.Dot(begin.data, intersections[1].data);

        if (similarity_a > similarity_b) // Note: the collision should be the first in front, but as long as the velocity is capped this is not an issue.
        {
            return intersections[0].data;
        }

        return intersections[1].data; 
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