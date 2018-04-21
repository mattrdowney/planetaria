using UnityEngine;
using Exception;

namespace Planetaria
{
    public static class ProceduralPacking
    {
        /// <summary>
        /// Inspector - The 3D location of the nth sphere in a procedural dense sphere pack.
        /// </summary>
        /// <param name="linear_index">The generator index.</param>
        /// <param name="diameter">The diameter (scale) of the sphere. Default: 2</param>
        public static Vector3 sphere(uint linear_index, float diameter = 2)
        {
            return cube(linear_index, diameter); // FIXME:
        }

        /// <summary>
        /// Inspector - The 3D location of the nth cube in a procedural block.
        /// </summary>
        /// <param name="linear_index">The generator index.</param>
        /// <param name="scale">The size of the cube. Default: 1</param>
        public static Vector3 cube(uint linear_index, float scale = 1)
        {
            uint octant = linear_index / 8;
            Vector3 result = cube_octant(octant, scale);
            for (int dimension = 0; dimension < 3; ++dimension)
            {
                if (Miscellaneous.is_bit_set((int)linear_index, dimension))
                {
                    Vector3 mirror = Vector3.zero;
                    mirror[dimension] = 1;
                    result = Vector3.Reflect(result, mirror);
                }
            }
            return result;
        }

        /// <summary>
        /// Inspector - The 3D location of the nth cube in a procedural octant of a block.
        /// </summary>
        /// <param name="linear_index">The generator index.</param>
        /// <param name="scale">The size of the cube. Default: 1</param>
        public static Vector3 cube_octant(uint octant_index, float scale = 1)
        {
            Vector3 result = Vector3.one*(scale/2);
            float recursive_scale = scale;
            while (octant_index > 0)
            {
                for (int dimension = 0; dimension < 3; ++dimension)
                {
                    if (Miscellaneous.is_bit_set((int)octant_index, dimension))
                    {
                        Vector3 shift = Vector3.zero;
                        shift[dimension] = recursive_scale;
                        result += shift;
                    }
                }
                recursive_scale *= 2;
                octant_index /= 8;
            }

            return result;
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