using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    [CustomEditor(typeof(Block))]
    public class BlockEditor : Editor
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void draw_block_gizmos(Block self, GizmoType gizmo_type)
        {
            if (!EditorGlobal.self.hide_graphics)
            {
                foreach (optional<Arc> arc in self.iterator())
                {
                    if (arc.exists)
                    {
                        if (!self.is_dynamic)
                        {
                            ArcEditor.draw_arc(arc.data);
                        }
                        else if (Mathf.Abs(arc.data.circle(0).radius) > Precision.threshold)
                        {
                            ArcEditor.draw_arc(arc.data, self.GetComponent<Transform>());
                        }
                        else
                        {
                            ArcEditor.draw_arc(arc.data, self.GetComponent<Transform>());
                            // FIXME: add corners for dynamic blocks
                        }
                    }
                }
            }
        }
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