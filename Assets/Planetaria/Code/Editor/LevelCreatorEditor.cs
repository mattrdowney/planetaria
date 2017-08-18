using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    void OnSceneGUI ()
    {
        Align(SceneView.currentDrawingSceneView);

        if (Event.current.type == EventType.MouseDown)
        {
            Ray editor_camera_ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            GameObject prefabricated_object = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Planetaria/PrefabricatedObjects/Resources/Sphere.prefab") as GameObject;
            GameObject spawned_object = PrefabUtility.InstantiatePrefab(prefabricated_object) as GameObject;
            spawned_object.transform.position = editor_camera_ray.direction;

            EditorUtility.SetDirty(spawned_object);
            Event.current.Use();
        }
    }

    public static void Align(SceneView scene_view)
	{
		GameObject empty_gameobject = new GameObject("Alignment");

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