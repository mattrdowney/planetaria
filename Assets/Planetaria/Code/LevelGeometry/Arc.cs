using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// An immutable class that stores an arc along the surface of a unit sphere.
    /// Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
    /// </summary>
    public class Arc
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

        public GeospatialCircle circle(float extrusion = 0)
        {
            Vector3 center = pole(extrusion);
            float radius = elevation(extrusion);
            return GeospatialCircle.circle(center, radius);
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
            return Vector3.Dot(normal_for_left, rightward_for_right) < Precision.tolerance;
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
            float angle = Mathf.Atan2(y,x);
            float result = (angle >= 0 ? angle : angle + 2*Mathf.PI);
            if (float.IsNaN(result) || float.IsInfinity(result))
            {
                result = this.angle();
            }
            Debug.Assert(result >= 0);
            return result;
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
            bool long_path = Vector3.Dot(right_axis, end_axis) < 0;
            arc_angle = Vector3.Angle(from - center, to - center)*Mathf.Deg2Rad;
            arc_latitude = Mathf.PI/2 - Mathf.Acos(elevation);

            if (long_path)
            {
                arc_angle = 2*Mathf.PI - arc_angle;
            }
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
        private static Arc convex_corner(Arc left, Arc right) // CHECKME: does this work when latitude is >0?
        {
            // find the arc along the equator and set the latitude to -PI/2 (implicitly, that means the arc radius is ~0)

            // The equatorial positions can be found by extruding the edges by PI/2
            Vector3 start = left.position(left.angle(), Mathf.PI/2);
            Vector3 end = right.position(0, Mathf.PI/2);

            // The left tangent slope vector should point away from the position "start"
            Vector3 slope = Bearing.right(start, left.normal(left.angle(), Mathf.PI/2)); // idk why it's left instead of right, but it works so w/e

            // Create arc along equator
            Arc result = new Arc(start, slope, end);

            // And move the arc to the "South Pole" instead
            result.arc_latitude = -Mathf.PI/2;

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
        [SerializeField] private Vector3 center_axis;
        /// <summary>An axis that helps define the beginning of the arc.</summary>
        [SerializeField] private Vector3 forward_axis;
        /// <summary>A binormal to center_axis and forward_axis. Determines points after the beginning of the arc.</summary>
        [SerializeField] private Vector3 right_axis;
    
        /// <summary>The length of the arc</summary>
        [SerializeField] private float arc_angle;
        /// <summary>The angle of the arc from its parallel "equator".</summary>
        [SerializeField] private float arc_latitude;
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