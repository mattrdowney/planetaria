using System;
using System.Collections.Generic;
using System.Linq; // TODO: Linq affects garbage collector (which affects virtual reality), also collisions are crazy inefficient
using UnityEngine;

namespace Planetaria
{
    /*
    [Serializable]
    public class CollisionObserver // FIXME: remove this, it really confuses me (terrible architecture)
    {
        public CollisionObserver() { }

        public void initialize (PlanetariaCollider observed, PlanetariaMonoBehaviour[] observers)
        {
            this.planetaria_collider = observed;
            this.planetaria_rigidbody = observed.GetComponent<PlanetariaRigidbody>();
            this.planetaria_transformation = observed.GetComponent<PlanetariaTransform>();
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                if (!this.observers.Contains(observer))
                {
                    this.observers.Add(observer);
                }
            }
        }

        public void register(PlanetariaMonoBehaviour observer)
        {
            if (!this.observers.Contains(observer))
            {
                observers.Add(observer);
                foreach (PlanetariaCollider field in current_fields)
                {
                    observer.enter_field(field);
                }
                foreach (BlockCollision collision in current_collisions)
                {
                    observer.enter_block(collision);
                }
            }
        }

        public void unregister(PlanetariaMonoBehaviour observer)
        {
            if (this.observers.Contains(observer))
            {
                observers.Remove(observer);
                foreach (PlanetariaCollider field in current_fields)
                {
                    observer.exit_field(field);
                }
                foreach (BlockCollision collision in current_collisions)
                {
                    observer.exit_block(collision);
                }
            }
        }

        public void notify()
        {
            notify_all_field();
            notify_all_block();
        }

        public PlanetariaCollider collider()
        {
            return planetaria_collider;
        }
        
        public bool colliding()
        {
            return current_collisions.Count > 0;
        }

        public List<BlockCollision> collisions() // HACK: FIXME: remove
        {
            return new List<BlockCollision>(current_collisions);
        }
        
        public void potential_field_collision(PlanetariaCollider field) 
        {
            field_candidates.Add(field);
        }
 
        public void potential_block_collision(Arc arc, PlanetariaCollider collider)
        {
            if (planetaria_rigidbody.exists)
            {
                if (current_collisions.Count == 0 || current_collisions[0].other != collider)
                {
                    optional<BlockCollision> collision = BlockCollision.block_collision(this, arc, collider, planetaria_transformation, planetaria_rigidbody.data);
                    if (collision.exists)
                    {
                        if (planetaria_rigidbody.exists)
                        {
                            if (planetaria_rigidbody.data.collide(collision.data, this))
                            {
                                collision_candidates.Add(collision.data);
                            }
                        }
                    }
                }
            }
        }

        public void clear_block_collision()
        {
            while (current_collisions.Count > 0)
            {
                BlockCollision collision = current_collisions[current_collisions.Count-1];
                collision.self.get_observer().notify_exit_block(collision);
                collision.other.get_observer().notify_exit_block(collision);
                current_collisions.Remove(collision);
            }
            collision_candidates.Clear();
        }

        internal void notify_all_block()
        {
            if (collision_candidates.Count > 0)
            {
                BlockCollision next_collision = collision_candidates.Aggregate(
                        (closest, next_candidate) =>
                        closest.distance < next_candidate.distance ? closest : next_candidate);

                if (current_collisions.Count > 1)
                {
                    Debug.LogError("This should never happen.");
                }

                while (current_collisions.Count > 0)
                {
                    BlockCollision collision = current_collisions[current_collisions.Count - 1];
                    collision.self.get_observer().notify_exit_block(collision);
                    collision.other.get_observer().notify_exit_block(collision);
                }
                next_collision.self.get_observer().notify_enter_block(next_collision);
                next_collision.other.get_observer().notify_enter_block(next_collision);
                collision_candidates.Clear();
            }
            notify_stay_block();
        }

        internal void notify_enter_block(BlockCollision collision)
        {
            current_collisions.Add(collision);
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                observer.enter_block(collision);
            }
        }

        internal void notify_stay_block()
        {
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                foreach (BlockCollision collision in new List<BlockCollision>(current_collisions))
                {
                    observer.stay_block(collision);
                }
            }
        }

        internal void notify_exit_block(BlockCollision collision)
        {
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                observer.exit_block(collision);
            }

            //Debug.LogError(collision.GetHashCode() + " vs " + (current_collisions.Count > 0 ? current_collisions[0].GetHashCode().ToString() : ""));
            
            int before_count = current_collisions.Count;
            current_collisions.Remove(collision);
            int after_count = current_collisions.Count;
            Debug.Assert(before_count > after_count || after_count == 0,
                    before_count + " should be greater than " + after_count + " for " + collision.GetHashCode() + " vs " + (after_count > 0 ? current_collisions[0].GetHashCode().ToString() : ""));
        }

        internal void notify_all_field()
        {
            foreach (PlanetariaCollider field in field_candidates) // if a new field is detected for first time, enter
            {
                if (!current_fields.Contains(field))
                {
                    notify_enter_field(field);
                }
            }
            foreach (PlanetariaCollider field in current_fields) // if a field is no longer detected, exit
            {
                if (!field_candidates.Contains(field))
                {
                    notify_exit_field(field);
                }
            }
            current_fields = field_candidates; // Swap happens here (to avoid iterator invalidation)
            notify_stay_field(); // ORDER DEPENDENCY : current_fields must be set first!
            field_candidates = new List<PlanetariaCollider>(); // .Clear doesn't work because of reference/pointer logic
        }

        internal void notify_enter_field(PlanetariaCollider field)
        {
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                observer.enter_field(field);
            }
        }

        internal void notify_stay_field()
        {
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                foreach (PlanetariaCollider field in current_fields)
                {
                    observer.stay_field(field);
                }
            }
        }

        internal void notify_exit_field(PlanetariaCollider field)
        {
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                observer.exit_field(field);
            }
            //current_fields.Remove(field); // Don't want invalidated iterators - swap at end of loop
        }

        [SerializeField] private PlanetariaTransform planetaria_transformation;
        [SerializeField] private PlanetariaCollider planetaria_collider;
        [SerializeField] private optional<PlanetariaRigidbody> planetaria_rigidbody;
        [SerializeField] private List<PlanetariaMonoBehaviour> observers = new List<PlanetariaMonoBehaviour>();

        [NonSerialized] private List<BlockCollision> current_collisions = new List<BlockCollision>();
        [NonSerialized] private List<BlockCollision> collision_candidates = new List<BlockCollision>();

        [NonSerialized] private List<PlanetariaCollider> current_fields = new List<PlanetariaCollider>();
        [NonSerialized] private List<PlanetariaCollider> field_candidates = new List<PlanetariaCollider>();
    }
    */
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