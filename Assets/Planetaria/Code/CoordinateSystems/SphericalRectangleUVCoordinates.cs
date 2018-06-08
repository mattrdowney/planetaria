using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct SphericalRectangleUVCoordinates
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
        /// <param name="u">The u coordinate in UV space for the spherical rectangle. Range: (-inf,+inf)</param>
        /// <param name="v">The v coordinate in UV space for the spherical rectangle. Range: (-inf,+inf)</param>
        /// <param name="angular_width">The width of the spherical rectangle in radians. Range: (0,2pi)</param>
        /// <param name="angular_height">The height of the spherical rectangle in radians. Range: (0,2pi)</param>
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
            return (NormalizedSphericalCoordinates)uv; // implicit chains of length three won't automatically work so convert SphericalRectangleUVCoordinates -> NormalizedSphericalCoordinates -> NormalizedCartesianCoordinates
        }

        /// <summary>
        /// Inspector - Converts spherical rectangle UV coordinates into spherical coordinates.
        /// </summary>
        /// <param name="uv">The coordinates in spherical rectangle UV space that will be converted</param>
        /// <returns>The spherical coordinates.</returns> 
        public static implicit operator NormalizedSphericalCoordinates(SphericalRectangleUVCoordinates uv)
        {
            float elevation = Mathf.PI/2 + uv.size.x*(uv.uv.x-0.5f);
            float azimuth = Mathf.PI/2 + uv.size.y*(uv.uv.y-0.5f);
            return new NormalizedSphericalCoordinates(elevation, azimuth);
        }

        // CONSIDER: re-add normalize() for scaling width/height?

        [SerializeField] private Vector2 uv_variable;
        [SerializeField] private Vector2 size_variable;
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