using UnityEditor;
using UnityEngine;

namespace Planetaria
{
    [CustomEditor(typeof(PlanetariaCollider))]
    public class PlanetariaColliderEditor : Editor
    {
        [DrawGizmo(GizmoType.Selected)]
        private static void draw_planetaria_collider_gizmos(PlanetariaCollider self, GizmoType gizmo_type)
        {
            for (int sphere_index = 0; sphere_index < self.colliders.Length; ++sphere_index)
            {
                if (sphere_index < sizeof(int) && !mask[sphere_index])
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
            PlanetariaCollider self = (PlanetariaCollider) target;
            for (int bit_position = 0; bit_position < mask.Length && bit_position < self.colliders.Length; ++bit_position)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Collider Mask " + bit_position);
                mask[bit_position] = EditorGUILayout.Toggle(mask[bit_position], GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
     
            SceneView.RepaintAll();
        }
        
        private static bool[] mask = new bool[8*sizeof(int)];
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