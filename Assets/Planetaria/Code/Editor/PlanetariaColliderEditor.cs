using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    [CustomEditor(typeof(PlanetariaCollider))]
    public class PlanetariaColliderEditor : Editor
    {
        public static List<int> hidden_colliders = new List<int>();
        [DrawGizmo(GizmoType.Selected)]
        private static void draw_planetaria_collider_gizmos(PlanetariaCollider self, GizmoType gizmo_type)
        {
            for (int sphere_index = 0; sphere_index < self.colliders.Length; ++sphere_index)
            {
                if (sphere_index != mask[0] && sphere_index != mask[1])
                {
                    Sphere sphere = self.colliders[sphere_index];
                    Gizmos.color = new Color(0,1,0,0.5f); // translucent green
                    Gizmos.DrawSphere(sphere.debug_center, sphere.radius); // consider drawing a higher precision mesh
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(sphere.debug_center, sphere.radius);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(" Collider Mask 1 ");
            mask[0] = EditorGUILayout.IntField(mask[0], GUILayout.Width(50));
            GUILayout.EndHorizontal();
     
            GUILayout.BeginHorizontal();
            GUILayout.Label(" Collider Mask 2 ");
            mask[1] = EditorGUILayout.IntField(mask[1], GUILayout.Width(50));
            GUILayout.EndHorizontal();
     
            SceneView.RepaintAll();
        }

        private static int[] mask = new int[2] {-1, -1 };
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