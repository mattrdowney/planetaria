using System;
using UnityEngine;
using Planetaria;

public class Crosshair : PlanetariaComponent
{
    private void Start()
    {
        main_character = GameObject.FindObjectOfType<Ship>().gameObject.internal_game_object.transform;
        main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
        planetaria_transform = this.GetComponent<PlanetariaTransform>();
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
        planetaria_transform.direction = crosshair_up;
    }

    private Transform main_character;
    private Transform main_controller;
    private PlanetariaTransform planetaria_transform;
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