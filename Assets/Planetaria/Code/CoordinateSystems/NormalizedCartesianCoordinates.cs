using UnityEngine;

namespace Planetaria
{
    public class NormalizedCartesianCoordinates
    {
        public Vector3 data
        {
            get { return data_variable; }
            set { data_variable = value; normalize(); }
        }

        /// <summary>
        /// Constructor - Stores Cartesian coordinates in a wrapper class.
        /// </summary>
        /// <param name="cartesian">The Cartesian coordinates. Note: matches Unity's default Vector3 definition.</param>
        public NormalizedCartesianCoordinates(Vector3 cartesian) // Note: this is an example of where there is ambiguity between variables and types under new naming convention
        {
            data_variable = cartesian;
            normalize(); 
        }

        /// <summary>
        /// Inspector - Converts Cartesian coordinates into octahedral coordinates.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The octahedral coordinates.</returns>
        public static implicit operator NormalizedOctahedralCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            return new NormalizedOctahedralCoordinates(cartesian.data);
        }

        /// <summary>
        /// Inspector - Converts Cartesian coordinates into spherical coordinates.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The spherical coordinates.</returns>
        public static implicit operator NormalizedSphericalCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            float elevation = Mathf.Acos(-cartesian.data.y);
            float azimuth = Mathf.Atan2(cartesian.data.z, -cartesian.data.x);
            return new NormalizedSphericalCoordinates(elevation, azimuth);
        }

        /// <summary>
        /// Inspector - Converts Cartesian coordinates into octahedron UV space.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The UV coordinates for an octahedron.</returns>
        public static implicit operator OctahedralUVCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            return (NormalizedOctahedralCoordinates) cartesian; // implicit chains of length three won't automatically work so convert NormalizedCartesianCoordinates -> NormalizedOctahedralCoordinates -> OctahedralUVCoordinates
        }

        /// <summary>
        /// Inspector - Projects Cartesian coordinates onto plane z=0 in stereoscopic projection coordinates.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The stereoscopic projection coordinates on plane z=0.</returns>
        public static implicit operator StereoscopicProjectionCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            float x = cartesian.data.x;
            float y = cartesian.data.y;
            float z = cartesian.data.z;

            float denominator = (1 - z);

            Vector2 stereoscopic_projection = new Vector2(x / denominator, y / denominator);

            return new StereoscopicProjectionCoordinates(stereoscopic_projection);
        }

        /// <summary>
        /// Mutator - Project the Cartesian coordinates onto a unit sphere.
        /// </summary>
        private void normalize()
        {
            float approximate_length = data_variable.sqrMagnitude;
            float approximate_error = Mathf.Abs(approximate_length-1);
            if (approximate_error < Precision.tolerance)
            {
                return;
            }
 
            data_variable.Normalize();
        }
    
        private Vector3 data_variable;
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