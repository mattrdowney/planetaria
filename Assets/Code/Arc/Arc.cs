using UnityEngine;

/// <summary>
/// A "immutable" class that stores an arc along the surface of a unit sphere. Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
/// </summary>
public class Arc : Object
{
    /// <summary>An axis that includes the center of the circle that defines the arc.</summary>
    Vector3 center_axis;
    /// <summary>An axis that helps define the beginning of the arc.</summary>
    Vector3 forward_axis;
    /// <summary>A binormal to center_axis and forward_axis. Can also be used as a boundary to determine which points are after the beginning of the arc.</summary>
    Vector3 right_axis;
    /// <summary>A boundary that determines which points are before the end of the arc. This is normal to center_axis.</summary>
    Vector3 before_end;
    
    /// <summary>The length of the arc</summary>
    float arc_angle;
    /// <summary>The distance of the arc from its parallel "equator".</summary>
    float arc_latitude;

    /// <summary>
    /// Constructor - Creates convex, concave, or great arcs.
    /// </summary>
    /// <param name="start">The start point of the arc.</param>
    /// <param name="normal">A normal that defines the "cutting plane" that defines the circle.</param>
    /// <param name="end">The end point of the arc.</param>
    /// <param name="long_path">Is the path longer than PI radians?</param>
    /// <returns></returns>
    public static Arc CreateArc(NormalizedCartesianCoordinates start, NormalizedCartesianCoordinates normal, NormalizedCartesianCoordinates end, bool long_path = false)
    {
        Arc result = new Arc();

        Vector3 center = Vector3.Dot(start.data, normal.data) * normal.data;

        result.center_axis = normal.data.normalized;
        result.forward_axis = (start.data - center).normalized;
        result.right_axis = Vector3.Cross(result.center_axis, result.forward_axis).normalized;

        Vector3 end_axis = (end.data - center).normalized;
        result.before_end = -Vector3.Cross(result.center_axis, end_axis).normalized;

        result.arc_angle = Vector3.Angle(result.forward_axis - center, end_axis - center);
        result.arc_latitude = Mathf.Acos(center.magnitude);

        if (long_path)
        {
            result.arc_angle += Mathf.PI;
        }

        bool bIsReflexAngle = result.arc_angle > Mathf.PI;
        bool bVectorsHaveReflexAngle = Vector3.Dot(result.right_axis, result.before_end) < 0;
        
        if (bIsReflexAngle ^ bVectorsHaveReflexAngle) // if a reflex angle is present/absent when it shouldn't be, flip the axes (i.e. make sure the angles match).
        {
            result.right_axis *= -1;
            result.before_end *= -1;
        }

        return result;
    }

    public static Arc CreateCorner(Arc left, Arc right)
    {
        Arc result = new Arc();

        return result;
    }
    
    private static Arc ConcaveCorner()
    {
        return null; // Concave corners are not actually arcs; it's complicated...
    }

    private static Arc ConvexCorner()
    {
        Arc result = new Arc();

        return result;
    }

    /// <summary>
    /// Inspector - Get the angle of the arc in radians.
    /// </summary>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>The angle of the arc in radians.</returns>
    float angle(float extrusion = 0f)
    {
        return arc_angle;
    }

    public static float closest_normal(Arc arc, Vector3 normal, int precision = Precision.float_bits)
    {
        return 0;
    }
    public static float closest_point(Arc arc, Vector3 point, int precision = Precision.float_bits)
    {
        return 0;
    }

    /// <summary>
    /// Inspector - Determine if a circle is inside the arc / Determine if a point is inside the arc extruded by radius.
    /// </summary>
    /// <param name="position">The position (on a unit sphere) of the circle's center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>
    /// True if a collision is detected;
    /// False otherwise.
    /// </returns>
    public bool contains(Vector3 position, float radius = 0f)
    {
        bool bAboveFloor = Vector3.Dot(position, center_axis) >= Mathf.Sin(arc_latitude); // XXX: potential bug based on usage / arc initialization.
        bool bBelowCeiling = Vector3.Dot(position, center_axis) <= Mathf.Sin(arc_latitude + radius);
        bool bCorrectLatitude = bAboveFloor && bBelowCeiling;

        bool bInsideBeginning = Vector3.Dot(position, right_axis) >= 0;
        bool bInsideEnd = Vector3.Dot(position, before_end) >= 0;
        bool bReflexAngle = arc_angle > Mathf.PI;

        bool bTotallyInside = bInsideBeginning && bInsideEnd;
        bool bValidReflexAngle = bReflexAngle && (bInsideBeginning || bInsideEnd);
        bool bCorrectAngle = bTotallyInside || bValidReflexAngle;

        /*bool bCorrectAngle = System.Convert.ToInt32(bInsideBeginning) + // TODO: test if this is properly optimized in C#
                System.Convert.ToInt32(bInsideEnd) +
                System.Convert.ToInt32(bReflexAngle) >= 2;*/

        return bCorrectLatitude && bCorrectAngle;
    }

    /// <summary>
    /// Inspector - Determine if the corner between between left's end and right's beginning is convex (i.e. a reflex angle).
    /// </summary>
    /// <param name="left">Arc that will connect to beginning.</param>
    /// <param name="right">Arc that will connect to end.</param>
    /// <returns>
    /// True if the arc is convex.
    /// False if the arc is concave.
    /// </returns>
    public static bool is_convex(Arc left, Arc right)
    {
        Vector3 normal_for_left = left.normal(left.angle());
        Vector3 rightward_for_right = Bearing.right(right.position(0), right.normal(0));
        return Vector3.Dot(normal_for_left, rightward_for_right) < 0;
    }

    /// <summary>
    /// Inspector - Get the arc length.
    /// </summary>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>The arc length.</returns>
    public float length(float extrusion = 0f)
    {
        return arc_angle * Mathf.Cos(arc_latitude + extrusion);
    }

    /// <summary>
    /// Inspector - Get the normal at a particular angle.
    /// </summary>
    /// <param name="angle">The angle in radians along the arc.</param>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>A normal on the arc.</returns>
    public Vector3 normal(float angle, float extrusion = 0f)
    {
        Vector3 equator_position = PlanetariaMath.slerp(forward_axis, right_axis, arc_angle);
        return PlanetariaMath.slerp(center_axis, -equator_position, arc_latitude + extrusion);
    }

    /// <summary>
    /// Inspector - Get the position at a particular angle.
    /// </summary>
    /// <param name="angle">The angle in radians along the arc.</param>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>A position on the arc.</returns>
    public Vector3 position(float angle, float extrusion = 0f)
    {
        Vector3 equator_position = PlanetariaMath.slerp(forward_axis, right_axis, arc_angle);
        return PlanetariaMath.slerp(equator_position, center_axis, arc_latitude + extrusion);
    }

    /// <summary>
    /// Mutator - Create an AABB that contains a circular arc
    /// </summary>
    /// <param name="arc">The arc whose AABB will be recalculated.</param>
    private static void RecalculateAABB(Arc arc)
    {
        float x_min = arc.position(closest_point(arc, Vector3.left   )).x;
        float x_max = arc.position(closest_point(arc, Vector3.right  )).x;
        float y_min = arc.position(closest_point(arc, Vector3.down   )).y;
        float y_max = arc.position(closest_point(arc, Vector3.up     )).y;
        float z_min = arc.position(closest_point(arc, Vector3.back   )).z;
        float z_max = arc.position(closest_point(arc, Vector3.forward)).z;

        /*
        arc.transform.position = new Vector3((x_max + x_min) / 2,
                                              (y_max + y_min) / 2,
                                              (z_max + z_min) / 2);

        BoxCollider    collider = arc.GetComponent<BoxCollider>();

        collider.size   = new Vector3(x_max - x_min,
                                      y_max - y_min,
                                      z_max - z_min);
        */
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