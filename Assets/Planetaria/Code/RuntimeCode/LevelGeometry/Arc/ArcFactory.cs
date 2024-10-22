﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public static class ArcFactory
    {
        /// <summary>
        /// Constructor (Named) - Creates a circle of the given radius (relative to Vector3.forward).
        /// </summary>
        /// <param name="radius">The radius of the circle along a unit-sphere in radians. Range: [0, PI].</param>
        /// <returns>An Arc representing a circle on a unit-sphere.</returns>
        public static SerializedArc circle(float radius)
        {
            return new SerializedArc(Quaternion.LookRotation(Vector3.up, Vector3.back), Mathf.PI, -Mathf.Acos(radius),
                    radius <= Mathf.PI/2 ? ArcType.ConvexEdge : ArcType.ConcaveEdge);
        }

        /// <summary>
        /// Constructor (Named) - Determines whether a corner is concave or convex and delegates accordingly.
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>A concave or convex corner arc.</returns>
        public static SerializedArc corner(Arc left, Arc right) // TODO: normal constructor
        {
            ArcType type = corner_type(left, right);
            switch(type)
            {
                case ArcType.StraightCorner:
                    return straight_corner(left, right);
                case ArcType.ConvexCorner:
                    return convex_corner(left, right);
                case ArcType.ConcaveCorner: default:
                    return concave_corner(left, right);
            }
        }

        /// <summary>
        /// Constructor (Named) - Creates convex, concave, or great arcs.
        /// </summary>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        public static SerializedArc curve(Vector3 from, Vector3 slope, Vector3 to, bool clockwise = true)
        {
            return validify(from, slope, to, clockwise);
        }

        /*
        /// <summary>
        /// Constructor (Named) - Creates convex, concave, or great arcs.
        /// </summary>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        public static Arc[] dual_curve(Vector3 from, Vector3 from_slope, Vector3 to_slope, Vector3 to, bool clockwise = true)
        {
            // from == to --> corner constructor
            // 
            return validify(from, from_slope, to_slope, to, clockwise);
        }
        */

        public static SerializedArc line(Vector3 from, Vector3 to, bool clockwise = true)
        {
            return curve(from, to, to, clockwise);
        }

        /// <summary>
        /// Constructor - Creates convex, concave, or great arcs.
        /// </summary>
        /// <returns>An arc along the surface of a unit sphere.</returns>
        private static SerializedArc arc(Vector3 from, Vector3 slope, Vector3 to, bool clockwise)
        {
            Vector3 right_axis = slope;
            Vector3 forward_axis = from; // for great circles only
            if (from != to) // typical case (for all arcs)
            {
                forward_axis = Vector3.ProjectOnPlane(from - to, slope).normalized; // [from - to] is within the arc's plane
            }
            Vector3 center_axis = Vector3.Cross(forward_axis, right_axis).normalized; // get binormal using left-hand rule
            if (!clockwise) // Invert (e.g. for GeometryType.ConcaveCorner)
            {
                center_axis *= -1;
            }
            Quaternion orientation = Quaternion.LookRotation(forward_axis, center_axis);

            Vector3 begin_axis = Vector3.ProjectOnPlane(from, center_axis).normalized;
            Vector3 end_axis = Vector3.ProjectOnPlane(to, center_axis).normalized;

            float arc_angle = Vector3.Angle(begin_axis, end_axis) * Mathf.Deg2Rad;
            bool long_path = Vector3.Dot(slope, to) < 0 || from == to;
            long_path ^= !clockwise; // Long path is inverted if going counterclockwise
            if (long_path)
            {
                arc_angle = 2*Mathf.PI - arc_angle;
            }
            orientation *= Quaternion.Euler(0, arc_angle/2*Mathf.Rad2Deg, 0);

            float elevation = Vector3.Dot(from, center_axis);
            float arc_latitude = Mathf.Asin(elevation);
            ArcType curvature = edge_type(arc_latitude);

            return new SerializedArc(orientation, arc_angle/2, arc_latitude, curvature);
        }

        /// <summary>
        /// Constructor - Create a concave corner arc (for underground path)
        /// </summary>
        /// <param name="serialized_left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="serialized_right">The arc that attaches to the end of the corner.</param>
        /// <returns>An arc that represents the path of a burrowed object.</returns>
        private static SerializedArc concave_corner(Arc left, Arc right) // CONSIDER: combine with convex_corner?
        {
            SerializedArc convex = convex_corner(left, right);
            Vector3 forward_axis = convex.compact_basis_vectors * Vector3.back; // forward axis negated because it's concave
            Vector3 center_axis = convex.compact_basis_vectors * Vector3.up;
            Quaternion concave_rotation = Quaternion.LookRotation(forward_axis, center_axis);
            return new SerializedArc(concave_rotation, convex.half_angle, convex.arc_latitude, ArcType.ConcaveCorner);
        }

        /// <summary>
        /// Constructor - Create a convex corner arc.
        /// </summary>
        /// <param name="serialized_left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="serialized_right">The arc that attaches to the end of the corner.</param>
        /// <returns>A convex corner arc.</returns>
        private static SerializedArc convex_corner(Arc left, Arc right) // TODO: does this work when latitude is >0?
        {
            Vector3 center_axis = -left.end(); // same as -right.begin()

            Vector3 left_normal = left.end_normal();
            Vector3 right_normal = right.begin_normal();
            Vector3 forward_axis = (left_normal + right_normal).normalized; // forward axis is halfway between

            //Vector3 right_axis = Vector3.Cross(center_axis, forward_axis);
            float angle = (Vector3.Angle(left_normal, right_normal)*Mathf.Deg2Rad);

            Quaternion convex_rotation = Quaternion.LookRotation(forward_axis, center_axis);
            return new SerializedArc(convex_rotation, angle/2, -Mathf.PI/2, ArcType.ConvexCorner);
        }

        /// <summary>
        /// Constructor - Create a straight corner arc (should have zero length).
        /// </summary>
        /// <param name="serialized_left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="serialized_right">The arc that attaches to the end of the corner.</param>
        /// <returns>A straight corner arc.</returns>
        private static SerializedArc straight_corner(Arc left, Arc right)
        {
            SerializedArc convex = convex_corner(left, right);
            return new SerializedArc(convex.compact_basis_vectors, Precision.just_above_zero, convex.arc_latitude, ArcType.StraightCorner);
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
        public static ArcType corner_type(Arc left, Arc right)
        {
            // Both cases
            Vector3 normal_for_left = left.end_normal();
            // Straight angle check
            Vector3 normal_for_right = right.begin_normal();
            // Convex/Concave check
            Vector3 rightward_for_right = Bearing.right(right.begin(), right.begin_normal());

            if (Vector3.Dot(normal_for_left, normal_for_right) > 1 - Precision.tolerance)
            {
                return ArcType.StraightCorner;
            }
            else
            {
                return Vector3.Dot(normal_for_left, rightward_for_right) < Precision.tolerance ?
                        ArcType.ConvexCorner : ArcType.ConcaveCorner;
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
        private static ArcType edge_type(float latitude)
        {
            if (Mathf.Abs(latitude) < Precision.tolerance)
            {
                return ArcType.StraightEdge;
            }
            else
            {
                return latitude < 0 ? ArcType.ConvexEdge : ArcType.ConcaveEdge;
            }
        }

        private static SerializedArc validify(Vector3 from, Vector3 slope, Vector3 to, bool clockwise)
        {
            if (from == slope)
            {
                if (from != to)
                {
                    slope = to;
                }
                else
                {
                    slope = Vector3.right;
                }
            }

            from.Normalize();
            slope = Vector3.ProjectOnPlane(slope, from);
            slope.Normalize();
            to.Normalize();

            return arc(from, slope, to, clockwise);
        }
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