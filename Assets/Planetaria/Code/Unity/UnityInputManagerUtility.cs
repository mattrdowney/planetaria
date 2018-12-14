#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    /// <summary>
	/// Hooks into UnityEngine.InputManager
    /// </summary>
    /// <see cref="http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html"/> // almost verbatim from this source
	public class UnityInputManagerUtility
	{
        private static void add_keyboard_axes(KeyCode up, KeyCode left, KeyCode down, KeyCode right)
        {
            string axis_name = up.ToString() + left.ToString() + down.ToString() + right.ToString() + "Axis";
            add_axis(new UnityInputAxis() { name = horizontal, descriptive_name = axis_name, positive_button = right, negative_button = left,
                    sensitivity = 20f, gravity = 20f, type = UnityAxisType.KeyOrMouseButton, axis = UnityAxisIdentity.XAxis });
            add_axis(new UnityInputAxis() { name = vertical, descriptive_name = axis_name, positive_button = up, negative_button = down,
                    sensitivity = 20f, gravity = 20f, type = UnityAxisType.KeyOrMouseButton, axis = UnityAxisIdentity.XAxis });
        }

        private static void add_horizontal_joystick_axis(UnityAxisIdentity horizontal_axis)
        {
            string axis_name = "Joystick0" + horizontal_axis.ToString();
            add_axis(new UnityInputAxis() { name = horizontal, descriptive_name = axis_name, joystick = UnityJoystickIdentity.JoystickAll,
                    sensitivity = 1, gravity = 0, dead = 0.001f, type = UnityAxisType.JoystickAxis, axis = horizontal_axis });
        }

        private static void add_vertical_joystick_axis(UnityAxisIdentity vertical_axis)
        {
            string axis_name = "Joystick0" + vertical_axis.ToString();
            add_axis(new UnityInputAxis() { name = vertical, descriptive_name = axis_name, joystick = UnityJoystickIdentity.JoystickAll,
                    sensitivity = 1, gravity = 0, dead = 0.001f, type = UnityAxisType.JoystickAxis, axis = vertical_axis, invert = true });
        }

        [MenuItem("Planetaria/Generate Planetaria Input Manager Controls")]
        public static void setup_planetaria_inputs()
        {
            // Add mouse definitions
            //add_axis(new UnityInputAxis() { name = horizontal, descriptive_name = "MouseX", sensitivity = 1f, type = UnityAxisType.MouseMovement, axis = UnityAxisIdentity.XAxis });
            //add_axis(new UnityInputAxis() { name = vertical, descriptive_name = "MouseY", sensitivity = 1f, type = UnityAxisType.MouseMovement, axis = UnityAxisIdentity.YAxis });
            //add_axis(new UnityInputAxis() { name = vertical, descriptive_name = "ScrollWheel", sensitivity = 1f, type = UnityAxisType.MouseMovement, axis = UnityAxisIdentity.Axis3 });

            // add common keyboard layouts
            add_keyboard_axes(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
            add_keyboard_axes(KeyCode.T, KeyCode.F, KeyCode.G, KeyCode.H);
            add_keyboard_axes(KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L);
            add_keyboard_axes(KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow);
            add_keyboard_axes(KeyCode.Keypad8, KeyCode.Keypad4, KeyCode.Keypad2, KeyCode.Keypad6);

            // add common controller buttons
            add_keyboard_axes(KeyCode.JoystickButton3, KeyCode.JoystickButton2, KeyCode.JoystickButton0, KeyCode.JoystickButton1);
            add_keyboard_axes(KeyCode.JoystickButton19, KeyCode.JoystickButton18, KeyCode.JoystickButton16, KeyCode.JoystickButton17);

            // add common controller dpad
            add_keyboard_axes(KeyCode.JoystickButton5, KeyCode.JoystickButton7, KeyCode.JoystickButton6, KeyCode.JoystickButton8);
            add_keyboard_axes(KeyCode.JoystickButton13, KeyCode.JoystickButton11, KeyCode.JoystickButton14, KeyCode.JoystickButton12);

            // Add joystick axes
            add_horizontal_joystick_axis(UnityAxisIdentity.XAxis);
            add_vertical_joystick_axis(UnityAxisIdentity.YAxis);
            add_vertical_joystick_axis(UnityAxisIdentity.Axis3); // HACK: due to design constraints (certain controllers overlap) - add third axis (even if 3/4 are rotated on some controllers)
            add_horizontal_joystick_axis(UnityAxisIdentity.Axis4);
            add_vertical_joystick_axis(UnityAxisIdentity.Axis5);
            add_vertical_joystick_axis(UnityAxisIdentity.Axis6);
            add_horizontal_joystick_axis(UnityAxisIdentity.Axis6);
            add_vertical_joystick_axis(UnityAxisIdentity.Axis7);
            add_horizontal_joystick_axis(UnityAxisIdentity.Axis8); // HACK: due to design constraints (certain controllers overlap) - add third axis (even if 7/8 are rotated on some controllers)
        }
        private static SerializedProperty get_child_property(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name)
                {
                    return child;
                }
            }
            while (child.Next(false));
            return null;
        }

        private static bool is_axis_defined(string axis_name, string axis_description)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axis_iterator = serializedObject.FindProperty("m_Axes");
            axis_iterator.Next(true);
            axis_iterator.Next(true);

            while (axis_iterator.Next(false))
            {
                SerializedProperty axis_properties = axis_iterator.Copy();
                if (get_child_property(axis_iterator, "m_Name").stringValue == axis_name)
                {
                    if (get_child_property(axis_iterator, "descriptiveName").stringValue == axis_description)
                    {
                        return true; // when both match, the axis is defined already
                    }
                }
            }
            return false;
        }

        private static void add_axis(UnityInputAxis axis) // TODO: in-place edit if axis exists
        {
            if (is_axis_defined(axis.name, axis.descriptive_name))
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize += 1;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            get_child_property(axisProperty, "m_Name").stringValue = axis.name;
            get_child_property(axisProperty, "descriptiveName").stringValue = axis.descriptive_name;
            get_child_property(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptive_negative_name;
            get_child_property(axisProperty, "negativeButton").stringValue = convert_keycode(axis.negative_button);
            get_child_property(axisProperty, "positiveButton").stringValue = convert_keycode(axis.positive_button);
            get_child_property(axisProperty, "altNegativeButton").stringValue = convert_keycode(axis.alternate_negative_button);
            get_child_property(axisProperty, "altPositiveButton").stringValue = convert_keycode(axis.alternate_positive_button);
            get_child_property(axisProperty, "gravity").floatValue = axis.gravity;
            get_child_property(axisProperty, "dead").floatValue = axis.dead;
            get_child_property(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            get_child_property(axisProperty, "snap").boolValue = axis.snap;
            get_child_property(axisProperty, "invert").boolValue = axis.invert;
            get_child_property(axisProperty, "type").intValue = (int)axis.type;
            get_child_property(axisProperty, "axis").intValue = (int)axis.axis - 1; // internal Unity quirk
            get_child_property(axisProperty, "joyNum").intValue = (int)axis.joystick;

            serializedObject.ApplyModifiedProperties();
        }

        public static string convert_keycode(KeyCode key) // useful table (but it's ~600 lines): https://answers.unity.com/questions/1388346/keypad-enter-in-the-input-manager.html
        {
            string default_result = key.ToString(); // CONSIDER: make this work for all keycodes?
            string result = "";
            bool digit_found = false;
            foreach (char letter in default_result)
            {
                if (char.IsUpper(letter))
                {
                    result += " ";
                }
                else if (char.IsDigit(letter) && !digit_found)
                {
                    result += " ";
                    digit_found = true;
                }
                result += char.ToLower(letter);
            }
            if (result.StartsWith(" "))
            {
                result = result.Substring(1);
            }
            if (result.EndsWith(" arrow")) // for left, right, up, down arrow keys
            {
                result = result.Substring(0, result.Length - " arrow".Length);
            }
            else if (result.StartsWith("keypad "))
            {
                result = "[" + result.Substring("keypad ".Length) + "]";
            }
            return result;
        }
        
        private const string horizontal = "PlanetariaUniversalInputHorizontal";
        private const string vertical = "PlanetariaUniversalInputVertical";
	}
}

#endif

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