﻿using System;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    public class Projectile : MonoBehaviour // HACK: normally I would use PlanetariaMonoBehaviour, but this should drastically improve collision performance
    {
        public void respawn(Vector3 position, Vector3 direction)
        {
            planetaria_transform.position = position;
            planetaria_transform.direction = direction;
            planetaria_rigidbody.relative_velocity = new Vector2(0, speed);
            has_collided = false;
            sphere_collider.enabled = true;
            mesh_renderer.enabled = true;
        }

        private void OnValidate()
        {
            sphere_collider = this.GetComponent<SphereCollider>();
            planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
            planetaria_transform = this.GetComponent<PlanetariaTransform>();
            planetaria_renderer = this.GetComponent<AreaRenderer>();
            mesh_renderer = this.GetComponentInChildren<MeshRenderer>();

            //planetaria_collider.shape = PlanetariaShape.Create(planetaria_transform.localScale);
            planetaria_renderer.scale = planetaria_transform.localScale;
        }
        
        public void Start()
        {
            // set velocity
            planetaria_rigidbody.relative_velocity = new Vector2(0, speed);
            //PlanetariaGameObject.Destroy(this.gameObject, lifetime);
        }

        // while I do like the idea of raycasting in update, that would require PlanetariaColliders or hacking my current raycast implementation, so true-to-original it is
        // Also, raycasting would be broken with relative projectile velocities

        public void OnTriggerEnter(Collider collider)
        {
            if (!has_collided) // make sure one projectile doesn't collide with 2+ debris.
            {
                Debris debris = collider.GetComponent<Debris>();
                if (debris.destroy_asteroid()) // make sure two projectiles don't collide with the same debris (wasting one of them).
                {
                    //PlanetariaGameObject.Destroy(this.gameObject); -- the bullets are reused (as a form of object pooling)
                    has_collided = true;
                    mesh_renderer.enabled = false;
                    sphere_collider.enabled = false;
                }
            }
        }
        
        [SerializeField] public float speed = Mathf.PI;
        [SerializeField] public float lifetime = 1;
        [SerializeField] [HideInInspector] private SphereCollider sphere_collider;
        [SerializeField] [HideInInspector] private AreaRenderer planetaria_renderer;
        [SerializeField] [HideInInspector] private MeshRenderer mesh_renderer;
        [SerializeField] [HideInInspector] private PlanetariaRigidbody planetaria_rigidbody;
        [SerializeField] [HideInInspector] private PlanetariaTransform planetaria_transform;
        [NonSerialized] private bool has_collided = false;
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