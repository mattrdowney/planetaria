using UnityEngine;
using UnityEngine.XR;

namespace Planetaria
{
    public sealed class PlanetariaActuator : PlanetariaComponent // while this is not a Unity class, I have a hypothesis that something similar will be implemented.
    {
        public enum InputDevice { DualAxis, Gyroscope, Mouse, Touchpad, Touchscreen };

        // TODO: relativity (e.g. for joysticks)

        protected override sealed void Awake()
        {
            base.Awake();
            internal_camera = GameObject.FindObjectOfType<Camera>();
            camera_transform = internal_camera.GetComponent<Transform>();
            internal_transform = gameObject.internal_game_object.GetComponent<Transform>();
        }

        protected override void OnDestroy() { }

        private void Update()
        {
            step();
        }

        private void LateUpdate() // This is for graphics that require correct positioning of controller (e.g. crosshair)
        {
            step();
        }

        private void step()
        {
            switch (input_device_type)
            {
                // Analog stick (e.g. for conventional console controllers)
                case InputDevice.DualAxis:
                    Debug.LogError("Not Implemented"); // TODO: implement
                    break;
                // Motion controllers (e.g. virtual reality)
                case InputDevice.Gyroscope:
                    internal_transform.rotation = camera_transform.rotation * InputTracking.GetLocalRotation(XRNode.RightHand);
                    break;
                // Computer mouse
                case InputDevice.Mouse:
                    internal_transform.rotation = Quaternion.LookRotation(internal_camera.ScreenPointToRay(Input.mousePosition).direction, camera_transform.up);
                    break;
                // Touch devices (e.g. laptops and tablets)
                case InputDevice.Touchpad:
                    Debug.LogError("Not Implemented"); // TODO: implement
                    break;
                // Touch devices (e.g. laptops and tablets)
                case InputDevice.Touchscreen:
                    if (Input.touches.Length > 0)
                    {
                        internal_transform.rotation = Quaternion.LookRotation(internal_camera.ScreenPointToRay(Input.touches[0].position).direction, camera_transform.up);
                    }
                    break;
            }
        }

        public InputDevice input_device_type = InputDevice.Gyroscope;
        private Camera internal_camera;
        private Transform camera_transform;
        private Transform internal_transform;
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