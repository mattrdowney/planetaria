using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// An immutable class that stores an arc along the surface of a unit sphere.
    /// Includes convex corners, great edges, convex edges, and concave edges. Cannot store concave corners!
    /// </summary>
    public struct Arc
    {
        public static implicit operator Arc(SerializedArc serialized_arc)
        {
            return new Arc(serialized_arc);
        }

        /// <summary>
        /// Inspector - Get the angle of the arc in radians.
        /// </summary>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>The angle of the arc in radians.</returns>
        public float angle(float extrusion = 0f)
        {
            return 2*half_angle;
        }

        public Vector3 begin(float extrusion = 0f)
        {
            return position(-half_angle, extrusion);
        }

        public Vector3 begin_normal(float extrusion = 0f)
        {
            return normal(-half_angle, extrusion);
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
            if (curvature == ArcType.ConcaveCorner) // Concave corners are "inside-out" // TODO: keep DRY (Do not Repeat Yourself)
            {
                extrusion *= -1;
            }

            // TODO: CONSIDER: contains should always returns false when (arc_latitude + extrusion) < -Mathf.PI/2 // (?) && GeometryType.ConcaveCorner (i.e. above ground)

            bool above_floor = Mathf.Asin(Vector3.Dot(position, center_axis)) >= arc_latitude + Mathf.Min(extrusion, 0); // TODO: verify - potential bug?
            bool below_ceiling = Mathf.Asin(Vector3.Dot(position, center_axis)) <= arc_latitude + Mathf.Max(extrusion, 0);
            bool correct_latitude = above_floor && below_ceiling;

            float angle = position_to_angle(position, extrusion);
            bool correct_angle = Mathf.Abs(angle) <= half_angle;

            return correct_latitude && correct_angle;
        }

        public Vector3 end(float extrusion = 0f)
        {
            return position(+half_angle, extrusion);
        }

        public Vector3 end_normal(float extrusion = 0f)
        {
            return normal(+half_angle, extrusion);
        }

        /// <summary>
        /// Inspector - Creates a SphericalCap that represents the floor (at given elevation).
        /// </summary>
        /// <param name="extrusion">The radius of the collider touching the floor.</param>
        /// <returns>A SphericalCap representing the floor. Normal goes "down" - towards floor.</returns> // FIXME: (?) unintuitive normal
        public SphericalCap floor(float extrusion = 0) // TODO: combine with circle()
        {
            return SphericalCap.cap(center_axis, Mathf.Sin(arc_latitude + extrusion)).complement();
        }

        /// <summary>
        /// Inspector - Get the arc length.
        /// </summary>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>The arc length.</returns>
        public float length(float extrusion = 0f)
        {
            return Mathf.Abs(angle() * Mathf.Cos(arc_latitude + extrusion));
        }

        /// <summary>
        /// Inspector - Get the normal at a particular angle.
        /// </summary>
        /// <param name="angle">The angle in radians along the arc.</param>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>A normal on the arc.</returns>
        public Vector3 normal(float angle, float extrusion = 0f) // TODO: delegate to position() [bug-prone when adding PI/2]
        {
            if (curvature == ArcType.ConcaveCorner) // Concave corners are "inside-out"
            {
                extrusion *= -1;
            }

            float actual_elevation = arc_latitude + extrusion;
            if (actual_elevation >= -Mathf.PI/2)
            {
                Vector3 equator_position = PlanetariaMath.spherical_linear_interpolation(forward_axis, right_axis, angle);
                Vector3 result = PlanetariaMath.spherical_linear_interpolation(equator_position, center_axis, actual_elevation + Mathf.PI/2);
                return curvature == ArcType.ConcaveCorner ? -result : result;
            }
            else // if (actual_elevation < -Mathf.PI/2) // Primarily used for concave corners
            {
                actual_elevation += Mathf.PI/2;
                actual_elevation /= Mathf.Cos(half_angle);
                actual_elevation -= Mathf.PI/2;

                if (curvature == ArcType.ConcaveCorner || curvature == ArcType.ConvexCorner) // Concave corners are "inside-out"
                {
                    angle *= -1;
                }
                
                Vector3 normal_position = PlanetariaMath.spherical_linear_interpolation(forward_axis, center_axis, actual_elevation - Mathf.PI/2);
                Vector3 result = PlanetariaMath.spherical_linear_interpolation(normal_position, right_axis, angle);
                return curvature == ArcType.ConvexCorner ? -result : result;
            }
        }

        /// <summary>
        /// Inspector - Get the position at a particular angle.
        /// </summary>
        /// <param name="angle">The angle in radians along the arc.</param>
        /// <param name="extrusion">The radius to extrude.</param>
        /// <returns>A position on the arc.</returns>
        public Vector3 position(float angle, float extrusion = 0f)
        {
            if (curvature == ArcType.ConcaveCorner) // Concave corners are "inside-out"
            {
                extrusion *= -1;
            }

            float actual_elevation = arc_latitude + extrusion;
            if (actual_elevation >= -Mathf.PI/2)
            {
                Vector3 equator_position = PlanetariaMath.spherical_linear_interpolation(forward_axis, right_axis, angle);
                return PlanetariaMath.spherical_linear_interpolation(equator_position, center_axis, actual_elevation);
            }
            else // if (actual_elevation < -Mathf.PI/2) // Primarily used for concave corners
            {
                actual_elevation += Mathf.PI/2;
                actual_elevation /= Mathf.Cos(half_angle);
                actual_elevation -= Mathf.PI/2;
                return PlanetariaMath.spherical_linear_interpolation(forward_axis, center_axis, actual_elevation);
            }
        }

        /// <summary>
        /// Inspector - Find the angle travelled along the arc given a position.
        /// </summary>
        /// <param name="position">The position along the arc (elevation doesn't matter).</param>
        /// <param name="extrusion">The elevation (which is ignored).</param>
        /// <returns>The angle along the arc starting from the center of the arc. Range: [-PI, PI]</returns>
        public float position_to_angle(Vector3 position, float extrusion = 0f) // FIXME: I don't think this works because the position isn't projected
        {
            float x = Vector3.Dot(position, forward_axis);
            float y = Vector3.Dot(position, right_axis);
            float angle = Mathf.Atan2(y,x);
            if (float.IsNaN(angle) || float.IsInfinity(angle))
            {
                angle = Mathf.PI;
            }
            Debug.Assert(Mathf.Abs(angle) <= Mathf.PI, angle);
            return angle;
        }

        /// <summary>
        /// Inspector - gets the curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).
        /// </summary>
        /// <returns>The curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).</returns>
        public ArcType type
        {
            get
            {
                return curvature;
            }
        }

        public SerializedArc serialize()
        {
            return new SerializedArc(Quaternion.LookRotation(forward_axis, center_axis), half_angle, arc_latitude, curvature);
        }

        public static bool operator==(Arc left, Arc right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Arc left, Arc right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(System.Object other_object) // this shouldn't be necessary because of struct equality semantics
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
                    this.half_angle == other.half_angle &&
                    this.arc_latitude == other.arc_latitude &&
                    this.curvature == other.curvature;
        }

        public override int GetHashCode()
        {
            return forward_axis.GetHashCode() ^
                    right_axis.GetHashCode() ^
                    center_axis.GetHashCode() ^
                    arc_latitude.GetHashCode() ^
                    half_angle.GetHashCode() ^
                    curvature.GetHashCode();
        }

        public override string ToString()
        {
            return curvature.ToString() + ": { " + forward_axis.ToString("F4") + ", " + right_axis.ToString("F4") + ", " + center_axis.ToString("F4") + "}" +
                    " : " + arc_latitude + "/+-1.570, " + half_angle + "/+-3.141";
        }

        private Arc(SerializedArc serialized_arc)
        {
            half_angle = serialized_arc.half_angle;
            arc_latitude = serialized_arc.arc_latitude;
            curvature = serialized_arc.curvature;

            forward_axis = Vector3.forward;
            right_axis = (curvature == ArcType.ConcaveCorner ? Vector3.left : Vector3.right);
            center_axis = Vector3.up;

            if (serialized_arc.compact_basis_vectors != Quaternion.identity)
            {
                forward_axis = serialized_arc.compact_basis_vectors * forward_axis;
                right_axis = serialized_arc.compact_basis_vectors * right_axis;
                center_axis = serialized_arc.compact_basis_vectors * center_axis;
            }
        }

        /// <summary>An axis that includes the center of the circle that defines the arc.</summary>
        [NonSerialized] private readonly Vector3 center_axis;
        /// <summary>An axis that helps define the beginning of the arc.</summary>
        [NonSerialized] private readonly Vector3 forward_axis;
        /// <summary>A binormal to center_axis and forward_axis. Determines points after the beginning of the arc.</summary>
        [NonSerialized] private readonly Vector3 right_axis;
    
        /// <summary>The angle of the arc in radians divided by two (must be positive). Range: [-PI, +PI]</summary>
        [NonSerialized] private readonly float half_angle;
        /// <summary>The angle of the arc from its parallel "equator". Range: [-PI/2, +PI/2]</summary>
        [NonSerialized] private readonly float arc_latitude;
        /// <summary>The curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).</summary>
        [NonSerialized] private readonly ArcType curvature;
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.