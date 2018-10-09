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
            EditorGlobal.self.hide_graphics = EditorGUILayout.Toggle("Debug Rendering", EditorGlobal.self.hide_graphics);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGlobal.self.show_inspector = EditorGUILayout.Toggle("Hidden Inspector", EditorGlobal.self.show_inspector);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
	        draw_mode = (DrawMode) EditorGUILayout.EnumPopup("Draw Mode", draw_mode);
	        GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
	        is_field = EditorGUILayout.Toggle("Field", is_field);
	        GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            allow_self_intersections = EditorGUILayout.Toggle("Allow self-intersections", allow_self_intersections);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Equator Rows");
	        EditorGlobal.self.rows = EditorGUILayout.IntField(EditorGlobal.self.rows, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Time Zone Columns");
            EditorGlobal.self.columns = EditorGUILayout.IntField(EditorGlobal.self.columns, GUILayout.Width(50));
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
            SnapUtility.draw_grid();
            if (EditorWindow.mouseOverWindow == SceneView.currentDrawingSceneView)
            {
                HandleUtility.AddDefaultControl(mouse_control);
                bool escape = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
                capture_v_press();
                state_machine = state_machine(escape);
                use_mouse_event();
            }
            Repaint();
        }

        public static void capture_v_press()
        {
            switch (Event.current.type)
            {
                case EventType.KeyDown:
                    if (Event.current.keyCode == (KeyCode.V))
                    {
                        EditorGlobal.self.v_pressed = true;
                        GUIUtility.hotControl = keyboard_control;
                        Event.current.Use();
                    }
                    break;
                case EventType.KeyUp:
                    if (Event.current.keyCode == (KeyCode.V))
                    {
                        EditorGlobal.self.v_pressed = false;
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
            }
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
            Vector3 adjusted_position = SnapUtility.snap(position, false); // FIXME: BUG: HACK: TODO:

            return adjusted_position;
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

        [MenuItem("Planetaria/Toggle Debug Graphics #g")]
        private static void toggle_debug_graphics() // (Shift + g(raphics))
        {
            EditorGlobal.self.hide_graphics = !EditorGlobal.self.hide_graphics; // toggle state
        }

        [MenuItem("Planetaria/Toggle Hidden Inspector #i")]
        private static void toggle_hidden_inspector() // (Shift + i(nspector))
        {
            EditorGlobal.self.show_inspector = !EditorGlobal.self.show_inspector; // toggle state
            foreach (GameObject game_object in GameObject.FindObjectsOfType<GameObject>())
            {
                // Hide planetaria internals (GitHub Issue #43 and #75).
                // Toggling the inspector shows these objects (Shift + i(nspector))
                if (game_object.name.Substring(0, 2) == "__") // double underscore indicates hidden object
                {
                    if (!EditorGlobal.self.show_inspector)
                    {
                        game_object.hideFlags |= (HideFlags.HideInHierarchy | HideFlags.HideInInspector); // set
                    }
                    else
                    {
                        game_object.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector); // unset
                    }
                }
                EditorUtility.SetDirty(game_object);
            }
        }

        [Tooltip("")]
        public static DrawMode draw_mode;

        [Tooltip("")]
        public static bool is_field;

        [Tooltip("")]
        public static bool allow_self_intersections;



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

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.