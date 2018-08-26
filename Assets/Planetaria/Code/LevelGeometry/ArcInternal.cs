using System;
using UnityEngine;

namespace Planetaria
{
    public partial struct Arc
    {
        /// <summary>
        /// Constructor (Named) - Determines whether a corner is concave or convex and delegates accordingly.
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>A concave or convex corner arc.</returns>
        public static Arc corner(Arc left, Arc right) // TODO: normal constructor
        {
            GeometryType type = corner_type(left, right);
            switch(type)
            {
                case GeometryType.StraightCorner:
                    return straight_corner(left, right);
                case GeometryType.ConvexCorner:
                    return convex_corner(left, right);
                case GeometryType.ConcaveCorner: default:
                    return concave_corner(left, right);
            }
        }

        /// <summary>
        /// Inspector - Determine the type of corner connecting the left-hand-side and right-hand-side Arcs (concave/convex/straight).
        /// </summary>
        /// <param name="left">Arc that will connect to beginning.</param>
        /// <param name="right">Arc that will connect to end.</param>
        /// <returns>
        /// GeometryType.ConvexCorner if the corner arc is convex.
        /// GeometryType.ConcaveCorner if the corner arc is concave.
        /// GeometryType.StraightCorner if the corner arc is a straight angle.
        /// </returns>
        public static GeometryType corner_type(Arc left, Arc right)
        {
            // Both cases
            Vector3 normal_for_left = left.end_normal();
            // Straight angle check
            Vector3 normal_for_right = right.begin_normal();
            // Convex/Concave check
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
        /// Constructor - Creates convex, concave, or great arcs.
        /// </summary>
        /// <param name="curve">The GeospatialCurve that defines an arc on a unit sphere.</param>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        private Arc(Vector3 from, Vector3 slope, Vector3 to, bool clockwise)
        {
            center_axis = Vector3.Cross(from, slope).normalized;
            if (!clockwise) // Invert (e.g. for GeometryType.ConcaveCorner)
            {
                center_axis *= -1;
            }

            float elevation = Vector3.Dot(from, center_axis);
            Vector3 center = elevation * center_axis;

            Vector3 begin_axis = (from - center).normalized;
            Vector3 end_axis = (to - center).normalized;

            bool long_path = Vector3.Dot(slope, to) < 0 || from == to;
            long_path ^= !clockwise; // Long path is inverted if going counterclockwise

            float arc_angle = Vector3.Angle(begin_axis, end_axis) * Mathf.Deg2Rad;
            if (long_path)
            {
                arc_angle = 2*Mathf.PI - arc_angle;
            }
            half_angle = arc_angle/2;

            forward_axis = PlanetariaMath.spherical_linear_interpolation(begin_axis, slope, half_angle); // for great circles only
            right_axis = PlanetariaMath.spherical_linear_interpolation(begin_axis, slope, half_angle + Mathf.PI/2);

            arc_latitude = Mathf.Asin(elevation);
            curvature = edge_type(arc_latitude);
        }

        /// <summary>
        /// Constructor - Create a concave corner arc (for underground path)
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>An arc that represents the path of a burrowed object.</returns>
        private static Arc concave_corner(Arc left, Arc right) // CONSIDER: combine with convex_corner?
        {
            Arc result = convex_corner(left, right);
            result.forward_axis *= -1; // forward axis negated because it's concave
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
            Arc result;
            result.center_axis = -left.end(); // same as -right.begin()

            Vector3 left_normal = left.end_normal();
            Vector3 right_normal = right.begin_normal();
            result.forward_axis = (left_normal + right_normal).normalized; // forward axis is halfway between

            result.right_axis = Vector3.Cross(result.center_axis, result.forward_axis);
            result.half_angle = (Vector3.Angle(left_normal, right_normal)*Mathf.Deg2Rad)/2;
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
            Arc result = convex_corner(left, right);
            result.half_angle = Precision.just_above_zero;
            result.curvature = GeometryType.StraightCorner;
            return result;
        }

        private static Arc validify(Vector3 from, Vector3 slope, Vector3 to, bool clockwise)
        {
            if (from == slope)
            {
                slope = Vector3.up;
            }

            from.Normalize();
            slope = Vector3.ProjectOnPlane(slope, from);
            slope.Normalize();
            to.Normalize();

            return new Arc(from, slope, to, clockwise);
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