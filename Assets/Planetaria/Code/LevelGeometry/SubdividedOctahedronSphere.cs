using UnityEngine;

namespace Planetaria
{
    public class SubdividedOctahedronSphere // NOTE: Shaders must use CullOff due to the clockwise-to-counterclockwise alternation of backfaces
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
            vertex_uv = new Vector2[vertex_count];
            vertex_positions = new Vector3[vertex_count];

            identifiers = new optional<ushort>[size+1, size+1];
            next_identifier = 0;
            next_triangle = 0;

            create_tesselated_octahedron_sphere();

            Mesh shared_mesh = new Mesh();
            shared_mesh.vertices = vertex_positions;
            shared_mesh.uv = vertex_uv;
            shared_mesh.triangles = triangles;
            return shared_mesh;
        }

        private static void create_tesselated_octahedron_sphere()
        {
            current_position = Vector2.zero;
            create_triangle_wedge_from_apex(new Vector2[] { Vector2.right, new Vector2(-1, +1), Vector2.up });
            create_triangle_wedge_from_base(new Vector2[] { new Vector2(-1, +1), Vector2.right, Vector2.down });
            create_triangle_wedge_from_apex(new Vector2[] { Vector2.left, new Vector2(+1, +1), Vector2.up });
            create_triangle_wedge_from_base(new Vector2[] { new Vector2(+1, +1), Vector3.left, Vector2.down });
            current_position = new Vector2(size,size);
            create_triangle_wedge_from_apex(new Vector2[] { Vector2.left, new Vector2(+1, -1), Vector2.down });
            create_triangle_wedge_from_base(new Vector2[] { new Vector2(+1, -1), Vector2.left, Vector2.up });
            create_triangle_wedge_from_apex(new Vector2[] { Vector2.right, new Vector2(-1, -1), Vector2.down });
            create_triangle_wedge_from_base(new Vector2[] { new Vector2(-1, -1), Vector3.right, Vector2.up });
        }

        private static void create_triangle_wedge_from_base(Vector2[] direction_triplet)
        {
            for (int row = 0; row < quadrant_size; ++row)
            {
                int strip_size = 2*(quadrant_size-1-row) + 1;
                if (row % 2 == 0) // even steps
                {
                    constant_first_triangle_strip(direction_triplet[0], direction_triplet[1], strip_size);
                }
                else // odd steps
                {
                    constant_first_triangle_strip(-direction_triplet[0], direction_triplet[2], strip_size);
                }
            }
        }

        private static void create_triangle_wedge_from_apex(Vector2[] direction_triplet)
        {
            for (int row = 0; row < quadrant_size; ++row)
            {
                int strip_size = 2*row + 1;
                if (row % 2 == 0) // even steps
                {
                    variable_first_triangle_strip(direction_triplet[1], direction_triplet[0], strip_size);
                }
                else // odd steps
                {
                    variable_first_triangle_strip(-direction_triplet[1], direction_triplet[2], strip_size);
                }
            }
        }

        private static void constant_first_triangle_strip(Vector2 constant_direction, Vector2 variable_direction, int triangles)
        {
            while (triangles > 0)
            {
                Vector2 triangle_start = current_position;
                Vector2 triangle_corner = triangle_start + constant_direction;
                Vector2 triangle_end = triangle_corner + variable_direction;

                variable_direction = triangle_end - triangle_start; // ORDER DEPENDENCY
                current_position = triangle_end; // ORDER DEPENDENCY

                create_triangle(new Vector2[] { triangle_start, triangle_corner, triangle_end });
                --triangles;
            }
        }

        private static void variable_first_triangle_strip(Vector2 constant_direction, Vector2 variable_direction, int triangles)
        {
            while (triangles > 0)
            {
                Vector2 triangle_start = current_position;
                Vector2 triangle_corner = triangle_start + variable_direction;
                Vector2 triangle_end = triangle_corner + constant_direction;

                variable_direction = triangle_start - triangle_end; // ORDER DEPENDENCY
                current_position = triangle_end; // ORDER DEPENDENCY

                create_triangle(new Vector2[] { triangle_start, triangle_corner, triangle_end });
                --triangles;
            }
        }

        private static void create_triangle(Vector2[] corners)
        {
            foreach (Vector2 corner in corners)
            {
                int x = Mathf.RoundToInt(corner.x);
                int y = Mathf.RoundToInt(corner.y);

                Debug.Log(x + ", " + y);

                Vector3 position = positions[x,y];
                Vector2 uv = uvs[x,y];
                ushort identifier;
                if (!identifiers[x,y].exists)
                {
                    identifiers[x,y] = identifier = next_identifier++;
                    vertex_positions[identifier] = position;
                    vertex_uv[identifier] = uv;
                }
                identifier = identifiers[x,y].data;
                triangles[next_triangle++] = identifier;
            }
        }

        private static int quadrant_size;
        private static int size;
        private static int triangle_count;
        private static int vertex_count;
        private static ushort next_identifier;
        private static int next_triangle;

        private static Vector2 current_position;

        private static Vector3[,] positions;
        private static Vector2[,] uvs;
        private static optional<ushort>[,] identifiers;
        private static Vector3[] vertex_positions;
        private static Vector2[] vertex_uv;
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