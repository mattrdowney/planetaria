using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    public class PlanetariaCollider : MonoBehaviour // FIXME: while not incredibly complicated, I think there might be a way to simplify this
    {
        public float scale
        {
            get
            {
                return scale_variable;
            }
            set
            {
                scale_variable = value;
                if (scalable)
                {
                    colliders = new Sphere[] { Sphere.filled_circle(internal_transform, Vector3.forward, value) };
                    internal_collider.center = colliders[0].center;
                    internal_collider.radius = colliders[0].radius;
                }
            }
        }

        public void register(PlanetariaMonoBehaviour listener)
        {
            listeners.Add(listener);
            foreach (PlanetariaCollider field in field_set)
            {
                listener.enter_field(field);
            }
            foreach (BlockCollision collision in current_collisions)
            {
                listener.enter_block(current_collision.data);
            }
        }

        public void unregister(PlanetariaMonoBehaviour listener)
        {
            listeners.Remove(listener);
            foreach (PlanetariaCollider field in field_set)
            {
                listener.exit_field(field);
            }
            if (current_collision.exists)
            {
                listener.enter_block(new optional<BlockCollision>());
            }
        }

        public SphereCollider get_sphere_collider()
        {
            return internal_collider;
        }

        public void set_colliders(Sphere[] colliders)
        {
            this.colliders = colliders;
            Sphere furthest_collider = colliders.Aggregate(
                    (furthest, next_candidate) =>
                    furthest.center.magnitude - furthest.radius >
                    next_candidate.center.magnitude - next_candidate.radius
                    ? furthest : next_candidate);

            internal_collider.center = furthest_collider.debug_center;
            internal_collider.radius = furthest_collider.radius;
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

        private void Awake()
        {
            listeners.AddRange(this.GetComponentsInParent<PlanetariaMonoBehaviour>());
            GameObject child_for_collision = new GameObject("SphereCollider");
            child_for_collision.transform.parent = this.gameObject.transform;
            internal_collider = child_for_collision.transform.GetOrAddComponent<SphereCollider>();
            internal_transform = this.GetOrAddComponent<Transform>();
            planetaria_transform = this.GetOrAddComponent<PlanetariaTransform>();
            rigidbody = this.GetComponent<PlanetariaRigidbody>();
            StartCoroutine(notify());
            // add to collision_map and trigger_map for all objects currently intersecting (via Physics.OverlapBox()) // CONSIDER: I think Unity Fixed this, right?
        }

        private IEnumerator notify()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                field_notifications();
                block_notifications();
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            optional<SphereCollider> sphere_collider = collider as SphereCollider;
            if (!sphere_collider.exists)
            {
                Debug.LogError("This should never happen");
                return;
            }
            optional<PlanetariaCollider> other_collider = PlanetariaCache.collider_cache.get(sphere_collider.data);
            if (!other_collider.exists)
            {
                Debug.LogError("This should never happen");
                return;
            }
            /*
             * Collision draft

                Block instantiates as is_field = false
                Character instantiates as is_field = false

                if (!left.is_field && !right.is_field) collision
                    dynamic sends collide_with_me message to static
                    dynamic uses previous_position -> position to generate collision point if it exists
                    current_collision needs to be augmented so static colliders can register multiple collisions

                if (left.is_field xor right.is_field) field
                    collide regardless
            */
            if (!(this.is_field && other_collider.data.is_field)) // fields pass through each other (same as triggers)
            {
                if (PlanetariaIntersection.collider_collider_intersection(this.colliders, other_collider.data.colliders))
                {
                    if (this.is_field || other_collider.data.is_field) // field collision
                    {
                        if (!field_set.Contains(other_collider.data)) // field triggering is handled in OnCollisionStay(): notification stage
                        {
                            field_set.Add(other_collider.data);
                            fields_entered.Add(other_collider.data);
                        }
                    }
                    else // block collision
                    {
                        if (rigidbody.exists)
                        {
                            optional<Arc> arc = PlanetariaCache.arc_cache.get(sphere_collider.data); // C++17 if statements are so pretty compared to this...
                            if (arc.exists)
                            {
                                optional<Block> block = PlanetariaCache.block_cache.get(sphere_collider.data);
                                if (!block.exists)
                                {
                                    Debug.LogError("Critical Err0r.");
                                    return;
                                }
                                enter_block(arc.data, block.data, other_collider.data, position); // block collisions are handled in OnCollisionStay(): notification stage
                            }
                        }
                    }
                    /*
                    */
                }
                else
                {
                    if (this.is_field || other_collider.data.is_field)
                    {
                        if (field_set.Contains(other_collider.data))
                        {
                            field_set.Remove(other_collider.data);
                            fields_exited.Add(other_collider.data);
                        }
                    }
                }
            }
        }

        private void block_notifications()
        {
            optional<BlockCollision> next_collision = new optional<BlockCollision>();

            if (collision_candidates.Count > 0)
            {
                next_collision = collision_candidates.Aggregate(
                        (closest, next_candidate) =>
                        closest.distance < next_candidate.distance ? closest : next_candidate);
            }

            if (next_collision.exists)
            {
                block_notification(next_collision);
            }
        }
        
        private void block_notification(optional<BlockCollision> next_collision)
        {
            if (next_collision.exists)
            {
                foreach (PlanetariaMonoBehaviour listener in listeners)
                {
                    listener.enter_block(next_collision.data);
                }
                next_collision.data.collider.block_notification(next_collision);
                current_collision = next_collision;
            }
        }

        private void field_notifications()
        {
            foreach (PlanetariaMonoBehaviour listener in listeners)
            {
                foreach (PlanetariaCollider field in fields_entered)
                {
                    listener.enter_field(field);
                }
            }
            fields_entered.Clear();
            foreach (PlanetariaMonoBehaviour listener in listeners)
            {
                foreach (PlanetariaCollider field in fields_exited)
                {
                    listener.exit_field(field);
                }
            }
            fields_exited.Clear();
        }

        private void enter_block(Arc arc, Block block, PlanetariaCollider collider, NormalizedCartesianCoordinates position)
        {
            if (!current_collision.exists || current_collision.data.block != block)
            {
                if (block.active)
                {
                    optional<BlockCollision> collision = BlockCollision.block_collision(arc, block, collider, planetaria_transform.previous_position.data, planetaria_transform.position.data, scale);
                    if (collision.exists)
                    {
                        collision_candidates.Add(collision.data);
                    }
                }
            }
        }

        private Transform internal_transform;
        private PlanetariaTransform planetaria_transform;
        private SphereCollider internal_collider;
        private new optional<PlanetariaRigidbody> rigidbody;
        [SerializeField] public Sphere[] colliders = new Sphere[0]; // FIXME: private
        private List<PlanetariaMonoBehaviour> listeners = new List<PlanetariaMonoBehaviour>();
        private float scale_variable;
        private bool scalable = false;
        private bool is_field_variable = false;

        public List<BlockCollision> current_collisions = new List<BlockCollision>(); // FIXME: JANK
        private List<BlockCollision> collision_candidates = new List<BlockCollision>(); // FIXME: move to SharedCollision class with notification properties
        private List<PlanetariaCollider> field_set = new List<PlanetariaCollider>();
        private List<PlanetariaCollider> fields_entered = new List<PlanetariaCollider>();
        private List<PlanetariaCollider> fields_exited = new List<PlanetariaCollider>();
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