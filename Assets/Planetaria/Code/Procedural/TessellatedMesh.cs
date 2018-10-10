using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public class TessellatedMesh
    {
        public static Mesh[] generate(Mesh mesh, int triangle_budget, int minimum_chunks = 1, bool triangle_strip = false)
        {
            int triangles = mesh.triangles.Length/3;
            if (minimum_chunks < 1 || triangle_budget < triangles)
            {
                return new Mesh[0];
            }
            List<Mesh> result = new List<Mesh>();
            for (int triangle = 0; triangle < triangles; triangle += 1)
            {
                Debug.Log("Check: " + triangle_budget/triangles);
                // TODO create chunks: TessellatedTriangle call with triangle chunk size, then tessellate those triangles and add them to result
                result.Add(TessellatedTriangle.generate(mesh, triangle, triangle_budget/triangles, triangle_strip));
            }
            Debug.Log(result.Count);
            Debug.Log(result[0].triangles.Length/3);
            Debug.Log(result.Count * (result[0].triangles.Length/3));
            Debug.Log(triangle_budget);
            CombineInstance[] mesh_combiner = new CombineInstance[result.Count];
            for (int submesh = 0; submesh < result.Count; submesh += 1)
            {
                mesh_combiner[submesh].mesh = result[submesh];
                mesh_combiner[submesh].transform = Matrix4x4.identity;
            }
            Mesh combined_result = new Mesh();
            combined_result.CombineMeshes(mesh_combiner);
            return new Mesh[] { combined_result };
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