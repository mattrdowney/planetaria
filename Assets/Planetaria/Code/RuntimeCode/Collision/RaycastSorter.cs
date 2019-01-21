using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    /*
    public class RaycastSorter : IComparer<PlanetariaRaycastHit>
    {
        /// <summary>
        /// Named Constructor - create a object that sorts points along the given raycast_arc (inverting the result if distance is negative).
        /// </summary>
        /// <param name="raycast_arc">The arc that shall contain the input points.</param>
        /// <param name="distance">The distance of the raycast (only negative sign matters).</param>
        /// <returns>An object that can sort any set of points along the given raycast_arc.</returns>
        public static RaycastSorter sorter(Arc raycast_arc, float distance)
        {
            return new RaycastSorter(raycast_arc, distance);
        }

        public int Compare(PlanetariaRaycastHit left, PlanetariaRaycastHit right)
        {
            Vector2 local_left = world_to_local * left.point;
            Vector2 local_right = world_to_local * right.point;

            return counterclockwise_ordering(local_left, local_right);
        }

        /// <summary>
        /// Inspector - Determines the relative counterclockwise ordering of cartesian 2D points.
        /// </summary>
        /// <param name="left">The left-hand-side of the comparison</param>
        /// <param name="right">The right-hand-side of the comparison</param>
        /// <returns>
        /// False if left comes before right in terms of its angle in polar coordinates - counterclockwise from positive x-axis
        /// True if left is greater than or equal to right by the same metric
        /// </returns>
        private static int counterclockwise_ordering(Vector2 left, Vector2 right) // https://stackoverflow.com/questions/6989100/sort-points-in-clockwise-order/46635372#46635372 // TODO: cleanup
        {
            int left_quadrant = quadrant(left); // quadrant in range [1,4]
            int right_quadrant = quadrant(right);

            int relative_quadrant = left_quadrant - right_quadrant;
            if (relative_quadrant != 0)
            {
                return relative_quadrant;
            }
            float relative_position = (right.x) * (left.y) - (right.y) * (left.x); // only works locally within quadrant
            if (relative_position == 0)
            {
                return 0;
            }
            else
            {
                return relative_position > 0 ? +1 : -1;
            }
        }

        /// <summary>
        /// Inspector - Finds the quadrant of a point [1,4] listed clockwise starting from the positive quadrant
        /// </summary>
        /// <param name="point">The cartesian 2D point in space which is being evaluated.</param>
        /// <returns>Label of [1,4] listed counterclockwise starting from positive quadrant</returns>
        private static int quadrant(Vector2 point)
        {
            bool negative_x = point.x < 0;
            bool negative_y = point.y < 0;

            // the compiler should optimize this; if not, oh well
            if (!negative_y) // top
            {
                return negative_x ? 2 : 1; // left : right
            }
            else // bottom
            {
                return negative_x ? 3 : 4; // left : right
            }
        }

        /// <summary>
        /// Constructor - create a object that sorts points along the given raycast_arc (inverting the result if distance is negative).
        /// </summary>
        /// <param name="raycast_arc">The arc that shall contain the input points.</param>
        /// <param name="distance">The distance of the raycast (only negative sign matters).</param>
        /// <returns>An object that can sort any set of points along the given raycast_arc.</returns>
        private RaycastSorter(Arc raycast_arc, float distance)
        {
            Vector3 raycast_right = raycast_arc.begin();
            Vector3 raycast_up = raycast_arc.position(-raycast_arc.angle()/2 + Mathf.PI/2);
            raycast_up *= distance >= 0 ? +1 : -1; // if distance is negative, the results are sorted in the opposite direction
            Vector3 raycast_forward = Vector3.Cross(raycast_right, raycast_up); // Left-hand rule to find z-axis.
            Quaternion local_to_world = Quaternion.LookRotation(raycast_forward, raycast_up); // Find the orientation of the arc
            world_to_local = Quaternion.Inverse(local_to_world); // Find the rotation that undoes the arc (so that all points lie on the x-y plane).
        }

        /// <summary>A rotation that places points along the x-y plane.</summary>
        private Quaternion world_to_local;
    }
    */
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