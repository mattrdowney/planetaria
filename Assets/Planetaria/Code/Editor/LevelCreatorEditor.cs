using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    // FIXME singleton created only when #if UNITY_EDITOR and created automatically in editor mode for any arbitrary level (ideally)

    [CustomEditor(typeof(LevelCreator))] // TODO: this probably isn't necessary since the other object isn't being used.
    public class LevelCreatorEditor : Editor
    {
        public enum DrawMode
        {
            Free,
            Equilateral,
        }

        /// <summary>
        /// Mutator - draws shapes in the editor; MouseDown creates point at position, MouseUp location determines the slope of the line; Escape finalizes shape.
        /// </summary>
        /// <returns>The next mode for the state machine.</returns>
        public delegate CreateShape CreateShape(bool force_quit);

        private void OnEnable ()
        {
            state_machine = draw_initialize;
            mouse_control = GUIUtility.GetControlID(FocusType.Passive); //UNSURE: use FocusType.Keyboard?
            keyboard_control = GUIUtility.GetControlID(FocusType.Passive);
        }

        private void OnDisable ()
        {
            bool force_quit = true;
            state_machine = state_machine(force_quit);
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
	        debug_rendering = EditorGUILayout.Toggle("Debug Rendering", debug_rendering);
	        GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
	        draw_mode = (DrawMode) EditorGUILayout.EnumPopup("Draw Mode", draw_mode);
	        GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
	        is_field = EditorGUILayout.Toggle("Field", is_field);
	        GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Equator Rows");
	        rows = EditorGUILayout.IntField(rows, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Zone Columns");
	        columns = EditorGUILayout.IntField(columns, GUILayout.Width(50));
	        GUILayout.EndHorizontal();

            switch (draw_mode)
            {
                case DrawMode.Equilateral:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Polygon Sides");
	                edges = EditorGUILayout.IntField(edges, GUILayout.Width(50));
	                GUILayout.EndHorizontal();
                    break;
            }
        }

        private void OnSceneGUI ()
        {
            move_camera();

            center_camera_view();
        
            GridUtility.draw_grid(rows, columns);

            if (EditorWindow.mouseOverWindow == SceneView.currentDrawingSceneView)
            {
                HandleUtility.AddDefaultControl(mouse_control);
                bool escape = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
                state_machine = state_machine(escape);
                use_mouse_event();
            }
            
            Repaint();
        }

        public static CreateShape draw_initialize(bool escape = false)
        {
            switch (draw_mode)
            {
                case DrawMode.Free:
                    return FreeLineDrawMode.draw_first_point;
                case DrawMode.Equilateral:
                    return EquilateralDrawMode.draw_center;
            }
            return draw_initialize;
        }

        /// <summary>
        /// Inspector (quasi-mutator) - locks camera at the origin (so it can't drift).
        /// </summary>
        /// <param name="scene_view">The view that will be locked / "mutated".</param>
        private static void center_camera_view()
	    {
		    GameObject empty_gameobject = new GameObject("origin");
            empty_gameobject.transform.position = Vector3.zero;
            empty_gameobject.transform.rotation = Quaternion.Euler(pitch, yaw, 0);

            SceneView scene_view = SceneView.currentDrawingSceneView;
		    scene_view.AlignViewToObject(empty_gameobject.transform);
		    scene_view.camera.transform.position = Vector3.zero;
		    scene_view.Repaint();

            DestroyImmediate(empty_gameobject);
        }

        public static Vector3 get_mouse_position()
        {
            Vector3 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).direction;
            Vector3 clamped_position = GridUtility.grid_snap(position, rows, columns);

            return clamped_position;
        }

        private static void move_camera()
        {
            HandleUtility.AddDefaultControl(keyboard_control);
            if (Event.current.type == EventType.KeyDown)
            {
                switch(Event.current.keyCode)
                {
                    case KeyCode.W:
                        pitch -= camera_speed;
                        break;
                    case KeyCode.A:
                        yaw -= camera_speed;
                        break;
                    case KeyCode.S:
                        pitch += camera_speed;
                        break;
                    case KeyCode.D:
                        yaw += camera_speed;
                        break;
                }

                switch(Event.current.keyCode)
                {
                    case KeyCode.W: case KeyCode.A: case KeyCode.S: case KeyCode.D:
                        GUIUtility.hotControl = keyboard_control;
                        Event.current.Use();
                        break;
                }

            }
            else if (Event.current.type == EventType.KeyUp)
            {
                switch(Event.current.keyCode)
                {
                    case KeyCode.W: case KeyCode.A: case KeyCode.S: case KeyCode.D:
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        break;
                }
            }
        }

        /// <summary>
        /// Mutator - prevents editor from selecting objects in the editor view (which can de-select the current object)
        /// </summary>
        /// <param name="editor">The current level editor.</param>
        private static void use_mouse_event()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                GUIUtility.hotControl = mouse_control;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
            }
        }

        [MenuItem("Planetaria/Debug Rendering #g")]
        private static void toggle_debug_rendering()
        {
            debug_rendering = !debug_rendering; // toggle state
        }

        [Tooltip("")]
        public static bool debug_rendering = true;

        [Tooltip("")]
        public static DrawMode draw_mode;

        [Tooltip("")]
        public static bool is_field;

        /// <summary>Number of rows in the grid. Equator is drawn when rows is odd</summary>
        [Tooltip("")]
        public static int rows = 63;
        
        /// <summary>Number of columns in the grid (per hemisphere).</summary>
        [Tooltip("")]
	    public static int columns = 64;

        [Tooltip("")]
        public static int edges = 3;

        private static CreateShape state_machine;

        private static float camera_speed = 5f;

        private static float yaw = 0;
        private static float pitch = 0;

        private static int mouse_control;
        private static int keyboard_control;
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