using UnityEngine;

namespace Planetaria
{
    public class PlanetariumSphere
    {
        public static Mesh generate(int triangle_budget)
        {
            Mesh original_mesh = TessellatedMesh.generate(Octahedron.octahedron_mesh(), triangle_budget, 1)[0];
            Vector2[] uvs = new Vector2[original_mesh.vertices.Length];
            for (int vertex = 0; vertex < original_mesh.vertices.Length; vertex += 1)
            {
                if (original_mesh.vertices[vertex] != Vector3.down) // almost every point uses polar coordinates from the underlying texture
                {
                    float angle = Vector3.SignedAngle(Vector3.right, Vector3.ProjectOnPlane(original_mesh.vertices[vertex], Vector3.up).normalized, Vector3.up) * Mathf.Deg2Rad;
                    float radius = Vector3.Angle(Vector3.up, original_mesh.vertices[vertex]) * Mathf.Deg2Rad;
                    Vector2 uv = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    uv *= (radius/Mathf.PI);
                    uv = uv/2 + new Vector2(0.5f, 0.5f);
                    uvs[vertex] = uv;
                }
                else // for points at 2PI, the four corners are identical and need to be handled separately.
                {
                    if (original_mesh.uv[vertex] == Vector2.one)
                    {
                        uvs[vertex] = (new Vector2(+1, +1)).normalized;
                    }
                    else if (original_mesh.uv[vertex] == Vector2.up)
                    {
                        uvs[vertex] = (new Vector2(-1, +1)).normalized;
                    }
                    else if (original_mesh.uv[vertex] == Vector2.zero)
                    {
                        uvs[vertex] = (new Vector2(-1, -1)).normalized;
                    }
                    else //if (original_mesh.uv[vertex] == Vector2.right)
                    {
                        uvs[vertex] = (new Vector2(+1, -1)).normalized;
                    }
                }
            }
            original_mesh.uv = uvs;
            return original_mesh;
        }
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