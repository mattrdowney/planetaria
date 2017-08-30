using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    float yaw;
    float pitch;

    int control_identifier;

    void Start ()
    {
        yaw = pitch = 0;
        control_identifier = GUIUtility.GetControlID(FocusType.Passive);
    }

    void OnSceneGUI ()
    {
        center_camera_view(SceneView.currentDrawingSceneView);

        /// button_down 1: create equatorial circle at point.
        /// button_up 1: create right-hand-side slope for point 1. 
        /// button_down 2-n: create arc through point using last right-hand-side slope; adjust previous slope (if neccessary) so that it meets with current point (best fit).
        /// button_up 2-n: create right-hand-side slope for current point.
        /// escape: adjust final slope (if neccessary) so that it meets with first point (best fit).
        /// NOTE: escape should be disabled until mouse_up happens.
        if (Event.current.type == EventType.MouseDown)
        {
            Ray editor_camera_ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            GameObject prefabricated_object = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Planetaria/PrefabricatedObjects/Resources/Sphere.prefab") as GameObject;
            GameObject spawned_object = PrefabUtility.InstantiatePrefab(prefabricated_object) as GameObject;
            spawned_object.transform.position = editor_camera_ray.direction;

            GUIUtility.hotControl = control_identifier;

            EditorUtility.SetDirty(spawned_object);
            Event.current.Use();
        }
    }

    public void center_camera_view(SceneView scene_view)
	{
		GameObject empty_gameobject = new GameObject("origin");
        empty_gameobject.transform.position = Vector3.zero;
        empty_gameobject.transform.rotation = Quaternion.Euler(yaw, pitch, 0);

		scene_view.AlignViewToObject(empty_gameobject.transform);
		scene_view.camera.transform.position = Vector3.zero;
		scene_view.Repaint();

        GameObject.DestroyImmediate(empty_gameobject);
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