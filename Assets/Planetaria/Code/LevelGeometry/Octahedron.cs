using UnityEngine;

namespace Planetaria
{
    public static class Octahedron
    {
        public static readonly float octahedron_face_distance = 1/Mathf.Sqrt(3);

        /// <summary>
        /// Singleton - Fetch the octahedron mesh.
        /// </summary>
        /// <returns>The Mesh for an octahedron.</returns>
        public static Mesh octahedron_mesh()
        {
            if (!octahedron.exists)
            {
                octahedron = new Mesh();

                octahedron.data.vertices = vertex_list;
                octahedron.data.uv = uv_list;
                octahedron.data.triangles = triangle_list;

                octahedron.data.RecalculateBounds();
                octahedron.data.RecalculateNormals();
            }
            return octahedron.data;
        }

        /// <summary>
        /// Inspector - Use Barycentric coordinates to convert from octahedral coordinates to octahedral UV coordinates.
        /// </summary>
        /// <param name="octahedral">The octahedral coordinates.</param>
        /// <returns>The UV coordinates of an octahedron.</returns>
        public static Vector2 cartesian_to_uv(Vector3 octahedral)
        {
            // get mapping index from 0-7
            int xyz_mask =
                    (octahedral.x < 0 ? 1 : 0) + // x sign is 1s place
                    (octahedral.y < 0 ? 2 : 0) + // y sign is 2s place
                    (octahedral.z < 0 ? 4 : 0);  // z sign is 4s place

            Mesh mesh = octahedron_mesh();
            return convert(mesh.vertices, mesh.uv, octahedral, xyz_mask);
        }

        /// <summary>
        /// Inspector - Use Barycentric coordinates to convert from octahedral UV coordinates to octahedral coordinates.
        /// </summary>
        /// <param name="uv">The UV coordinates of the octahedron.</param>
        /// <returns>The octahedral coordinates.</returns>
        public static Vector3 uv_to_cartesian(Vector2 uv)
        {
            // get mapping index from 0-7
            int xyz_mask =
                    (uv.x < 0.5f ? 1 : 0) + // x sign is 1s place
                    (PlanetariaMath.manhattan_distance(uv - Vector2.one/2) > 0.5f ? 2 : 0) + // y sign is 2s place
                    (uv.y < 0.5f ? 4 : 0);  // z sign is 4s place

            Mesh mesh = octahedron_mesh();
            return convert(mesh.uv, mesh.vertices, uv, xyz_mask);
        }

    
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="From"></typeparam>
        /// <typeparam name="To"></typeparam>
        /// <param name="from_array"></param>
        /// <param name="to_array"></param>
        /// <param name="from"></param>
        /// <param name="xyz_mask">Number from [0,7] inclusive. 1's place => negative x value, 2's => -y, 4's => -z. </param>
        /// <returns></returns>
        private static To convert<From, To>(From[] from_array, To[] to_array, From from, int xyz_mask)
        {
            int triangle_start_index = xyz_mask * 3; // there are 24 indices in the octahedron mesh (i.e. 3 triangle vertices times 8 faces)

            Mesh mesh = octahedron_mesh();
        
            int[] triangle_indices = new int[3];
            Vector3[] from_triangle = new Vector3[3];
            Vector3[] to_triangle = new Vector3[3];

            for (int triangle = 0; triangle < 3; ++triangle)
            {
                int current_triangle_index = mesh.triangles[triangle_start_index + triangle];
                triangle_indices[triangle] = current_triangle_index;
                from_triangle[triangle] = get_vector(from_array[current_triangle_index]);
                to_triangle[triangle] = get_vector(to_array[current_triangle_index]);
            }

            Vector3 uvw = PlanetariaMath.barycentric_coordinates(get_vector(from),
                    from_triangle[0],from_triangle[1],from_triangle[2]);

            Vector3 to = to_triangle[0]*uvw[0] + to_triangle[1]*uvw[1] + to_triangle[2]*uvw[2];

            return set_vector<To>(to); // set_vector() is also a workaround that merely sets a Vector2/Vector3
        }

        /// <summary>
        /// the six mesh corners (vertices) of the octahedron.
        /// </summary>
        private static readonly Vector3[] vertex_list =
        {
                Vector3.up,
                Vector3.right,
                Vector3.forward,
                Vector3.left,
                Vector3.back,
                Vector3.down, Vector3.down, Vector3.down, Vector3.down
        };

        /// <summary>
        /// the UVs for the six mesh corners (vertices) of the octahedron.
        /// </summary>
        private static readonly Vector2[] uv_list =
        {
            new Vector2(0.5f, 0.5f), // up
            new Vector2(1.0f, 0.5f), // right
            new Vector2(0.5f, 1.0f), // forward
            new Vector2(0.0f, 0.5f), // left
            new Vector2(0.5f, 0.0f), // back
            new Vector2(1.0f, 1.0f), // down
            new Vector2(0.0f, 1.0f), // down
            new Vector2(0.0f, 0.0f), // down
            new Vector2(1.0f, 0.0f) // down
        };

        /// <summary>
        /// the triplet indices that define the triangles of the octahedron.
        /// </summary>
        private static readonly int[] triangle_list =
        {
            // Note: normally, Unity's winding order is clockwise,
            // this uses counter-clockwise ordering because the octahedron is "inside-out"
            0, 1, 2, //+x, +y, +z
            0, 2, 3, //-x, +y, +z
            5, 2, 1, //+x, -y, +z
            6, 3, 2, //-x, -y, +z
            0, 4, 1, //+x, +y, -z
            0, 3, 4, //-x, +y, -z
            8, 1, 4, //+x, -y, -z
            7, 4, 3 //-x, -y, -z
        };

        private static Vector3 get_vector<Vector>(Vector vector) // HACK: (I still think this makes the code easier to read than the alternative)
        {
            if (typeof(Vector) == typeof(Vector3)) return (Vector3)(object) vector;
            else                                   return (Vector2)(object) vector;
        }

        private static Vector set_vector<Vector>(Vector3 vector) // HACK: 
        {
            if (typeof(Vector) == typeof(Vector3)) return (Vector) (object) vector;
            else                                   return (Vector) (object) new Vector2(vector.x, vector.y);
        }

        private static optional<Mesh> octahedron;
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