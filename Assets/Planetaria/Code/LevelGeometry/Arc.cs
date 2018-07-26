using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// An immutable class that stores an arc along the surface of a unit sphere.
    /// Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
    /// </summary>
    [Serializable] // FIXME: NonSerialized
    public struct Arc // possibility: Quaternion to store right/up/forward (conveniently 24 bytes: https://stackoverflow.com/questions/1082311/why-should-a-net-struct-be-less-than-16-bytes)
    {
        /// <summary>
        /// Constructor - Creates convex, concave, or great arcs.
        /// </summary>
        /// <param name="curve">The GeospatialCurve that defines an arc on a unit sphere.</param>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        public static Arc curve(Vector3 from, Vector3 slope, Vector3 to)
        {
            return validify(from, slope, to);
        }

        public static Arc line(Vector3 from, Vector3 to)
        {
            return curve(from, to, to);
        }

        /// <summary>
        /// Constructor - Determines whether a corner is concave or convex and delegates accordingly.
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>A concave or convex corner arc.</returns>
        public static Arc corner(Arc left, Arc right) // TODO: normal constructor
        {
            GeometryType type = corner_type(left, right);
            if (type == GeometryType.StraightCorner)
            {
                return straight_corner(left, right);
            }
            if (type == GeometryType.ConvexCorner)
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

        public Vector3 begin(float extrusion = 0f)
        {
            return position(0, extrusion);
        }

        public Vector3 begin_normal(float extrusion = 0f)
        {
            return normal(0, extrusion);
        }

        public GeospatialCircle circle(float extrusion = 0)
        {
            Vector3 center = pole(extrusion);
            float radius = elevation(extrusion);
            return GeospatialCircle.circle(center, radius);
        }
        
        /// Inspector - Determine if a circle is inside the arc / Determine if a point is inside the arc extruded by radius.
        /// </summary>
        /// <param name="position">The position (on a unit sphere) of the circle's center.</param>
        /// <param name="extrusion">The radius to extrude the arc.</param>
        /// <returns>
        /// True if a collision is detected;
        /// False otherwise.
        /// </returns>
        public bool contains(Vector3 position, float extrusion = 0f)
        {
            bool above_floor = Mathf.Asin(Vector3.Dot(position, center_axis)) >= arc_latitude; // TODO: verify - potential bug?
            bool below_ceiling = Mathf.Asin(Vector3.Dot(position, center_axis)) <= arc_latitude + extrusion;
            bool correct_latitude = above_floor && below_ceiling;

            float angle = position_to_angle(position, extrusion);
            bool correct_angle = angle < arc_angle;

            return correct_latitude && correct_angle;
        }

        public Vector3 end(float extrusion = 0f)
        {
            return position(angle(), extrusion);
        }

        public Vector3 end_normal(float extrusion = 0f)
        {
            return normal(angle(), extrusion);
        }

        /// <summary>
        /// Inspector - Get the position at a particular interpolation factor [0,1].
        /// </summary>
        /// <param name="interpolator">The interpolation factor [0,1] along the arc.</param>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>A position on the arc.</returns>
        public Vector3 interpolate(float interpolator, float extrusion = 0f)
        {
            return position(interpolator*angle(), extrusion);
        }

        /// <summary>
        /// Inspector - Determine the type of corner connecting the left-hand-side and right-hand-side Arcs (concave/convex/straight).
        /// </summary>
        /// <param name="left">Arc that will connect to beginning.</param>
        /// <param name="right">Arc that will connect to end.</param>
        /// <returns>
        /// GeometryType.ConvexCorner if the arc is convex.
        /// GeometryType.ConcaveCorner if the arc is concave.
        /// GeometryType.StraightCorner if the arc is a straight angle.
        /// </returns>
        public static GeometryType corner_type(Arc left, Arc right)
        {

            // Straight angle
            Vector3 normal_for_left = left.end_normal();
            Vector3 normal_for_right = right.begin_normal();

            // Convex (true) / Concave (false)
            Vector3 rightward_for_right = Bearing.right(right.begin(), right.begin_normal());

            if (Vector3.Dot(normal_for_left, normal_for_right) > 1 - Precision.tolerance)
            {
                return GeometryType.StraightCorner;
            }
            else
            {
                return Vector3.Dot(normal_for_left, rightward_for_right) < Precision.tolerance ?
                        GeometryType.ConvexCorner : GeometryType.ConcaveCorner;
            }
        }

        /// <summary>
        /// Inspector - Determine the type of edge (concave/convex/straight).
        /// </summary>
        /// <param name="latitude">The latitude of the arc (i.e. the arc_latitude).</param>
        /// <returns>
        /// GeometryType.ConvexEdge if the arc is small circle with convex focus.
        /// GeometryType.ConcaveEdge if the arc is small circle with concave focus.
        /// GeometryType.StraightEdge if the arc is a great circle.
        /// </returns>
        public static GeometryType edge_type(float latitude)
        {
            if (Mathf.Abs(latitude) < Precision.tolerance)
            {
                return GeometryType.StraightEdge;
            }
            else
            {
                return latitude < 0 ? GeometryType.ConvexEdge : GeometryType.ConcaveEdge;
            }
        }

        /// <summary>
        /// Inspector - Get the arc length.
        /// </summary>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>The arc length.</returns>
        public float length(float extrusion = 0f)
        {
            return Mathf.Abs(arc_angle * Mathf.Cos(arc_latitude + extrusion));
        }

        /// <summary>
        /// Inspector - Get the normal at a particular angle.
        /// </summary>
        /// <param name="angle">The angle in radians along the arc.</param>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>A normal on the arc.</returns>
        public Vector3 normal(float angle, float extrusion = 0f)
        {
            return position(angle, extrusion + Mathf.PI/2);
        }

        /// <summary>
        /// Inspector - Creates a SphericalCap that represents the floor (at given elevation).
        /// </summary>
        /// <param name="extrusion">The radius of the collider touching the floor.</param>
        /// <returns>A SphericalCap representing the floor. Normal goes "down" - towards floor.</returns>
        public SphericalCap floor(float extrusion = 0)
        {
            return SphericalCap.cap(center_axis, Mathf.Sin(arc_latitude + extrusion)).complement();
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
        public float position_to_angle(Vector3 position, float extrusion = 0f) // FIXME: I don't think this works because the position isn't projected
        {
            float x = Vector3.Dot(position, forward_axis);
            float y = Vector3.Dot(position, right_axis);
            float angle = Mathf.Atan2(y,x);
            float result = (angle >= 0 ? angle : angle + 2*Mathf.PI);
            if (float.IsNaN(result) || float.IsInfinity(result) || result > this.angle())
            {
                result = this.angle();
            }
            Debug.Assert(0 <= result && result <= this.angle(), result);
            return result;
        }

        /// <summary>
        /// Inspector - gets the curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).
        /// </summary>
        /// <returns>The curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).</returns>
        public GeometryType type()
        {
            return curvature;
        }

        public static bool operator==(Arc left, Arc right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Arc left, Arc right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(System.Object other_object)
        {
            bool equal_type = other_object is Arc;
            if (!equal_type)
            {
                return false;
            }
            Arc other = (Arc) other_object;
            return this.forward_axis == other.forward_axis &&
                    this.right_axis == other.right_axis &&
                    this.center_axis == other.center_axis &&
                    this.arc_angle == other.arc_angle &&
                    this.arc_latitude == other.arc_latitude &&
                    this.curvature == other.curvature;
        }

        public override int GetHashCode()
        {
            return forward_axis.GetHashCode() ^
                    right_axis.GetHashCode() ^
                    center_axis.GetHashCode() ^
                    arc_latitude.GetHashCode() ^
                    arc_angle.GetHashCode() ^
                    curvature.GetHashCode();
        }

        public override string ToString()
        {
            return curvature.ToString() + ": { " + forward_axis.ToString("F4") + ", " + right_axis.ToString("F4") + ", " + center_axis.ToString("F4") + "}" +
                    " : " + arc_latitude + ", " + arc_angle;
        }

        /// <summary>
        /// Constructor - Creates convex, concave, or great arcs.
        /// </summary>
        /// <param name="curve">The GeospatialCurve that defines an arc on a unit sphere.</param>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        private Arc(Vector3 from, Vector3 slope, Vector3 to)
        {
            right_axis = slope;
            forward_axis = from; // for great circles only
            if (from != to) // typical case (for all arcs)
            {
                forward_axis = Vector3.ProjectOnPlane(from - to, slope).normalized; // [from - to] is within the arc's plane
            }
            center_axis = Vector3.Cross(forward_axis, right_axis).normalized; // get binormal using left-hand rule

            float elevation = Vector3.Dot(from, center_axis);
            Vector3 center = elevation * center_axis;

            Vector3 end_axis = (to - center).normalized;
            bool long_path = Vector3.Dot(right_axis, end_axis) < 0 || from == to;
            arc_angle = Vector3.Angle(from - center, to - center)*Mathf.Deg2Rad;
            arc_latitude = Mathf.Asin(elevation);

            if (long_path)
            {
                arc_angle = 2*Mathf.PI - arc_angle;
            }

            curvature = edge_type(arc_latitude);
        }

        /// <summary>
        /// Constructor - Create a concave corner arc (for underground path)
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>An arc that represents the path of a burrowed object.</returns>
        private static Arc concave_corner(Arc left, Arc right)
        {
            // find the arc along the equator and set the latitude to -PI/2 (implicitly, that means the arc radius is ~0)

            // The equatorial positions can be found by extruding the edges by -PI/2
            Vector3 start = left.end(-Mathf.PI/2);
            Vector3 end = right.begin(-Mathf.PI/2);

            // The left tangent slope vector should point away from the position "start"
            Vector3 slope = Bearing.right(start, left.end_normal(-Mathf.PI/2));

            // Create arc along equator
            Arc result = new Arc(start, slope, end);

            // And move the arc to the "South Pole" instead
            result.arc_latitude = -Mathf.PI/2;

            // Flip the handedness from clockwise to counterclockwise
            result.center_axis *= -1;

            result.curvature = GeometryType.ConcaveCorner;
            return result;
        }
        
        /// <summary>
        /// Constructor - Create a convex corner arc.
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>A convex corner arc.</returns>
        private static Arc convex_corner(Arc left, Arc right) // TODO: does this work when latitude is >0?
        {
            // find the arc along the equator and set the latitude to -PI/2 (implicitly, that means the arc radius is ~0)

            // The equatorial positions can be found by extruding the edges by PI/2
            Vector3 start = left.end(Mathf.PI/2);
            Vector3 end = right.begin(Mathf.PI/2);

            // The left tangent slope vector should point away from the position "start"
            Vector3 slope = Bearing.right(start, left.end_normal(Mathf.PI/2));

            // Create arc along equator
            Arc result = new Arc(start, slope, end);

            // And move the arc to the "South Pole" instead
            result.arc_latitude = -Mathf.PI/2;

            result.curvature = GeometryType.ConvexCorner;
            return result;
        }

        /// <summary>
        /// Constructor - Create a straight corner arc (should have zero length).
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>A straight corner arc.</returns>
        private static Arc straight_corner(Arc left, Arc right)
        {
            Vector3 start = left.end();
            Vector3 direction = Bearing.right(start, left.end_normal()); // idk why it's left instead of right, but it works so w/e

            Arc result = Arc.line(start, direction);

            // Straight arcs have no length / angle
            result.arc_angle = Precision.just_above_zero;

            result.curvature = GeometryType.StraightCorner;
            return result;
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
        private float elevation(float extrusion = 0f)
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
        /// Inspector - Returns the axis perpendicular to all movement along the arc.
        /// Returns the closer pole, so the pole can be above or below the arc (with respect to the normal).
        /// </summary>
        /// <returns>The closest pole, which is perpendicular to the axes of motion.</returns>
        private Vector3 pole(float extrusion = 0f)
        {
            float latitude = arc_latitude + extrusion;

            if (latitude >= 0)
            {
                return center_axis;
            }
        
            return -center_axis;
        }

        private static Arc validify(Vector3 from, Vector3 slope, Vector3 to)
        {
            if (from == slope)
            {
                slope = Vector3.up;
            }

            from.Normalize();
            slope = Vector3.ProjectOnPlane(slope, from);
            slope.Normalize();
            to.Normalize();

            return new Arc(from, slope, to);
        }

        /// <summary>An axis that includes the center of the circle that defines the arc.</summary>
        [SerializeField] private Vector3 center_axis; // FIXME: NonSerialized
        /// <summary>An axis that helps define the beginning of the arc.</summary>
        [SerializeField] private Vector3 forward_axis;
        /// <summary>A binormal to center_axis and forward_axis. Determines points after the beginning of the arc.</summary>
        [SerializeField] private Vector3 right_axis;
    
        /// <summary>The angle of the arc in radians (must be positive). Range: [0, 2PI]</summary>
        [SerializeField] private float arc_angle;
        /// <summary>The angle of the arc from its parallel "equator". Range: [-PI/2, +PI/2]</summary>
        [SerializeField] private float arc_latitude;
        /// <summary>The curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).</summary>
        [SerializeField] private GeometryType curvature;
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