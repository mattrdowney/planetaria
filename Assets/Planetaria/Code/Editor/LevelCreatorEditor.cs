using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    /// <summary>
    /// Mutator - draws shapes in the editor; MouseDown creates point at position, MouseUp location determines the slope of the line; Escape finalizes shape.
    /// </summary>
    /// <param name="editor">The reference to the LevelCreatorEditor object that stores shape information.</param>
    /// <returns>The next mode for the state machine.</returns>
    delegate CreateShape CreateShape(LevelCreatorEditor editor);
    CreateShape state_machine;

    Block block;

    Vector3 start;
    Vector3 right;
    Vector3 end;

    float yaw;
    float pitch;

    int control_identifier;

    void OnEnable ()
    {
        Debug.Log("Enable");
        yaw = pitch = 0;
        control_identifier = GUIUtility.GetControlID(FocusType.Passive);
        start = right = end = Vector3.zero;
        state_machine = wait_mouse_event;

        GameObject shape = Block.CreateBlock();
        block = shape.GetComponent<Block>();
    }

    void OnDisable ()
    {
        Debug.Log("Disable");
    }

    void OnSceneGUI ()
    {
        Debug.Log("GUI");

        center_camera_view(SceneView.currentDrawingSceneView);

        state_machine = state_machine(this);
    }

    /// <summary>
    /// Inspector (quasi-mutator) - locks camera at the origin (so it can't drift).
    /// </summary>
    /// <param name="scene_view">The view that will be locked / "mutated".</param>
    void center_camera_view(SceneView scene_view)
	{
		GameObject empty_gameobject = new GameObject("origin");
        empty_gameobject.transform.position = Vector3.zero;
        empty_gameobject.transform.rotation = Quaternion.Euler(yaw, pitch, 0);

		scene_view.AlignViewToObject(empty_gameobject.transform);
		scene_view.camera.transform.position = Vector3.zero;
		scene_view.Repaint();

        GameObject.DestroyImmediate(empty_gameobject);
    }

    /// <summary>
    /// Mutator - MouseDown creates point at position
    /// </summary>
    /// <param name="editor">The reference to the LevelCreatorEditor object that stores shape information.</param>
    /// <returns>The next mode for the state machine.</returns>
    static CreateShape mouse_down(LevelCreatorEditor editor)
    {
        Debug.Log("MouseDown");
        /// button_down 1: create equatorial circle at point.
        /// button_down 2-n: create arc through point using last right-hand-side slope; adjust previous slope (if neccessary) so that it meets with current point (best fit).
        /// NOTE: MouseUp should be diabled until mouse_down happens.
        /// NOTE: Escape should be disabled until mouse_up happens.
        if (Event.current.type == EventType.MouseDown)
        {
            editor.end = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).direction;

            if (editor.start != Vector3.zero)
            {
                Arc arc = Arc.CreateArc(editor.start, editor.right, editor.end);

                Debug.Log(arc);

                editor.block.Add(arc);

                EditorUtility.SetDirty(editor.block.gameObject);
            }

            editor.start = editor.end;

            use_mouse_event(editor);

            return mouse_up;
        }
        /// escape: adjust final slope (if neccessary) so that it meets with first point (best fit).
        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            //FIXME:

            //GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Keyboard);

            return wait_mouse_event;
        }

        return mouse_down;
    }

    /// <summary>
    /// Mutator - MouseUp creates slope at position
    /// </summary>
    /// <param name="editor">The reference to the LevelCreatorEditor object that stores shape information.</param>
    /// <returns>mouse_up if nothing was pressed; mouse_down if MouseUp or Escape was pressed.</returns>
    static CreateShape mouse_up(LevelCreatorEditor editor)
    {
        Debug.Log("MouseUp");

        /// button_up 1: create right-hand-side slope for point 1.
        /// button_up 2-n: create right-hand-side slope for current point.
        if (Event.current.type == EventType.MouseUp)
        {
            Vector3 slope_endpoint = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).direction;
            editor.right = (slope_endpoint - editor.start).normalized;

            use_mouse_event(editor);

            return mouse_down;
        }

        return mouse_up;
    }

    /// <summary>
    /// Mutator - prevents editor from selecting objects in the editor view (which can de-select the current object)
    /// </summary>
    /// <param name="editor">The current level editor.</param>
    static void use_mouse_event(LevelCreatorEditor editor)
    {
        GUIUtility.hotControl = editor.control_identifier;
        Event.current.Use();
    }

    static CreateShape wait_mouse_event(LevelCreatorEditor editor)
    {
        Debug.Log("Wait");

        if (Event.current.type == EventType.MouseDown)
        {
            use_mouse_event(editor);
        }

        return mouse_down;
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