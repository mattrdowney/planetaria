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

        DrawArc(from, to, center, color);
    }

    // FIXME: 1) this fails when center is Vector3.zero 2) this does not work well when from = to = center for corners
    void DrawArc(Vector3 from, Vector3 to, Vector3 center, Color color)
    {
        float radius = (from - center).magnitude;

        from = (from - center).normalized;
        to = (to - center).normalized;

        Vector3 forward = from;
        Vector3 up = center;
        Vector3 right = -Vector3.Cross(from, up);

        float x = Vector3.Dot(to, forward);
        float y = Vector3.Dot(to, right);

        float angle = Mathf.Atan2(y, x);

        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawWireArc(center, center, from, angle, radius);
    }
   
    void DrawRadial(float angle, float radius, Color color)
    {
        DrawArc(arc.position(angle, 0), arc.position(angle, radius), Vector3.zero, color); 
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