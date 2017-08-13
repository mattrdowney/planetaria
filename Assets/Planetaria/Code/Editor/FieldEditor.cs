using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Field))]
public class FieldEditor : Editor
{
    Field field;
    PlanetariaTransform transform; // TODO: make fields relative (for moving fields)

    void Awake()
    {
        List<Plane> plane_list = field.get_plane_list();

        //for all planes...
        //  PlanetariaIntersection.circle_circle_intersection(
        //  add to container of field arcs
    }

    void OnSceneGUI()
    {
        //for all field arcs
        //  RendererFacilities.draw_arc(
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