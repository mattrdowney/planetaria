using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    public class Crosshair : PlanetariaMonoBehaviour
    {
        private void OnValidate()
        {
            main_controller = this.GetComponent<PlanetariaActuator>().gameObject.internal_game_object.transform;
            planetaria_transform = this.GetComponent<PlanetariaTransform>();
            crosshair_renderer = this.GetComponent<AreaRenderer>();
        }

        private void Start()
        {
            satellite = GameObject.FindObjectOfType<Satellite>();
            main_character = GameObject.FindObjectOfType<Satellite>().gameObject.transform;
#if UNITY_EDITOR
            GameObject.FindObjectOfType<PlanetariaActuator>().input_device_type = PlanetariaActuator.InputDevice.Mouse;
#else
            GameObject.FindObjectOfType<PlanetariaActuator>().input_device_type = PlanetariaActuator.InputDevice.Gyroscope;
            GameObject.FindObjectOfType<PlanetariaActuator>().virtual_reality_tracker_type = UnityEngine.XR.XRNode.Head;
#endif
        }

        private void LateUpdate()
        {
            Vector3 character_position = main_character.forward;
            Vector3 controller_position = main_controller.forward;
            Vector3 crosshair_up = Bearing.repeller(controller_position, character_position);

            crosshair_renderer.angle = Vector3.SignedAngle(main_controller.up, crosshair_up, main_controller.forward) * Mathf.Deg2Rad;
            crosshair_renderer.scale = Mathf.Lerp(crosshair_size/2, crosshair_size, satellite.acceleration_direction().magnitude);
        }

        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        [SerializeField] [HideInInspector] private Satellite satellite;
        [SerializeField] [HideInInspector] private Transform main_character;
        [SerializeField] [HideInInspector] private Transform main_controller;
        [SerializeField] [HideInInspector] private PlanetariaTransform planetaria_transform;
        [SerializeField] [HideInInspector] private AreaRenderer crosshair_renderer;

        private const float crosshair_size = 0.03f; // same as satellite size
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