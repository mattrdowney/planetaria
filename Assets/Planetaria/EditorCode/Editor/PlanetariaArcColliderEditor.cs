#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public class PlanetariaArcColliderEditor : Editor
    {
        public static void draw_arc(PlanetariaArcCollider self, Quaternion orientation, bool[] mask)
        {
            if (!EditorGlobal.self.hide_graphics)
            {
                for (int index = 0; index < 3; ++index)
                {
                    if (!mask[index])
                    {
                        PlanetariaSphereColliderEditor.draw_sphere(self[index], orientation);
                    }
                }
            }
        }
    }
}
#endif

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