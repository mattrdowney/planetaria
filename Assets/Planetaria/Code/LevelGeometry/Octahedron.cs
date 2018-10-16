using UnityEngine;

namespace Planetaria
{
    [System.Obsolete("Please import an octahedron mesh and use that instead.")]
    public static class Octahedron
    {
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

        private static optional<Mesh> octahedron;
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