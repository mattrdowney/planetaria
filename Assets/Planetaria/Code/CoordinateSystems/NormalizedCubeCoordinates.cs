using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct NormalizedCubeCoordinates // http://mathproofs.blogspot.com/2005/07/mapping-cube-to-sphere.html
    {
        public Vector3 data
        {
            get { return data_variable; }
        }

        /// <summary>
        /// Constructor - Stores cube coordinates in a wrapper class.
        /// </summary>
        /// <param name="cube">The unit cube coordinates (|x,y,z| up to +1). Note: matches Unity's default Vector3 definition.</param>
        public NormalizedCubeCoordinates(Vector3 cube)
        {
            data_variable = cube;
            normalize();
        }

        /// <summary>
        /// Inspector - Converts cube coordinates into Cartesian coordinates.
        /// </summary>
        /// <param name="cube">The unit cube coordinates (|x,y,z| up to +1) that will be converted.</param>
        /// <returns>The Cartesian coordinates.</returns> 
        public static implicit operator NormalizedCartesianCoordinates(NormalizedCubeCoordinates cube) // LOCATION cube_to_sphere
        {
            return new NormalizedCartesianCoordinates(cube.data);
        }

        /// <summary>
        /// Inspector - Converts cube coordinates into cube UV coordinates.
        /// </summary>
        /// <param name="cube">The unit cube coordinates (|x,y,z| up to +1) that will be converted</param>
        /// <returns>The UV coordinates of a cube [0,1] and their skybox index [0,6).</returns> 
        public static implicit operator CubeUVCoordinates(NormalizedCubeCoordinates cube)
        {
            int face_index = CubeUVCoordinates.face(cube.data);
            Quaternion to_local = Quaternion.identity;
            Vector3 local_cube_position = to_local * cube.data;
            float u = local_cube_position.x / 2 + 0.5f;
            float v = local_cube_position.y / 2 + 0.5f;
            return new CubeUVCoordinates(u, v, face_index);
        }

        /// <summary>
        /// Mutator - Normalizes Cube vector
        /// </summary>
        private void normalize()
        {
            float x_scale = Mathf.Abs(data_variable.x);
            float y_scale = Mathf.Abs(data_variable.y);
            float z_scale = Mathf.Abs(data_variable.z);
            float max_scale = Mathf.Max(x_scale, y_scale, z_scale);
            if (max_scale != 1)
            {
                data_variable /= max_scale;
            }
        }

        [SerializeField] private Vector3 data_variable;
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