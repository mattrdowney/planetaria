using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Arc))]
public class ArcEditor : Editor
{
    Arc arc;

    public void OnEnable()
    {
        arc = target as Arc;
    }

    void DrawArc(float radius, Color color)
    {
        Vector3 from = arc.position(0, radius);
        Vector3 to = arc.position(arc.angle(), radius);
        Vector3 normal = arc.pole(radius);
        Vector3 center = Vector3.Project(from, normal);

        DrawArc(from, to, center, normal, color);
    }

    // FIXME: this doesn't quite work because arcs and their complementary arcs are ambiguous
    void DrawArc(Vector3 from, Vector3 to, Vector3 center, Vector3 normal, Color color)
    {
        if (from == center) // If this is a corner
        {
            return; // there is no need to render since corners have no radius
        }

        float radius = (from - center).magnitude;

        from = (from - center).normalized;
        to = (to - center).normalized;

        Vector3 forward = from;
        Vector3 right = -Vector3.Cross(forward, normal);

        float x = Vector3.Dot(to, forward);
        float y = Vector3.Dot(to, right);

        float angle = Mathf.Atan2(y, x);

        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawWireArc(center, normal, from, angle, radius);
    }
   
    void DrawRadial(float angle, float radius, Color color)
    {
        Vector3 from = arc.position(angle, 0);
        Vector3 to = arc.position(angle, radius);
        Vector3 normal = Vector3.Cross(from, to);

        DrawArc(from, to, Vector3.zero, normal, color);
    }

    void OnSceneGUI()
    {
        DrawArc(0.0f, Color.black);
        DrawArc(0.05f, Color.gray);
        DrawArc(0.1f, Color.white);

        DrawRadial(0, 0.1f, Color.yellow);
        DrawRadial(arc.angle(), 0.1f, Color.green);
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