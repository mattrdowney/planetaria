using UnityEngine;

/// <summary>
/// A immutable class that stores an arc along the surface of a unit sphere. Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
/// </summary>
public class Arc : Object
{
    /// <summary>An axis that includes the center of the circle.</summary>
    Vector3 center_axis;
	
    /// <summary>The begin of the arc</summary>
	Vector3 beginning;
    /// <summary>A vector that is normal to both center_axis and beginning.</summary>
	Vector3 binormal;
	
    /// <summary>A boundary that determines what is outside of the arc before the arc's beginning.</summary>
	Vector3 inside_of_beginning;
    /// <summary>A boundary that determines what is outside of the arc after the arc's end.</summary>
	Vector3 inside_of_end;
	
    /// <summary>The length of the arc</summary>
    float arc_angle;
    /// <summary>The distance of the arc from its parallel "equator".</summary>
    float arc_latitude;

    /// <summary>
    /// Inspector - Get the angle of the arc in radians.
    /// </summary>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>The angle of the arc in radians.</returns>
    float angle(float extrusion = 0f)
    {
        return arc_angle;
    }

    public static float closest_normal(Arc arc, Vector3 normal, int precision = Precision.float_bits);
    public static float closest_point(Arc arc, Vector3 point, int precision = Precision.float_bits);

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

        bool bInsideBeginning = Vector3.Dot(position, inside_of_beginning) >= 0;
        bool bInsideEnd = Vector3.Dot(position, inside_of_end) >= 0;
        bool bReflexAngle = arc_angle > Mathf.PI;
        bool bCorrectAngle = System.Convert.ToInt32(bInsideBeginning) +
                System.Convert.ToInt32(bInsideEnd) +
                System.Convert.ToInt32(bReflexAngle) >= 2;
        //bool bTotallyInside = bInsideBeginning && bInsideEnd;
        //bool bValidReflexAngle = arc_angle > Mathf.PI && (bInsideBeginning || bInsideEnd);
        //bool bCorrectAngle = bTotallyInside || bValidReflexAngle;

        return bCorrectLatitude && bCorrectAngle;
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
        Vector3 equator_position = PlanetariaMath.slerp(beginning, binormal, arc_angle);
        PlanetariaMath.slerp(center_axis, -equator_position, arc_latitude + extrusion);
    }

    /// <summary>
    /// Inspector - Get the position at a particular angle.
    /// </summary>
    /// <param name="angle">The angle in radians along the arc.</param>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>A position on the arc.</returns>
	public Vector3 position(float angle, float extrusion = 0f)
    {
        Vector3 equator_position = PlanetariaMath.slerp(beginning, binormal, arc_angle);
        return PlanetariaMath.slerp(equator_position, center_axis, arc_latitude + extrusion);
    }

    /// <summary>
    /// Mutator - Create an AABB that contains a circular arc
    /// </summary>
    /// <param name="arc">The arc whose AABB will be recalculated.</param>
    private static void RecalculateAABB(Arc arc)
	{
		float x_min = closest_position(arc, Vector3.left   ).x;
		float x_max = closest_position(arc, Vector3.right  ).x;
		float y_min = closest_position(arc, Vector3.down   ).y;
		float y_max = closest_position(arc, Vector3.up     ).y;
		float z_min = closest_position(arc, Vector3.back   ).z;
		float z_max = closest_position(arc, Vector3.forward).z;

        /*
		arc.transform.position = new Vector3((x_max + x_min) / 2,
		                    			 	 (y_max + y_min) / 2,
		                            	 	 (z_max + z_min) / 2);

		BoxCollider	collider = arc.GetComponent<BoxCollider>();

		collider.size   = new Vector3(x_max - x_min,
		                              y_max - y_min,
		                              z_max - z_min);
        */
	}
}