using UnityEngine;

namespace Planetaria
{
    public static class Bearing
    {
        public static float angle(Vector3 position, Vector3 normal)
        {
            float x = Vector3.Dot(normal, east(position));
            float y = Vector3.Dot(normal, north(position));
            return Mathf.Atan2(y, x);
        }

        /// <summary>
        /// Find right relative to a point with a given normal vector.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <param name="normal">A vector normal to the position.</param>
        /// <returns>Relative right.</returns>
        public static Vector3 right(Vector3 position, Vector3 normal)
        {
            return Vector3.Cross(normal, position);
        }

        /// <summary>
        /// Find left relative to a point with a given normal vector.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <param name="normal">A vector normal to the position.</param>
        /// <returns>Relative left.</returns>
        public static Vector3 left(Vector3 position, Vector3 normal)
        {
            return -right(position,normal);
        }

        /// <summary>
        /// Find up relative to a point with a given normal vector.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <param name="normal">A vector normal to the position.</param>
        /// <returns>Relative up.</returns>
        public static Vector3 up(Vector3 position, Vector3 normal)
        {
            return normal;
        }

        /// <summary>
        /// Find down relative to a point with a given normal vector.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <param name="normal">A vector normal to the position.</param>
        /// <returns>Relative down.</returns>
        public static Vector3 down(Vector3 position, Vector3 normal)
        {
            return -up(position,normal);
        }

        /// <summary>
        /// Find the direction that goes clockwise around the equator when viewed from above the North Pole.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <returns>Clockwise around the equator at position.</returns>
        public static Vector3 east(Vector3 position)
        {
            if (position.x == 0 && position.z == 0)
            {
                return Vector3.right;
            }

            return new Vector3(position.z, 0, -position.x).normalized; // set y-axis to zero and find "negative inverse"
        }

        /// <summary>
        /// Find the direction that goes counterclockwise around the equator when viewed from above the North Pole.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <returns>Counterclockwise around the equator at position.</returns>
        public static Vector3 west(Vector3 position)
        {
            return -east(position);
        }

        /// <summary>
        /// Find the fastest path to the North Pole.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <returns>The path towards North Pole.</returns>
        public static Vector3 north(Vector3 position)
        {
            return Vector3.Cross(position, east(position)).normalized;
        }

        /// <summary>
        /// Find the fastest path to the South Pole.
        /// </summary>
        /// <param name="position">The position on a unit sphere.</param>
        /// <returns>The path towards South Pole.</returns>
        public static Vector3 south(Vector3 position)
        {
            return -north(position);
        }

        /// <summary>
        /// Find the attraction slope at point "position" that will approach "towards".
        /// </summary>
        /// <param name="position">The current point.</param>
        /// <param name="away_from">The point that should be approached.</param>
        /// <returns>The attracting slope from "position" towards "towards".</returns>
        public static Vector3 attractor(Vector3 position, Vector3 towards)
        {
            Vector3 path_normal = Vector3.Cross(position, towards).normalized;
            if (path_normal == Vector3.zero)
            {
                // Find any perpendicular vector to position and return it as a result (since position and attractor are podal/antipodal)
                return new Vector3(position.y - position.z, position.z - position.x, position.y - position.x); // https://www.quora.com/How-do-I-find-a-vector-perpendicular-to-another-vector
            }
            Vector3 slope = Vector3.Cross(path_normal, position).normalized;
            return slope;
        }

        /// <summary>
        /// Find the repulsion slope at point "position" that will avoid "away_from".
        /// </summary>
        /// <param name="position">The current point.</param>
        /// <param name="away_from">The point that should be avoided.</param>
        /// <returns>The repelling slope from "position" away from "away_from".</returns>
        public static Vector3 repeller(Vector3 position, Vector3 away_from)
        {
            return -attractor(position, away_from);
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