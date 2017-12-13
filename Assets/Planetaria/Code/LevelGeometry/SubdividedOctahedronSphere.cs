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
            int size = level_of_detail*2;
            return new SubdividedOctahedronSphere(size);
        }

        public Mesh get_shared_mesh()
        {
            return shared_mesh;
        }

        private SubdividedOctahedronSphere(int size) // size is a positive multiple of 2 for convenience
        {
            int triangle_count = 2*size*size; // smallest octahedron is 2*2*2 (or eight faces) [8 triangles in UV coordinates]
            int vertex_count = (size+1)*(size+1); // smallest octahedron defines up, right, forward, left, back, down x 4 [a 3x3 grid in UV coordinates]

            int identifier = 0;
            optional<Vertex>[,] vertices = new optional<Vertex>[size+1, size+1];

        }

        private Mesh shared_mesh;
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