using System;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    public class Railgun : PlanetariaMonoBehaviour
    {
        public GameObject prefabricated_object;

        private void Start()
        {
            main_character = GameObject.FindObjectOfType<Satellite>().gameObject.internal_game_object.transform;
            main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            spotlight = GameObject.FindObjectOfType<Satellite>().transform.Find("Spotlight").gameObject.internal_game_object.transform;
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

            bool firing = Input.GetButtonDown("PlanetariaUniversalInputButton");
            // If a button is held, then continue firing every x milleseconds.

            spotlight.localRotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(main_character.up, bullet_direction, main_character.forward));
            if (firing)
            {
                optional<PlanetariaRaycastHit> raycast_information =
                        PlanetariaPhysics.raycast(ArcFactory.line(character_position, bullet_direction),
                        Mathf.PI, 1 << LayerMask.NameToLayer("Debris"), QueryTriggerInteraction.Collide);

                if (raycast_information.exists)
                {
                    Debug.Log(raycast_information.data.collider.name);
                    //raycast_information.data.collider.gameObject.GetComponent<LargeDebris>().destroy_asteroid();
                }
                else
                {
                    Debug.Log("Hit nothing");
                }
                //PlanetariaGameObject.Instantiate(prefabricated_object, character_position, bullet_direction);
            }
        }

        private void LateUpdate()
        {
            Vector3 character_position = main_character.forward;
            Vector3 controller_position = main_controller.forward;
            Vector3 crosshair_up = Bearing.repeller(controller_position, character_position);
            main_controller.rotation = Quaternion.LookRotation(controller_position, crosshair_up);
        }

        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        private Transform main_character;
        private Transform main_controller;
        [NonSerialized] private Transform spotlight;
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