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
            ShapeEditor.draw_shape(self.shape, self.gameObject.internal_game_object.transform.rotation);
            if (!self.is_field)
            {
                PlanetariaArcColliderEditor.draw_arc(self.shape.block_list[arc_identifier],
                        self.gameObject.internal_game_object.transform.rotation, mask);
            }
            else
            {
                for (int index = 0; index < self.shape.field_list.Length; ++index)
                {
                    if (!mask[index])
                    {
                        PlanetariaSphereColliderEditor.draw_sphere(self.shape.field_list[index],
                        self.gameObject.internal_game_object.transform.rotation);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            PlanetariaCollider self = (PlanetariaCollider)target;
            if (!self.is_field)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(" Arc Number ");
                arc_identifier = EditorGUILayout.IntField(arc_identifier, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                int bit_length = 3;
                for (int bit_position = 0; bit_position < mask.Length && bit_position < bit_length; ++bit_position)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(" Block Mask " + bit_position);
                    mask[bit_position] = EditorGUILayout.Toggle(mask[bit_position], GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                int bit_length = self.shape.field_list.Length;
                for (int bit_position = 0; bit_position < mask.Length && bit_position < bit_length; ++bit_position)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(" Field Mask " + bit_position);
                    mask[bit_position] = EditorGUILayout.Toggle(mask[bit_position], GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                }
            }
     
            SceneView.RepaintAll();
        }
        
        private static bool[] mask = new bool[8*sizeof(int)];
        private static int arc_identifier = 0;
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