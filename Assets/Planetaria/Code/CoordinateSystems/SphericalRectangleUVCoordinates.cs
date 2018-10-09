using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct SphericalRectangleUVCoordinates // similar to picture here: https://math.stackexchange.com/questions/1205927/how-to-calculate-the-area-covered-by-any-spherical-rectangle
    {
        public Vector2 uv
        {
            get { return uv_variable; }
        }

        public Vector2 size
        {
            get { return size_variable; }
        }

        public bool valid()
        {
            return 0 <= uv.x && uv.x <= 1 &&
                    0 <= uv.y && uv.y <= 1;
        }

        /// <summary>
        /// Constructor - Stores the spherical rectangle's UV coordinates in a wrapper class.
        /// </summary>
        /// <param name="u">The u coordinate in UV space for the spherical rectangle. Range: (-INF, +INF)</param>
        /// <param name="v">The v coordinate in UV space for the spherical rectangle. Range: (-INF, +INF)</param>
        /// <param name="angular_width">The width of the spherical rectangle in radians. Range: (0, PI)</param>
        /// <param name="angular_height">The height of the spherical rectangle in radians. Range: (0, PI)</param>
        public SphericalRectangleUVCoordinates(float u, float v, float angular_width, float angular_height)
        {
            uv_variable = new Vector2(u, v);
            size_variable = new Vector2(angular_width, angular_height);
        }

        /// <summary>
        /// Inspector - Converts spherical rectangle UV coordinates into normalized cartesian coordinates.
        /// </summary>
        /// <param name="uv">The coordinates in spherical rectangle UV space that will be converted</param>
        /// <returns>The normalized cartesian coordinates.</returns>
        public static implicit operator NormalizedCartesianCoordinates(SphericalRectangleUVCoordinates uv)
        {
            float x = uv.uv.x - 0.5f;
            float y = uv.uv.y - 0.5f;
            NormalizedCartesianCoordinates horizontal_point = new NormalizedSphericalCoordinates(Mathf.PI/2 - uv.size.x, Mathf.PI/2 + y*uv.size.y);
            NormalizedCartesianCoordinates vertical_point = new NormalizedSphericalCoordinates(Mathf.PI/2 - x*uv.size.x, Mathf.PI/2 + uv.size.y);
            Arc horizontal = ArcFactory.curve(Vector3.left, horizontal_point.data, Vector3.right);
            Arc vertical = ArcFactory.curve(Vector3.down, vertical_point.data, Vector3.up);
            optional<Vector3> intersection = PlanetariaIntersection.arc_arc_intersection(horizontal, vertical, 0); // FIXME: some paths won't intersect at a single point
            return new NormalizedCartesianCoordinates(intersection.data); // FIXME: this code is very slow compared to most conversion functions
        }

        // CONSIDER: re-add normalize() for scaling width/height?

        [SerializeField] private Vector2 uv_variable;
        [SerializeField] private Vector2 size_variable;
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