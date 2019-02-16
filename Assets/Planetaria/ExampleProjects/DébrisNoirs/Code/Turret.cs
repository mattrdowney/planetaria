﻿using System;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    public class Turret : MonoBehaviour
    {
        private void Start()
        {
            satellite = this.transform.parent.parent.gameObject.GetComponent<Satellite>();
            turret = this.gameObject.transform;
#if UNITY_EDITOR
            GameObject.FindObjectOfType<PlanetariaActuator>().input_device_type = PlanetariaActuator.InputDevice.Mouse;
#else
            GameObject.FindObjectOfType<PlanetariaActuator>().input_device_type = PlanetariaActuator.InputDevice.Gyroscope;
#endif
        }

        private void Update()
        {
            Vector3 rail_position = turret.forward;
            Vector3 bullet_direction = turret.up;
            
            if (DebrisNoirsInput.firing() && satellite.alive()) // if firing and not dead
            {
                PlanetariaGameObject.Instantiate(projectile, rail_position, bullet_direction);
            }
        }

        [SerializeField] public Satellite satellite;
        [SerializeField] public GameObject projectile;
        [SerializeField] private Transform turret;
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