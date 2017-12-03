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
                internal_collider.radius = scale_variable;
            }
        }

        public void register(PlanetariaMonoBehaviour listener)
        {
            listeners.Add(listener);
            foreach (PlanetariaCollider field in field_set)
            {
                listener.enter_field(field);
            }
            if (current_collision.exists)
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

        private void Awake()
        {
            listeners.AddRange(this.GetComponentsInParent<PlanetariaMonoBehaviour>());
            GameObject child_for_collision = new GameObject("PlanetariaCollider");
            child_for_collision.transform.localPosition = Vector3.forward;
            child_for_collision.transform.parent = this.gameObject.transform;
            internal_collider = child_for_collision.transform.GetOrAddComponent<SphereCollider>();
            transform = this.GetOrAddComponent<PlanetariaTransform>();
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
                return;
            }
            optional<PlanetariaCollider> planetaria_collider = PlanetariaCache.collider_cache.get(sphere_collider.data);
            if (!planetaria_collider.exists)
            {
                return;
            }
            NormalizedCartesianCoordinates position = transform.position;
            optional<Arc> arc = PlanetariaCache.arc_cache.get(sphere_collider.data); // C++17 if statements are so pretty compared to this...
            if (arc.exists)
            {
                optional<Block> block = PlanetariaCache.block_cache.get(sphere_collider.data);
                if (!block.exists)
                {
                    Debug.LogError("Critical Err0r.");
                    return;
                }
                enter_block(arc.data, block.data, planetaria_collider.data, position); // block collisions are handled in OnCollisionStay(): notification stage
            }
            else // field
            {
                optional<PlanetariaCollider> field = PlanetariaCache.collider_cache.get(sphere_collider.data);
                if (!field.exists)
                {
                    Debug.LogError("This is likely an Err0r or setup issue.");
                    return;
                }
                exit_field(field.data, position); // field triggering is handled in OnCollisionStay(): notification stage
                enter_field(field.data, position);
            }
        }

        private void block_notifications()
        {
            optional<BlockCollision> next_collision = collision_candidates.Aggregate(
                    (closest, next_candidate) =>
                    closest.distance < next_candidate.distance ? closest : next_candidate);

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
                if (block.active && arc.contains(position.data, transform.scale/2))
                {
                    float half_height = transform.scale / 2;
                    optional<BlockCollision> collision = BlockCollision.block_collision(arc, block, collider, transform.previous_position.data, transform.position.data, half_height);
                    if (collision.exists)
                    {
                        collision_candidates.Add(collision.data);
                    }
                }
            }
        }
        
        private void enter_field(PlanetariaCollider field, NormalizedCartesianCoordinates position)
        {
            if (!field_set.Contains(field) && PlanetariaIntersection.field_field_intersection(this.colliders, field.colliders))
            {
                field_set.Add(field);
                fields_entered.Add(field);
            }
        }

        private void exit_field(PlanetariaCollider field, NormalizedCartesianCoordinates position)
        {
            if (field_set.Contains(field) && !PlanetariaIntersection.field_field_intersection(this.colliders, field.colliders))
            {
                field_set.Remove(field);
                fields_exited.Add(field);
            }
        }

        private new PlanetariaTransform transform;
        private SphereCollider internal_collider; // FIXME: collider list
        public Sphere[] colliders = new Sphere[0];
        private List<PlanetariaMonoBehaviour> listeners = new List<PlanetariaMonoBehaviour>();
        float scale_variable;

        public optional<BlockCollision> current_collision = new optional<BlockCollision>(); // FIXME: JANK
        private List<BlockCollision> collision_candidates = new List<BlockCollision>();
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