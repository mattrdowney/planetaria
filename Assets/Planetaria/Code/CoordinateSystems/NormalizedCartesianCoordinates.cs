using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct NormalizedCartesianCoordinates
    {
        public Vector3 data
        {
            get { return data_variable; }
            set { data_variable = value; normalize(); }
        }

        /// <summary>
        /// Inspector - Converts Cartesian coordinates into a cube skybox's UV space.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The UV coordinates for an cube skybox.</returns>
        public static implicit operator CubeUVCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            return (NormalizedCubeCoordinates)cartesian; // implicit chains of length three won't automatically work so convert NormalizedCartesianCoordinates -> NormalizedCubeCoordinates -> CubeUVCoordinates
        }

        /// <summary>
        /// Constructor - Stores Cartesian coordinates in a wrapper class.
        /// </summary>
        /// <param name="cartesian">The Cartesian coordinates. Note: matches Unity's default Vector3 definition.</param>
        public NormalizedCartesianCoordinates(Vector3 cartesian)
        {
            data_variable = cartesian;
            normalize();
        }

        /// <summary>
        /// Inspector - Converts Cartesian coordinates into unit cube coordinates.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The unit cube coordinates. (At least one of x,y,and z will be magnitude 1.)</returns>
        public static implicit operator NormalizedCubeCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            int largest_dimension = 0;
            for (int dimension = 1; dimension < 3; ++dimension)
            {
                float largest_magnitude = Mathf.Abs(cartesian.data[largest_dimension]);
                float current_magnitude = Mathf.Abs(cartesian.data[dimension]);
                largest_dimension = (current_magnitude > largest_magnitude ? dimension : largest_dimension);
            }
            Vector3 cube_face_center = Vector3.zero;
            cube_face_center[largest_dimension] = Mathf.Sign(cartesian.data[largest_dimension]);
            Vector3 cube_face_normal = -cube_face_center;
            Plane cube_face = new Plane(cube_face_normal, cube_face_center);
            float intersection_distance;
            cube_face.Raycast(new Ray(Vector3.zero, cartesian.data), out intersection_distance);
            return new NormalizedCubeCoordinates(intersection_distance * cartesian.data);
        }

        /// <summary>
        /// Inspector - Converts Cartesian coordinates into octahedron coordinates.
        /// </summary>
        /// <param name="cartesian">The coordinates in Cartesian space that will be converted</param>
        /// <returns>The octahedron coordinates.</returns>
        public static implicit operator NormalizedOctahedronCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            return new NormalizedOctahedronCoordinates(cartesian.data);
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
        public static implicit operator OctahedronUVCoordinates(NormalizedCartesianCoordinates cartesian)
        {
            return (NormalizedOctahedronCoordinates) cartesian; // implicit chains of length three won't automatically work so convert NormalizedCartesianCoordinates -> NormalizedOctahedralCoordinates -> OctahedralUVCoordinates
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

            if (data_variable != Vector3.zero)
            {
                data_variable.Normalize();
            }
            else // No point should be at the origin
            {
                data_variable = Vector3.up; // TODO: is this breaking anything?
            }
        }
    
        [SerializeField] private Vector3 data_variable;
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