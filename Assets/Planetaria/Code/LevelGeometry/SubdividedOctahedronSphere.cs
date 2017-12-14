using UnityEngine;

namespace Planetaria
{
    public class SubdividedOctahedronSphere
    {
        public static SubdividedOctahedronSphere generate(int level_of_detail)
        {
            if (level_of_detail > 127)
            {
                Debug.LogError("Maximum resolution limited to 127 due to 65536 vertex constraint");
                level_of_detail = 127;
            }
            else if (level_of_detail < 1)
            {
                Debug.LogError("Minimum resolution limited to 1 (i.e. an 8 vertex octahedron)");
                level_of_detail = 1;
            }

            SubdividedOctahedronSphere result = new SubdividedOctahedronSphere();
            result.shared_mesh = SubdividedOctahedronSphereBuilder.generate(level_of_detail);

            return result;
        }

        public Mesh get_shared_mesh()
        {
            return shared_mesh;
        }

        private Mesh shared_mesh;
    }

    internal class SubdividedOctahedronSphereBuilder
    {
        public static Mesh generate(int level_of_detail)
        {
            quadrant_size = level_of_detail;
            size = 2*quadrant_size;
            triangle_count = 2*size*size; // smallest octahedron is 2*2*2 (or eight faces) [8 triangles in UV coordinates]
            vertex_count = (size+1)*(size+1); // smallest octahedron defines up, right, forward, left, back, down x 4 [a 3x3 grid in UV coordinates]

            uvs = new Vector2[size+1, size+1];
            positions = new Vector3[size+1, size+1];
            for (int row = 0; row < size+1; ++row)
            {
                float v = row / (float) size;
                for (int column = 0; column < size+1; ++column)
                {
                    float u = column / (float) size;
                    uvs[row, column] = new Vector2(u,v);
                    positions[row, column] = ((NormalizedCartesianCoordinates) new OctahedralUVCoordinates(u, v)).data;
                }
            }

            triangles = new int[triangle_count];
            identifiers = new optional<ushort>[size+1, size+1];
            next_identifier = 0;
        }

        private static void create_triangle_strip(Vector2 begin, Vector2 constant_direction, Vector2 variable_direction, int triangles, bool constant_direction_first)
        {
            if (constant_direction_first)
            {
                constant_first_triangle_strip(begin, constant_direction, variable_direction, triangles);
            }
            else
            {
                variable_first_triangle_strip(begin, constant_direction, variable_direction, triangles);
            }
        }

        private static void constant_first_triangle_strip(Vector2 begin, Vector2 constant_direction, Vector2 variable_direction, int triangles)
        {
            Vector2 current_position = begin;
            while (triangles > 0)
            {
                Vector2 triangle_start = current_position;
                Vector2 triangle_corner = triangle_start + constant_direction;
                Vector2 triangle_end = triangle_corner + variable_direction;

                //variable_direction = current_position - triangle_end; // ORDER DEPENDENCY
                current_position = triangle_end; // ORDER DEPENDENCY
                --triangles;
            }
        }

        private static void variable_first_triangle_strip(Vector2 begin, Vector2 constant_direction, Vector2 variable_direction, int triangles)
        {
            Vector2 current_position = begin;
            while (triangles > 0)
            {
                Vector2 triangle_start = current_position;
                Vector2 triangle_corner = triangle_start + variable_direction;
                Vector2 triangle_end = triangle_corner + constant_direction;

                //first_direction = current_position - triangle_end; // ORDER DEPENDENCY
                current_position = triangle_end; // ORDER DEPENDENCY
                --triangles;
            }
        }

        private static int quadrant_size;
        private static int size;
        private static int triangle_count;
        private static int vertex_count;
        private static int next_identifier;

        private static Vector3[,] positions;
        private static Vector2[,] uvs;
        private static optional<ushort>[,] identifiers;
        private static int[] triangles;
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