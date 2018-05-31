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
            set { data_variable = value; normalize(); }
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
            //sx = x * sqrt(1 - y * y / 2 - z * z / 2 + y * y * z * z / 3);
            //sy = y * sqrt(1 - z * z / 2 - x * x / 2 + z * z * x * x / 3);
            //sz = z * sqrt(1 - x * x / 2 - y * y / 2 + x * x * y * y / 3);

            float x_squared = cube.data.x * cube.data.x;
            float y_squared = cube.data.y * cube.data.y;
            float z_squared = cube.data.z * cube.data.z;

            Vector3 result = Vector3.one;
            result.x = result.x - y_squared / 2 - z_squared / 2 + y_squared * z_squared / 3;
            result.y = result.y - x_squared / 2 - z_squared / 2 + x_squared * z_squared / 3;
            result.z = result.z - x_squared / 2 - y_squared / 2 + x_squared * y_squared / 3;
            result = new Vector3(Mathf.Sqrt(result.x), Mathf.Sqrt(result.y), Mathf.Sqrt(result.z));
            result = Vector3.Scale(result, cube.data);
            return new NormalizedCartesianCoordinates(result);
        }

        /// <summary>
        /// Inspector - Converts cube coordinates into cube UV coordinates.
        /// </summary>
        /// <param name="cube">The unit cube coordinates (|x,y,z| up to +1) that will be converted</param>
        /// <returns>The UV coordinates of a cube [0,1] and their skybox index [0,6).</returns> 
        public static implicit operator CubeUVCoordinates(NormalizedCubeCoordinates cube)
        {
            int face_index = CubeUVCoordinates.face(cube.data);
            Quaternion local_rotation = CubeUVCoordinates.world_to_local_rotation[face_index];
            Vector3 local_cube_position = local_rotation * cube.data;
            float u = local_cube_position.x / 2 + 0.5f;
            float v = local_cube_position.y / 2 + 0.5f;
            return new CubeUVCoordinates(u, v, face_index);
        }

        /// <summary>
        /// Mutator - Normalizes Cube vector
        /// </summary>
        private void normalize()
        {
            // TODO: implement
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