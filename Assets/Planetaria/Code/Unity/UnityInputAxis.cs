using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// As used in UnityEngine.InputManager
    /// </summary>
    /// <seealso cref="http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html"/>
    public enum UnityAxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2,
    };

    /// <summary>
    /// As defined in UnityEngine.InputManager
    /// </summary>
    /// <seealso cref="http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html"/>
    public class UnityInputAxis
    {
        public string name;
        public string descriptive_name;
        public string descriptive_negative_name;
        public KeyCode negative_button;
        public KeyCode positive_button;
        public KeyCode alternate_negative_button;
        public KeyCode alternate_positive_button;

        public float gravity;
        public float dead;
        public float sensitivity;

        public bool snap = false;
        public bool invert = false;

        public UnityAxisType type;

        public UnityAxisIdentity axis;
        public UnityJoystickIdentity joystick;
    }

    public enum UnityAxisIdentity
    {
        XAxis = 1,
        YAxis = 2,
        Axis3 = 3,
        Axis4 = 4,
        Axis5 = 5,
        Axis6 = 6,
        Axis7 = 7,
        Axis8 = 8,
        Axis9 = 9,
        Axis10 = 10,
        Axis11 = 11,
        Axis12 = 12,
        Axis13 = 13,
        Axis14 = 14,
        Axis15 = 15,
        Axis16 = 16,
        Axis17 = 17,
        Axis18 = 18,
        Axis19 = 19,
        Axis20 = 20,
        Axis21 = 21,
        Axis22 = 22,
        Axis23 = 23,
        Axis24 = 24,
        Axis25 = 25,
        Axis26 = 26,
        Axis27 = 27,
        Axis28 = 28,
    };

    public enum UnityJoystickIdentity
    {
        JoystickAll = 0,
        Joystick1 = 1,
        Joystick2 = 2,
        Joystick3 = 3,
        Joystick4 = 4,
        Joystick5 = 5,
        Joystick6 = 6,
        Joystick7 = 7,
        Joystick8 = 8,
        Joystick9 = 9,
        Joystick10 = 10,
        Joystick11 = 11,
    };

    public enum UnityJoystickButtonIdentity
    {
        JoystickButton0 = 0,
        JoystickButton1 = 1,
        JoystickButton2 = 2,
        JoystickButton3 = 3,
        JoystickButton4 = 4,
        JoystickButton5 = 5,
        JoystickButton6 = 6,
        JoystickButton7 = 7,
        JoystickButton8 = 8,
        JoystickButton9 = 9,
        JoystickButton10 = 10,
        JoystickButton11 = 11,
        JoystickButton12 = 12,
        JoystickButton13 = 13,
        JoystickButton14 = 14,
        JoystickButton15 = 15,
        JoystickButton16 = 16,
        JoystickButton17 = 17,
        JoystickButton18 = 18,
        JoystickButton19 = 19,
    };
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