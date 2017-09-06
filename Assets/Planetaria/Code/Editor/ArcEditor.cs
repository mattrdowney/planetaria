using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Arc))]
public class ArcEditor : Editor
{
    //Arc arc;
    //PlanetariaTransform transform; // TODO: make arcs relative (for moving platforms)

    /// <summary>
    /// Inspector - Draw an arc (extruded by a radius)
    /// </summary>
    /// <param name="radius">The distance to extrude the arc.</param>
    /// <param name="color">The color of the drawn arc.</param>
    public static void draw_arc(Arc arc, float radius, Color color)
    {
        float angle = arc.angle();
        Vector3 from = arc.position(0, radius);
        Vector3 to = arc.position(angle, radius);
        Vector3 normal = arc.pole(radius);

        RendererFacilities.draw_arc(from, to, normal, angle*Mathf.Rad2Deg, color);
    }

    /// <summary>
    /// Inspector - Draw an extruded radial arc at a particular angle of the specified arc (i.e. the extrusion is its own arc).
    /// </summary>
    /// <param name="angle">The angle at which the extruded radius is drawn.</param>
    /// <param name="radius">The radius to extrude the arc.</param>
    /// <param name="color">The color of the drawn radial arc.</param>
    public static void draw_radial(Arc arc, float angle, float radius, Color color)
    {
        Vector3 from = arc.position(angle, 0);
        Vector3 to = arc.position(angle, radius);
        Vector3 normal = Vector3.Cross(from, to);

        RendererFacilities.draw_arc(from, to, normal, radius*Mathf.Rad2Deg, color);
    }

    /*void OnEnable()
    {
        arc = target as Arc;
    }*/

    /*void OnSceneGUI()
    {
        ArcEditor.draw_arc(arc, 0.0f, Color.black);
        ArcEditor.draw_arc(arc, 0.05f, Color.gray);
        ArcEditor.draw_arc(arc, 0.1f, Color.white);

        ArcEditor.draw_radial(arc, 0, 0.1f, Color.yellow);
        ArcEditor.draw_radial(arc, arc.angle(), 0.1f, Color.green);
    }*/
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