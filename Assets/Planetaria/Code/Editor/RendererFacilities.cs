using UnityEngine;

public static class RendererFacilities
{
    /// <summary>
    /// Inspector - Draws an arc on the surface of a unit sphere.
    /// </summary>
    /// <param name="from">The starting point of the arc.</param>
    /// <param name="to">The ending point of the arc.</param>
    /// <param name="normal">The cutting normal that defines the circle that defines the arc.</param>
    /// <param name="angle">The angle (used to determine if the arc is above or below 180 degrees.</param>
    /// <param name="color">The color of the drawn arc.</param>
    public static void draw_arc(Vector3 from, Vector3 to, Vector3 normal, float angle, Color color)
    {
        if (from == normal) // If radius is zero
        {
            return; // there is no need to render since the arc is an infintessimally small point
        }

        Vector3 center = Vector3.Project(from, normal);

        float radius = (from - center).magnitude;

        from = (from - center).normalized;
        to = (to - center).normalized;

        Vector3 forward = from;
        Vector3 right = -Vector3.Cross(forward, normal);

        if (angle <= Mathf.PI)
        {
            angle *= -1;
        }

        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawWireArc(center, normal, from, angle, radius);
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