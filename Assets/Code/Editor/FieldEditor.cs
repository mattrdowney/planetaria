using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Field))]
public class FieldEditor : Editor
{
    Field field;
    PlanetariaTransform transform; // TODO: make zones relative (for moving fields)

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
