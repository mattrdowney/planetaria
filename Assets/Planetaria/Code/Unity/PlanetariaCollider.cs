using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public sealed class PlanetariaCollider : PlanetariaComponent // FIXME: while not incredibly complicated, I think there might be a way to simplify this
    {
        protected override sealed void Awake()
        {
            base.Awake();
            initialize();
            StartCoroutine(wait_for_fixed_update());
            observer = new CollisionObserver();
            observer.initialize(this, this.GetComponentsInParent<PlanetariaMonoBehaviour>());
        }

        protected override sealed void Reset()
        {
            base.Reset();
            initialize();
        }

        protected override void OnDestroy()
        {
            PlanetariaCache.self.uncache(this);
        }

        private void initialize()
        {
            GameObject game_object = this.GetOrAddChild("InternalCollider");
            if (internal_collider == null)
            {
                internal_collider = Miscellaneous.GetOrAddComponent<SphereCollider>(game_object);
            }
            if (internal_transform == null)
            {
                internal_transform = Miscellaneous.GetOrAddComponent<Transform>(game_object);
            }
            if (planetaria_transform == null)
            {
                planetaria_transform = this.GetOrAddComponent<PlanetariaTransform>();
            }
            if (!rigidbody.exists)
            {
                rigidbody = this.GetComponentInParent<PlanetariaRigidbody>();
            }
            cache(this.shape);
            // add to collision_map and trigger_map for all objects currently intersecting (via Physics.OverlapBox()) // CONSIDER: I think Unity Fixed this, right?
        }

        public PlanetariaShape shape
        {
            get
            {
                return shape_variable;
            }
            set
            {
                cache(value);
            }
        }

        private void cache(PlanetariaShape shape)
        {
            PlanetariaCache.self.uncache(this);
            shape_variable = shape;
            if (shape != null)
            {
                PlanetariaCache.self.cache(this);
                PlanetariaSphereCollider sphere = shape.bounding_sphere;

                internal_collider.center = sphere.center;
                internal_collider.radius = sphere.radius;
            }
            else
            {
                internal_collider.radius = float.NegativeInfinity; // FIXME: HACK: ensure there are no collisions
            }
            internal_collider.isTrigger = true; // Rigidbody must be added for collisions to be detected.
        }

        public void register(PlanetariaMonoBehaviour listener)
        {
            observer.register(listener);
        }

        public void unregister(PlanetariaMonoBehaviour listener)
        {
            observer.unregister(listener);
        }

        public CollisionObserver get_observer()
        {
            return observer;
        }

        public SphereCollider get_sphere_collider()
        {
            return internal_collider;
        }

        public bool is_field
        {
            get
            {
                return is_field_variable;
            }
            set
            {
                is_field_variable = value;
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            Debug.Log("Happening");
            optional<SphereCollider> sphere_collider = collider as SphereCollider;
            if (!sphere_collider.exists)
            {
                Debug.LogError("This should never happen");
                return;
            }
            optional<PlanetariaCollider> other_collider = PlanetariaCache.self.collider_fetch(sphere_collider.data);
            if (!other_collider.exists)
            {
                Debug.LogError("This should never happen");
                return;
            }
            if (!this.is_field) // fields pass through each other (same as triggers)
            {
                Quaternion shift_from_self_to_other = other_collider.data.internal_transform.rotation;
                if (this.internal_transform.rotation != Quaternion.identity) // Only shift orientation when necessary
                {
                    // TODO: verify the order of operations is correct (and logic itself)
                    shift_from_self_to_other = Quaternion.Inverse(other_collider.data.gameObject.internal_game_object.transform.rotation) * shift_from_self_to_other;
                }

                if (other_collider.data.is_field) // field collision
                {
                    if (this.shape.field_collision(other_collider.data.shape, shift_from_self_to_other))
                    {
                        observer.potential_field_collision(other_collider.data); // TODO: augment field (like Unity triggers) works on both the sender and receiver.
                    }
                }
                else // block collision
                {
                    // FIXME: TODO: make sure only the character collides for now

                    foreach (Arc intersection in this.shape.block_collision(other_collider.data.shape, shift_from_self_to_other))
                    {
                        Vector3 position = planetaria_transform.position.data;
                        if (shift_from_self_to_other != Quaternion.identity) // Only shift orientation when necessary
                        {
                            position = Quaternion.Inverse(other_collider.data.gameObject.internal_game_object.transform.rotation) * position;
                        }
                        if (intersection.contains(position, planetaria_transform.scale/2))
                        {
                            observer.potential_block_collision(intersection, other_collider.data); // block collisions are handled in OnCollisionStay(): notification stage
                        }
                    }
                }
            }
        }

        private IEnumerator wait_for_fixed_update()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                observer.notify();
            }
        }
        
        [SerializeField] public PlanetariaPhysicMaterial material;
        [SerializeField] private PlanetariaShape shape_variable;
        [SerializeField] [HideInInspector] private CollisionObserver observer;
        [SerializeField] [HideInInspector] private Transform internal_transform;
        [SerializeField] [HideInInspector] private PlanetariaTransform planetaria_transform;
        [SerializeField] [HideInInspector] private SphereCollider internal_collider;
        [SerializeField] [HideInInspector] public new optional<PlanetariaRigidbody> rigidbody;
        [SerializeField] public bool is_field_variable = false;
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