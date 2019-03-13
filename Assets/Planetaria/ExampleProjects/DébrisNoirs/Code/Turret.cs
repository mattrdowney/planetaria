using System;
using System.Collections.Generic;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    public class Turret : PlanetariaMonoBehaviour
    {
        public void die()
        {
            foreach (Projectile projectile in projectiles_on_screen)
            {
                PlanetariaGameObject.Destroy(projectile.gameObject);
            }
            projectiles_on_screen.Clear();
            next_projectile_to_reuse = 0;
        }

        private void Start()
        {
            satellite = this.transform.parent.parent.gameObject.internal_game_object.GetComponent<Satellite>();
            turret = this.gameObject.internal_game_object.transform;
            projectiles_on_screen = new List<Projectile>();
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
                if (projectiles_on_screen.Count < 14) // FIXME: MAGIC NUMBER - number of bullets spawned per second
                {
                    PlanetariaGameObject spawned_projectile = PlanetariaGameObject.Instantiate(projectile, rail_position, bullet_direction);
                    projectiles_on_screen.Add(spawned_projectile.internal_game_object.GetComponent<Projectile>());
                }
                else
                {
                    projectiles_on_screen[next_projectile_to_reuse].respawn(rail_position, bullet_direction);
                    next_projectile_to_reuse += 1;
                    if (next_projectile_to_reuse >= 14)
                    {
                        next_projectile_to_reuse -= 14;
                    }
                }
            }
        }

        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        [SerializeField] public Satellite satellite;
        [SerializeField] public GameObject projectile;
        [SerializeField] private Transform turret;

        [NonSerialized] private int next_projectile_to_reuse = 0;
        [NonSerialized] private List<Projectile> projectiles_on_screen;
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