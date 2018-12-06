#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public class PlanetariaSphereColliderEditor : Editor
    {
        public static void draw_sphere(PlanetariaSphereCollider self, Quaternion orientation)
        {
            if (!EditorGlobal.self.hide_graphics)
            {
                Vector3 center = self.center;
                if (orientation != Quaternion.identity)
                {
                    center = orientation * center;
                }
                Gizmos.color = new Color(0,1,0,0.5f); // translucent green
                Gizmos.DrawSphere(center, self.radius); // consider drawing a higher precision mesh
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(center, self.radius);
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