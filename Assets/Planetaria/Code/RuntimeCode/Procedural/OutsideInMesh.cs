using System.Linq;
using UnityEngine;

namespace Planetaria
{
    public class OutsideInMesh // NOTE: Shaders must use CullOff due to the clockwise-to-counterclockwise alternation of backfaces
    {
        public static Mesh generate(Mesh original)
        {
            Mesh result = new Mesh();
            result.vertices = original.vertices;
            result.uv = original.uv;
            result.triangles = original.triangles.Reverse().ToArray(); // FIXME: make this conditionally convert (when the normals are facing out).
            result.RecalculateNormals();
            return result;
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