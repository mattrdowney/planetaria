using System;
using UnityEngine;

namespace Planetaria
{
    public partial struct Arc
    {
        /// <summary>
        /// Constructor(Named) - Determines whether a corner is concave or convex and delegates accordingly.
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
            center_axis = Vector3.Cross(from, slope);
            if (!clockwise) // Invert (e.g. for GeometryType.ConcaveCorner)
            {
                center_axis *= -1;
            }

            float elevation = Vector3.Dot(from, center_axis);
            Vector3 center = elevation * center_axis;

            Vector3 begin_axis = (from - center).normalized;
            Vector3 end_axis = (to - center).normalized;
            bool long_path = Vector3.Dot(begin_axis, end_axis) < 0 || from == to; // INCORRECT
            long_path ^= !clockwise; // Long path is inverted if going counterclockwise
            float arc_angle = Vector3.Angle(begin_axis, end_axis) * Mathf.Deg2Rad;
            arc_latitude = Mathf.Asin(elevation);

            if (long_path)
            {
                arc_angle = 2*Mathf.PI - arc_angle;
            }

            half_angle = arc_angle/2;

            curvature = edge_type(arc_latitude);

            forward_axis = PlanetariaMath.slerp(begin_axis, slope, half_angle); // for great circles only
            right_axis = PlanetariaMath.slerp(begin_axis, slope, half_angle + Mathf.PI/2);
        }

        /// <summary>
        /// Constructor - Create a concave corner arc (for underground path)
        /// </summary>
        /// <param name="left">The arc that attaches to the beginning of the corner.</param>
        /// <param name="right">The arc that attaches to the end of the corner.</param>
        /// <returns>An arc that represents the path of a burrowed object.</returns>
        private static Arc concave_corner(Arc left, Arc right) // CONSIDER: combine with convex_corner?
        {
            // find the arc along the equator and set the latitude to -PI/2 (implicitly, that means the arc radius is ~0)

            // The equatorial positions can be found by extruding the edges by PI/2
            Vector3 start = left.end(-Mathf.PI/2);
            Vector3 end = right.begin(-Mathf.PI/2);

            // The left tangent slope vector should point away from the position "start"
            Vector3 slope = Bearing.right(start, left.end_normal(-Mathf.PI/2));

            // Create arc along equator
            Arc result = new Arc(start, slope, end, false);

            // And move the arc to the "South Pole" instead
            result.arc_latitude = -Mathf.PI/2;
            result.half_angle = Mathf.PI - result.half_angle;
            
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
            Vector3 slope = Bearing.right(start, right.begin_normal(Mathf.PI/2));

            // Create arc along equator
            Arc result = new Arc(start, slope, end, true);

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
            result.half_angle = Precision.just_above_zero;

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
        /// Inspector - Is the Arc an edge?
        /// </summary>
        /// <returns>
        /// True if the arc is a great or small circle (i.e. GeometryType.ConcaveEdge/ConvexEdge/StraightEdge)
        /// False otherwise (e.g. GeometryType.ConcaveCorner/ConvexCorner/StraightCorner)
        /// </returns>
        public bool is_edge()
        {
            switch(curvature)
            {
                case GeometryType.ConcaveEdge: case GeometryType.ConvexEdge: case GeometryType.StraightEdge:
                    return true;
            }
            return false;
        }
        

        /// <summary>
        /// Inspector - Returns the axis perpendicular to all movement along the arc.
        /// Returns the closer pole, so the pole can be above or below the arc (with respect to the normal).
        /// </summary>
        /// <returns>The closest pole, which is perpendicular to the axes of motion.</returns>
        [Obsolete("Arc.pole() is deprecated, please use Arc.floor().normal instead.")]
        private Vector3 pole(float extrusion = 0f)
        {
            float latitude = arc_latitude + extrusion;

            if (latitude >= 0)
            {
                return center_axis;
            }

            return -center_axis;
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