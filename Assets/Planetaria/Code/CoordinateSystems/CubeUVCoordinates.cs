using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct CubeUVCoordinates
    {
        /// <summary>
        /// Constructor - Stores the cube's UV coordinates in a Skybox-like wrapper class.
        /// </summary>
        /// <param name="u">The u coordinate in UV space for the cube. Range: [0,1]</param>
        /// <param name="v">The v coordinate in UV space for the cube. Range: [0,1]</param>
        /// <param name="face_index">[0,6) array index; 0=left, 1=right, 2=down, 3=up, 4=front, 5=back</param>
        public CubeUVCoordinates(float u, float v, int face_index)
        {
            uv_variable = new Vector2(u, v);
            face_index_variable = face_index;
            normalize();
        }

        /// <summary>
        /// Inspector - Converts cube UV coordinates into normalized cube coordinates.
        /// </summary>
        /// <param name="skybox">The coordinates in skybox UV space that will be converted.</param>
        /// <returns>The normalized cube coordinates. (At least one of x,y,and z will be magnitude 1.)</returns>
        public static implicit operator NormalizedCubeCoordinates(CubeUVCoordinates skybox)
        {
            Quaternion world_rotation = world_to_local_rotation[skybox.face_index_variable];
            Vector3 local_position = new Vector3(skybox.uv_variable.x, skybox.uv_variable.y, +1);
            Vector3 cube_world_position = world_rotation * local_position;
            return new NormalizedCubeCoordinates(cube_world_position);
        }

        public static int face(Vector3 cartesian)
        {
            float x_magnitude = Mathf.Abs(cartesian.x);
            float y_magnitude = Mathf.Abs(cartesian.y);
            float z_magnitude = Mathf.Abs(cartesian.z);

            if (x_magnitude >= Mathf.Max(y_magnitude, z_magnitude))
            {
                return Mathf.Sign(cartesian.x) == -1 ? 0 : 1;
            }
            else if (y_magnitude >= z_magnitude)
            {
                return Mathf.Sign(cartesian.y) == -1 ? 2 : 3;
            }
            return Mathf.Sign(cartesian.z) == -1 ? 4 : 5;
        }

        /// <summary>
        /// Mutator - Wrap UV coordinates so that neither uv coordinate value is outside of [0,1] and the face index is [0,6)
        /// </summary>
        private void normalize()
        {
            if (uv_variable.x != 1) // modulo goes from [0,1) and we want [0,1]
            {
                uv_variable.x = PlanetariaMath.modolo_using_euclidean_division(uv_variable.x, 1);
            }
            if (uv_variable.y != 1) // modulo goes from [0,1) and we want [0,1]
            {
                uv_variable.y = PlanetariaMath.modolo_using_euclidean_division(uv_variable.y, 1);
            }
            face_index_variable = (int)PlanetariaMath.modolo_using_euclidean_division(face_index_variable, 6); // HACK: CONSIDER: would anyone ever enter something like 2^31 in here?
        }

        public static readonly Quaternion[] world_to_local_rotation = // TODO: is this the right name?
        {
            Quaternion.LookRotation(Vector3.left, Vector3.up), // relative rotation of _LeftTex of Skybox
            Quaternion.LookRotation(Vector3.right, Vector3.up), // _RightTex
            Quaternion.LookRotation(Vector3.down, Vector3.forward), // _DownTex
            Quaternion.LookRotation(Vector3.up, Vector3.back), // _UpTex
            Quaternion.LookRotation(Vector3.back, Vector3.up), // _BackTex
            Quaternion.LookRotation(Vector3.forward, Vector3.up), // _FrontTex
        };

        public static readonly Quaternion[] local_rotation_to_world = // TODO: is this name good?
        {
            Quaternion.Inverse(world_to_local_rotation[0]), // relative rotation of _LeftTex of Skybox
            Quaternion.Inverse(world_to_local_rotation[1]), // _RightTex
            Quaternion.Inverse(world_to_local_rotation[2]), // _DownTex
            Quaternion.Inverse(world_to_local_rotation[3]), // _UpTex
            Quaternion.Inverse(world_to_local_rotation[4]), // _BackTex
            Quaternion.Inverse(world_to_local_rotation[5]), // _FrontTex
        };

        [SerializeField] private int face_index_variable;
        [SerializeField] private Vector2 uv_variable;
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
