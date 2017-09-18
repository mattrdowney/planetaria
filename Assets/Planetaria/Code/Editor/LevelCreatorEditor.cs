using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))] // TODO: this probably isn't necessary since the other object isn't being used.
public class LevelCreatorEditor : Editor
{
    /// <summary>
    /// Mutator - draws shapes in the editor; MouseDown creates point at position, MouseUp location determines the slope of the line; Escape finalizes shape.
    /// </summary>
    /// <returns>The next mode for the state machine.</returns>
    private delegate CreateShape CreateShape();

    private void OnEnable ()
    {
        yaw = pitch = 0;
        state_machine = draw_first_point;
        control_identifier = GUIUtility.GetControlID(FocusType.Passive);
        start_shape();
    }

    private void OnDisable ()
    {
        end_shape();
        Repaint();
    }

    private void OnSceneGUI ()
    {
        center_camera_view();
        
        GridUtility.draw_grid(rows, columns);

        if (EditorWindow.mouseOverWindow == SceneView.currentDrawingSceneView)
        {
            HandleUtility.AddDefaultControl(control_identifier);

            state_machine = state_machine();

            use_mouse_event();
        }
            
        Repaint();
    }

    /// <summary>
    /// Inspector (quasi-mutator) - locks camera at the origin (so it can't drift).
    /// </summary>
    /// <param name="scene_view">The view that will be locked / "mutated".</param>
    private static void center_camera_view()
	{
		GameObject empty_gameobject = new GameObject("origin");
        empty_gameobject.transform.position = Vector3.zero;
        empty_gameobject.transform.rotation = Quaternion.Euler(yaw, pitch, 0);

        SceneView scene_view = SceneView.currentDrawingSceneView;
		scene_view.AlignViewToObject(empty_gameobject.transform);
		scene_view.camera.transform.position = Vector3.zero;
		scene_view.Repaint();

        GameObject.DestroyImmediate(empty_gameobject);
    }

    private static Vector3 get_mouse_position()
    {
        Vector3 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).direction;
        Vector3 clamped_position = GridUtility.grid_snap(position, rows, columns);

        return clamped_position;
    }

    private static CreateShape draw_first_point()
    {
        temporary_arc.from = get_mouse_position();

        // MouseDown 1: create first corner of a shape
        if (Event.current.type == EventType.MouseDown)
        {
            temporary_arc.advance();
            return draw_tangent;
        }

        return draw_first_point;
    }

    /// <summary>
    /// Mutator - MouseUp creates slope at position
    /// </summary>
    /// <param name="editor">The reference to the LevelCreatorEditor object that stores shape information.</param>
    /// <returns>mouse_up if nothing was pressed; mouse_down if MouseUp or Escape was pressed.</returns>
    private static CreateShape draw_tangent()
    {
        temporary_arc.from_tangent = get_mouse_position();

        // MouseUp 1-n: create the right-hand-side slope for the last point added
        if (Event.current.type == EventType.MouseUp)
        {
            temporary_arc.advance();
            return draw_nth_point;
        }
        // Escape: close the shape so that it meets with the original point (using original point for slope)
        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            end_shape();
            start_shape();
            return draw_first_point;
        }

        return draw_tangent;
    }

    /// <summary>
    /// Mutator - MouseDown creates point at position
    /// </summary>
    /// <returns>The next mode for the state machine.</returns>
    private static CreateShape draw_nth_point()
    {
        temporary_arc.to = get_mouse_position();
        
        // MouseDown 2-n: create arc through point using last right-hand-side slope
        if (Event.current.type == EventType.MouseDown)
        {
            block.add(temporary_arc.arc.data);
            EditorUtility.SetDirty(block.gameObject);

            temporary_arc.finalize_edge();
            return draw_tangent;
        }
        // Escape: close the shape so that it meets with the original point
        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
        {
            end_shape();
            start_shape();
            return draw_first_point;
        }

        return draw_nth_point;
    }

    /// <summary>
    /// Mutator - prevents editor from selecting objects in the editor view (which can de-select the current object)
    /// </summary>
    /// <param name="editor">The current level editor.</param>
    private static void use_mouse_event()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            GUIUtility.hotControl = control_identifier;
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
            Event.current.Use();
        }
    }

    private static void start_shape()
    {
        GameObject arc_builder = new GameObject("Arc builder");
        temporary_arc = arc_builder.AddComponent<ArcBuilder>();

        GameObject shape = Block.block();
        block = shape.GetComponent<Block>() as Block;
    }

    private static void end_shape()
    {
        temporary_arc.close_shape();

        if (temporary_arc.arc.exists)
        {
            block.add(temporary_arc.arc.data);
            EditorUtility.SetDirty(block.gameObject);
        }

        GameObject arc_builder = temporary_arc.gameObject;
        GameObject.DestroyImmediate(arc_builder);

        if (block.size() == 0)
        {
            GameObject shape = block.gameObject;
            GameObject.DestroyImmediate(shape);
        }

        temporary_arc = null;
        block = null;
    }
    
    /// <summary>Number of rows in the grid. Equator is drawn when rows is odd</summary>
    public static int rows = 15;
    /// <summary>Number of columns in the grid (per hemisphere).</summary>
	public static int columns = 16;

    private static CreateShape state_machine;

    private static Block block;
    private static ArcBuilder temporary_arc;

    private static float yaw;
    private static float pitch;

    private static int control_identifier;
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