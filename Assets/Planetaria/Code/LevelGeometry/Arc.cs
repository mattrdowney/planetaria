using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// An immutable class that stores an arc along the surface of a unit sphere.
    /// Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
    /// </summary>
    public partial struct Arc
    {
        /// <summary>
        /// Constructor (Named) - Creates convex, concave, or great arcs.
        /// </summary>
        /// <param name="curve">The GeospatialCurve that defines an arc on a unit sphere.</param>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        public static Arc curve(Vector3 from, Vector3 slope, Vector3 to, bool clockwise = true)
        {
            return validify(from, slope, to, clockwise);
        }

        public static Arc line(Vector3 from, Vector3 to, bool clockwise = true)
        {
            return curve(from, to, to, clockwise);
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

        public GeospatialCircle circle(float extrusion = 0) // TODO: combine with floor()
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
        public bool contains(Vector3 position, float extrusion = 0f) // FIXME: TODO: ensure this works with 1) negative extrusions and 2) concave corners
        {
            bool above_floor = Mathf.Asin(Vector3.Dot(position, center_axis)) >= arc_latitude; // TODO: verify - potential bug?
            bool below_ceiling = Mathf.Asin(Vector3.Dot(position, center_axis)) <= arc_latitude + extrusion;
            bool correct_latitude = above_floor && below_ceiling;

            bool concave_underground = curvature == GeometryType.ConcaveCorner && extrusion > 0;
            bool convex_underground = curvature != GeometryType.ConcaveCorner && extrusion < 0;
            bool underground = concave_underground || convex_underground;

            float angle = position_to_angle(position, extrusion);
            bool correct_angle = angle < arc_angle;

            return (correct_latitude || underground) && correct_angle;
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
        /// Inspector - Creates a SphericalCap that represents the floor (at given elevation).
        /// </summary>
        /// <param name="extrusion">The radius of the collider touching the floor.</param>
        /// <returns>A SphericalCap representing the floor. Normal goes "down" - towards floor.</returns>
        public SphericalCap floor(float extrusion = 0) // TODO: combine with circle()
        {
            return SphericalCap.cap(center_axis, Mathf.Sin(arc_latitude + extrusion)).complement();
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
        public GeometryType type
        {
            get
            {
                return curvature;
            }
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
                    " : " + arc_latitude + "/3.141, " + arc_angle + "/6.283";
        }

        /// <summary>An axis that includes the center of the circle that defines the arc.</summary>
        [NonSerialized] private Vector3 center_axis; // FIXME: NonSerialized
        /// <summary>An axis that helps define the beginning of the arc.</summary>
        [NonSerialized] private Vector3 forward_axis;
        /// <summary>A binormal to center_axis and forward_axis. Determines points after the beginning of the arc.</summary>
        [NonSerialized] private Vector3 right_axis;
    
        /// <summary>The angle of the arc in radians (must be positive). Range: [0, 2PI]</summary>
        [NonSerialized] private float arc_angle; // CONSIDER: use half angle: speed improvements on 1) negative extrusions (e.g. concave corners (sort of)) 2) better angle checking because of arctangent2 (atan2)
        /// <summary>The angle of the arc from its parallel "equator". Range: [-PI/2, +PI/2]</summary>
        [NonSerialized] private float arc_latitude;
        /// <summary>The curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).</summary>
        [NonSerialized] private GeometryType curvature;
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