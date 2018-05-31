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
            int face_index = CubeUVCoordinates.face(cartesian.data);
            Quaternion world_to_local = CubeUVCoordinates.world_to_local_rotation[face_index];
            Quaternion local_to_world = CubeUVCoordinates.local_rotation_to_world[face_index];

            Vector3 sphere_local_position = world_to_local * cartesian.data;
            Vector3 cube_local_position = cubify(sphere_local_position);
            Vector3 cube_world_position = local_to_world * cube_local_position;
            return new NormalizedCubeCoordinates(cube_world_position);
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
        public static implicit operator OctahedralUVCoordinates(NormalizedCartesianCoordinates cartesian)
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

        private static Vector3 cubify(Vector3 sphere) // inverse function of "LOCATION cube_to_sphere" https://stackoverflow.com/questions/2656899/mapping-a-sphere-to-a-cube 
        {
            /*
            Derivation:

            sx = x * sqrt(1 - y * y / 2 - z * z / 2 + y * y * z * z / 3); // from "LOCATION cube_to_sphere"
            sy = y * sqrt(1 - z * z / 2 - x * x / 2 + z * z * x * x / 3);
            //sz = z * sqrt(1 - x * x / 2 - y * y / 2 + x * x * y * y / 3); // we know z=1, so we only need two equations (drop third)

            z=1, simplify

            sx = x * sqrt(1 - y^2 / 2 - 0.5 + y^2 / 3);
            sy = y * sqrt(1 - x^2 / 2 - 0.5 + x^2 / 3);

            sx = x * sqrt(0.5 - y^2/6);
            sy = y * sqrt(0.5 - x^2/6);

            x = sqrt(6)*sx / (sqrt(3 - y^2))
            y = sqrt(6)*sy / (sqrt(3 - x^2))

            x^2 = 6*sx^2 / (3 - y^2)
            y^2 = 6*sy^2 / (3 - x^2)

            ...

            Try solving for x

            x^2 = 6*sx^2 / (3 - y^2)
            x^2(3 - y^2) = 6*sx^2
            x^2(3 - [6*sy^2 / (3 - x^2)]) = 6*sx^2
            3x^2 - x^2(6*sy^2 / (3 - x^2)) = 6*sx^2
            -x^2(6*sy^2 / (3 - x^2)) = 6*sx^2 - 3x^2
            -6sy^2x^2 / (3 - x^2) = 6*sx^2 - 3x^2
            -6sy^2x^2 = (6sx^2 - 3x^2)(3 - x^2)
            -6sy^2x^2 = 3(2sx^2 - x^2)(3 - x^2)
            -2sy^2x^2 = (2sx^2 - x^2)(3 - x^2)
            -2sy^2x^2 = 6sx^2 - 2sx^2x^2 - 3x^2 + x^4
            0 = 6sx^2 - 2sx^2x^2 - 3x^2 + x^4 + 2sy^2x^2
            0 = x^4 - 2sx^2x^2 - 3x^2 + 2sy^2x^2 + 6sx^2
            0 = x^4 + x^2(2sy^2 - 2sx^2 - 3) + 6sx^2

            Quadratic equation found: solve for x^2 = [-b +/- sqrt(b^2 - 4ac)]/(2a)

            a = 1
            b = (2sy^2 - 2sx^2 - 3)
            c = 6sx^2

            x^2 = [-b +/- sqrt(b^2 - 4ac)]/(2a)
            so x = sqrt(quadratic)

            Finally:
            y = sy / sqrt(0.5 - x^2/6);

            Make sure that neither x nor y has a different sign than sx and sy!
            */

            float sx_squared = sphere.x * sphere.x;
            float sy_squared = sphere.y * sphere.y;

            const float a = 1;
            float b = 2*sy_squared - 2*sx_squared - 3;
            float c = 6*sx_squared;

            float discriminant = b*b - 4*a*c;
            float numerator = -b - Mathf.Sqrt(discriminant); // using addition instead would lead to cubes with coordinates outside [-1,+1]
            float denominator = 2*a;
            float quadratic_equation = numerator/denominator;

            float x = Mathf.Sqrt(quadratic_equation);
            float y = sphere.y / Mathf.Sqrt(0.5f - x*x/6);

            x *= Mathf.Sign(sphere.x);
            y *= Mathf.Sign(sphere.y);

            return new Vector3(x, y, 1); // We know z=1 (because at least one cube coordinate has a value of 1)
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