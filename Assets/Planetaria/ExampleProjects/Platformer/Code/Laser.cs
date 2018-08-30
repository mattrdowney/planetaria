using System.Collections.Generic;
using UnityEngine;
using Planetaria;

public class Laser : MonoBehaviour  // TODO: PlanetariaComponent
{
    public GameObject prefabricated_object;

    private void Start()
    {
        main_character = GameObject.FindObjectOfType<Character>().gameObject.internal_game_object.transform;
        main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
        arc_renderer = GameObject.FindObjectOfType<ArcRenderer>();
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
        List<GeospatialCurve> laser = new List<GeospatialCurve>
        {
            GeospatialCurve.curve(character_position, controller_position),
            GeospatialCurve.curve(controller_position, character_position),
        };
        arc_renderer.shape = new Shape(laser, false, false);
        arc_renderer.recalculate();

#if UNITY_EDITOR
        if (Input.GetButton("Fire1"))
        {
            arc_renderer.material.color = Color.red;
        }
        else
        {
            arc_renderer.material.color = Color.blue;
        }
#else
        if (Input.GetAxis("OSVR_IndexTrigger") > .9f)
        {
            arc_renderer.material.color = Color.red;
        }
        else
        {
            arc_renderer.material.color = Color.blue;
        }
#endif

        /*PlanetariaRaycastHit[] collision_info = PlanetariaPhysics.raycast_all(Arc.line(character_position, controller_position));
        Vector3 last_position = character_position;
        bool blue = true;
        Color color;
        foreach (PlanetariaRaycastHit hit in collision_info)
        {
            color = blue ? Color.blue : Color.red;
            Debug.DrawLine(last_position, hit.point, color);
            last_position = hit.point;
            blue = !blue;
        }
        color = blue ? Color.blue : Color.red;
        Debug.DrawLine(last_position, controller_position, color);
        arc_renderer.*/

        /*if (Input.GetButtonDown("Jump"))
        {
            PlanetariaGameObject.Instantiate(prefabricated_object, controller_position);
            //Destroy(new object, 3 seconds) + test Destroy for PlanetariaGameObject
        }*/
    }
    
    private Transform main_character;
    private Transform main_controller;
    private ArcRenderer arc_renderer;
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