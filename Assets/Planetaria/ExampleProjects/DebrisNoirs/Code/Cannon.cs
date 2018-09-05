using UnityEngine;
using Planetaria;

public class Cannon : PlanetariaComponent
{
    public GameObject prefabricated_object;

    private void Start()
    {
        main_character = GameObject.FindObjectOfType<Ship>().gameObject.internal_game_object.transform;
        main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
#if UNITY_EDITOR
        GameObject.FindObjectOfType<PlanetariaActuator>().input_device_type = PlanetariaActuator.InputDevice.Mouse;
#else
        GameObject.FindObjectOfType<PlanetariaActuator>().input_device_type = PlanetariaActuator.InputDevice.Gyroscope;
#endif
    }

    private void Update()
    {
        Vector3 character_position = main_character.forward;
        Vector3 controller_position = main_controller.forward;
        Vector3 bullet_direction = Bearing.attractor(character_position, controller_position);

        bool firing;
#if UNITY_EDITOR
        firing = Input.GetButtonDown("Fire1");
#else
        firing = Input.GetAxisRaw("OSVR_IndexTrigger") > .9f;
#endif
        if (firing)
        {
            PlanetariaGameObject.Instantiate(prefabricated_object, character_position, bullet_direction);
        }
    }

    private void LateUpdate()
    {
        Vector3 character_position = main_character.forward;
        Vector3 controller_position = main_controller.forward;
        Vector3 crosshair_up = Bearing.repeller(controller_position, character_position);
        main_controller.rotation = Quaternion.LookRotation(controller_position, crosshair_up);
    }

    private Transform main_character;
    private Transform main_controller;
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