using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{
    Block self;

    void OnDrawGizmos ()
    {
        for (int arc_index = 0; arc_index < self.size(); ++arc_index)
        {
            Arc arc = self.at(ref arc_index);

            ArcEditor.draw_arc(arc, 0.0f, Color.black);
            ArcEditor.draw_arc(arc, 0.05f, Color.gray);
            ArcEditor.draw_arc(arc, 0.1f, Color.white);

            ArcEditor.draw_radial(arc, 0, 0.1f, Color.yellow);
            ArcEditor.draw_radial(arc, arc.angle(), 0.1f, Color.green);
        }
    }

    public void OnEnable()
    {
        self = (Block) target;
    }
}
