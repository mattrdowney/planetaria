using UnityEngine;

/// <summary>
/// A pseudo-immutable class that stores an arc along the surface of a unit sphere.
/// Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
/// </summary>
[System.Serializable]
public struct Arc
{
    /// <summary>An axis that includes the center of the circle that defines the arc.</summary>
    [SerializeField] private Vector3 center_axis;
    /// <summary>An axis that helps define the beginning of the arc.</summary>
    [SerializeField] private Vector3 forward_axis;
    /// <summary>A binormal to center_axis and forward_axis. Determines points after the beginning of the arc.</summary>
    [SerializeField] private Vector3 right_axis;
    /// <summary>Determines points before the end of the arc. This is normal to center_axis.</summary>
    [SerializeField] private Vector3 before_end;
    
    /// <summary>The length of the arc</summary>
    [SerializeField] private float arc_angle;
    /// <summary>The angle of the arc from its parallel "equator".</summary>
    [SerializeField] private float arc_latitude;

    /// <summary>
    /// Constructor - Creates convex, concave, or great arcs.
    /// </summary>
    /// <param name="start">The start point of the arc.</param>
    /// <param name="right">The sphere-tangential slope/gradient at the start point going rightward.</param>
    /// <param name="end">The end point of the arc.</param>
    /// <returns>An arc along the surface of a unit sphere.</returns>
    public Arc(Vector3 start, Vector3 right, Vector3 end) // TODO: verify
    {
        start.Normalize();
        right = Vector3.ProjectOnPlane(right, start); // enforce orthogonality
        right.Normalize();
        end.Normalize();

        right_axis = right;
        forward_axis = Vector3.ProjectOnPlane(start - end, right).normalized; // [start - end] is within the arc's plane
        center_axis = Vector3.Cross(forward_axis, right_axis).normalized; // get binormal using left-hand rule

        float elevation = Vector3.Dot(start, center_axis);
        Vector3 center = elevation * center_axis;

        Vector3 end_axis = (end - center).normalized;
        before_end = -Vector3.Cross(center_axis, end_axis).normalized;

        bool long_path = Vector3.Dot(right_axis, end_axis) < 0;
        arc_angle = Vector3.Angle(start - center, end - center)*Mathf.Deg2Rad;
        arc_latitude = Mathf.PI/2 - Mathf.Acos(elevation);

        if (long_path)
        {
            arc_angle = 2*Mathf.PI - arc_angle;
        }

        // TODO: Add Arc to global map and create/change colliders/transforms appropriately
    }

    /// <summary>
    /// Constructor - Determines whether a corner is concave or convex and delegates accordingly.
    /// </summary>
    /// <param name="left">The arc that attaches to the beginning of the corner.</param>
    /// <param name="right">The arc that attaches to the end of the corner.</param>
    /// <returns>A concave or convex corner arc.</returns>
    public static optional<Arc> corner(Arc left, Arc right) // TODO: normal constructor
    {
        if (is_convex(left, right))
        {
            return convex_corner(left, right);
        }
        return concave_corner(left, right);
    }

    /// <summary>
    /// Inspector - Get the angle of the arc in radians.
    /// </summary>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>The angle of the arc in radians.</returns>
    public float angle(float extrusion = 0f)
    {
        return arc_angle;
    }

    public static float closest_normal(Arc arc, Vector3 normal, int precision = Precision.float_bits)
    {
        return closest_heuristic(arc, normal_heuristic, normal, precision);
    }

    public static float closest_point(Arc arc, Vector3 point, int precision = Precision.float_bits)
    {
        return closest_heuristic(arc, position_heuristic, point, precision);
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
        bool above_floor = Vector3.Dot(position, center_axis) >= Mathf.Sin(arc_latitude); // XXX: potential bug
        bool below_ceiling = Vector3.Dot(position, center_axis) <= Mathf.Sin(arc_latitude + radius);
        bool correct_latitude = above_floor && below_ceiling;

        bool inside_beginning = Vector3.Dot(position, right_axis) >= 0;
        bool inside_end = Vector3.Dot(position, before_end) >= 0;
        bool reflex_angle = arc_angle > Mathf.PI;
        bool correct_angle = Miscellaneous.count_true_booleans(inside_beginning, inside_end, reflex_angle) >= 2;

        return correct_latitude && correct_angle;
    }

    /// <summary>
    /// Inspector - Determine the elevation of the extruded radius compared to its pole.
    /// </summary>
    /// <param name="extrusion">The radius to extrude the arc.</param>
    /// <returns>
    /// For poles towards the normal, returns a negative number [-PI/2, 0]
    /// representing the angle of decline of the extruded point from the pole.
    /// For poles away from the normal, returns a positive number [0, PI/2]
    /// representing the angle of incline of the extruded point from the pole. 
    /// </returns>
    public float elevation(float extrusion = 0f)
    {
        float latitude = arc_latitude + extrusion;
        
        if (latitude >= 0f) // pole towards normal
        {
            return latitude - Mathf.PI/2; // elevation is zero or negative 
        }

        // pole away from normal
        return latitude + Mathf.PI/2; // elevation is positive
    }

    /// <summary>
    /// Inspector - Determine if the corner between between left's end and right's beginning is convex
    /// (i.e. a reflex angle).
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
    /// Inspector - Returns the axis perpendicular to all movement along the arc.
    /// Returns the closer pole, so the pole can be above or below the arc (with respect to the normal).
    /// </summary>
    /// <returns>The closest pole, which is perpendicular to the axes of motion.</returns>
    public Vector3 pole(float extrusion = 0f)
    {
        float latitude = arc_latitude + extrusion;

        if (latitude >= 0)
        {
            return center_axis;
        }
        
        return -center_axis;
    }

    /// <summary>
    /// Inspector - Get the position at a particular angle.
    /// </summary>
    /// <param name="angle">The angle in radians along the arc.</param>
    /// <param name="extrusion">The radius to extrude.</param>
    /// <returns>A position on the arc.</returns>
    public Vector3 position(float angle, float extrusion = 0f)
    {
        //for concave corners: extrusion / Mathf.Cos(arc_angle / 2) distance at angle/2
        Vector3 equator_position = PlanetariaMath.slerp(forward_axis, right_axis, angle);
        return PlanetariaMath.slerp(equator_position, center_axis, arc_latitude + extrusion);
    }

    /// <summary>
    /// Inspector - Find the angle travelled along the arc given a position. 
    /// </summary>
    /// <param name="position">The position along the arc (elevation doesn't matter).</param>
    /// <param name="extrusion">The elevation (which is ignored).</param>
    /// <returns>The angle along the arc starting from the forward vector.</returns>
    public float position_to_angle(Vector3 position, float extrusion = 0f)
    {
        float x = Vector3.Dot(position, forward_axis);
        float y = Vector3.Dot(position, right_axis);
        return Mathf.Atan2(y,x);
    }
        
    /// <summary>
    /// Constructor - Spoof a concave corner arc with a null value
    /// (since concave corner arcs do not extrude concentrically).
    /// </summary>
    /// <param name="left">The arc that attaches to the beginning of the corner.</param>
    /// <param name="right">The arc that attaches to the end of the corner.</param>
    /// <returns>A null arc (special value for a concave corner).</returns>
    private static optional<Arc> concave_corner(Arc left, Arc right)
    {
        return new optional<Arc>(); // Concave corners are not actually arcs; it's complicated...
    }

    /// <summary>
    /// Constructor - Create a convex corner arc.
    /// </summary>
    /// <param name="left">The arc that attaches to the beginning of the corner.</param>
    /// <param name="right">The arc that attaches to the end of the corner.</param>
    /// <returns>A convex corner arc.</returns>
    private static optional<Arc> convex_corner(Arc left, Arc right) // CHECKME: does this work when latitude is >0?
    {
        // Rather than doubling the codebase for constructors...
        // find the arc along the equator and set the latitude to -PI/2 (implicitly, that means the arc radius is zero)

        // The normal vector should point away from the position of the corner
        Vector3 cut_normal = -right.position(0);

        // The equatorial positions can be found by extruding the edges by PI/2
        Vector3 start = left.position(left.angle(), Mathf.PI/2);
        Vector3 end = right.position(0, Mathf.PI/2);

        // Create arc along equator
        Arc result = new Arc(start, cut_normal, end); // FIXME: cut_normal should be right_tangent

        // And move the arc to the "South Pole" instead
        result.arc_latitude = -Mathf.PI/2;

        return result;
    }

    private static float closest_heuristic(Arc arc, HeuristicFunction distance_heuristic, Vector3 target,
            int precision = Precision.float_bits)
    {
        float closest_angle = 0; // use a valid angle for when arc.angle() returns 0!
        float closest_heuristic = Mathf.Infinity;

        // calculate per each 4 quadrants, e.g. because the angle 2*PI become ambiguous because left == right
        float quadrants = Mathf.Ceil(arc.angle() / (Mathf.PI / 2f)); //maximum of 4, always integral
        for (float quadrant = 0; quadrant < quadrants; ++quadrant)
        {
            float left_angle = arc.angle() * (quadrant / quadrants); // beginning of quadrant e.g. 0.00,0.25,0.50,0.75
            float right_angle = arc.angle() * ((quadrant + 1) / quadrants); // end of quadrant e.g. 0.25,0.50,0.75,1.00

            float left_heuristic = distance_heuristic(arc, target, left_angle); //lower h(x) implies closer to target
            float right_heuristic = distance_heuristic(arc, target, right_angle);

            // A type of binary search
            // 1) find the distance heuristics for left and right side
            // 2) take the greater heuristic and ignore that half (since it's further away from target)
            for (int iteration = 0; iteration < precision; ++iteration) // when using floats, more precision often helps
            {
                float midpoint = (left_angle + right_angle) / 2;
                if (left_heuristic < right_heuristic) // answer is within left half of arc
                {
                    right_angle = midpoint; //throw out the right half (since it's further from target)
                    right_heuristic = distance_heuristic(arc, target, right_angle);
                }
                else // answer is within right half of arc
                {
                    left_angle = midpoint; //throw out the left half (since it's further from target)
                    left_heuristic = distance_heuristic(arc, target, left_angle);
                }
            }

            // find out if this quadrant has a closer point to the target
            if (right_heuristic < closest_heuristic)
            {
                closest_angle = right_angle;
                closest_heuristic = right_heuristic;
            }
            if (left_heuristic < closest_heuristic)
            {
                closest_angle = left_angle;
                closest_heuristic = left_heuristic;
            }
        }
        return closest_angle;
    }

    /// <summary>
    /// Inspector - Get an AABB that contains a circular arc.
    /// </summary>
    /// <param name="arc">The arc that should be surrounded by an AABB.</param>
    /// <returns>Bounds struct AKA axis-aligned bounding box (AABB).</returns>
    private static Bounds get_axis_aligned_bounding_box(Arc arc)
    {
        float x_min = arc.position(closest_point(arc, Vector3.left   )).x;
        float x_max = arc.position(closest_point(arc, Vector3.right  )).x;
        float y_min = arc.position(closest_point(arc, Vector3.down   )).y;
        float y_max = arc.position(closest_point(arc, Vector3.up     )).y;
        float z_min = arc.position(closest_point(arc, Vector3.back   )).z;
        float z_max = arc.position(closest_point(arc, Vector3.forward)).z;

        Vector3 center = new Vector3((x_max + x_min) / 2,
                (y_max + y_min) / 2,
                (z_max + z_min) / 2);

        Vector3 size = new Vector3(x_max - x_min,
                y_max - y_min,
                z_max - z_min);

        return new Bounds(center, size);
    }

    // New naming convention: data types are proper CamelCase and variables are lowercase with underscores.
    private delegate float HeuristicFunction(Arc arc, Vector3 operand, float angle);

    private static float normal_heuristic(Arc arc, Vector3 desired_normal, float angle)
    {
        // return [0,1] for the hell of it, the dot product would suffice, though
        return (Vector3.Dot(arc.normal(angle), -desired_normal) + 1f) / 2;
    }

    private static float position_heuristic(Arc arc, Vector3 desired_position, float angle)
    {
        // return [0,1] for the hell of it, the dot product would suffice, though
        return (Vector3.Dot(arc.position(angle), -desired_position) + 1f) / 2;
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