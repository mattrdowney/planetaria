using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    public class CollisionObserver
    {
        public CollisionObserver()
        {
        }

        public void initialize (PlanetariaTransform planetaria_transformation, PlanetariaMonoBehaviour[] observers)
        {
            this.planetaria_transformation = planetaria_transformation;
            this.planetaria_collider = planetaria_transformation.GetComponent<PlanetariaCollider>();
            this.planetaria_rigidbody = planetaria_transformation.GetComponent<PlanetariaRigidbody>();
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                this.observers.Add(observer);
            }
        }

        public void notify()
        {
            field_notifications();
            block_notifications();
        }

        private void block_notifications()
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
                    collision.self.get_observer().exit_block(collision);
                    collision.other.get_observer().exit_block(collision);
                }
                next_collision.self.get_observer().enter_block(next_collision);
                next_collision.other.get_observer().enter_block(next_collision);
                collision_candidates.Clear();
            }
        }

        private void field_notifications()
        {
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                foreach (PlanetariaCollider field in fields_entered)
                {
                    observer.enter_field(field);
                }
            }
            fields_entered.Clear();
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                foreach (PlanetariaCollider field in fields_exited)
                {
                    observer.exit_field(field);
                }
            }
            fields_exited.Clear();
        }

        public void register(PlanetariaMonoBehaviour observer)
        {
            observers.Add(observer);
            foreach (PlanetariaCollider field in field_set)
            {
                observer.enter_field(field);
            }
            foreach (BlockCollision collision in current_collisions)
            {
                observer.enter_block(collision);
            }
        }

        public void unregister(PlanetariaMonoBehaviour observer)
        {
            observers.Remove(observer);
            foreach (PlanetariaCollider field in field_set)
            {
                observer.exit_field(field);
            }
            foreach (BlockCollision collision in current_collisions)
            {
                observer.exit_block(collision);
            }
        }

        public void enter_field(PlanetariaCollider field)
        {
            if (!field_set.Contains(field)) // field triggering is handled in notify() stage
            {
                field_set.Add(field);
                fields_entered.Add(field);
            }
        }

        public void exit_field(PlanetariaCollider field)
        {
            if (field_set.Contains(field))
            {
                field_set.Remove(field);
                fields_exited.Add(field);
            }
        }

        public void enter_block(Arc arc, Block block, PlanetariaCollider collider)
        {
            if (planetaria_rigidbody.exists)
            {
                if (current_collisions.Count == 0 || current_collisions[0].block != block)
                {
                    if (block.active)
                    {
                        optional<BlockCollision> collision = BlockCollision.block_collision(this, arc, block, collider, planetaria_transformation, planetaria_rigidbody.data);
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
        }

        public void enter_block(BlockCollision collision)
        {
            current_collisions.Add(collision);
            foreach (PlanetariaMonoBehaviour observer in observers)
            {
                observer.enter_block(collision);
            }
        }

        public void exit_block(BlockCollision collision)
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

        public PlanetariaCollider collider()
        {
            return planetaria_collider;
        }
        
        public bool colliding()
        {
            return current_collisions.Count != 0;
        }

        public List<BlockCollision> collisions()
        {
            return new List<BlockCollision>(current_collisions);
        }

        public List<PlanetariaCollider> fields()
        {
            return new List<PlanetariaCollider>(field_set);
        }

        private PlanetariaTransform planetaria_transformation;
        private PlanetariaCollider planetaria_collider;
        private optional<PlanetariaRigidbody> planetaria_rigidbody;
        private List<PlanetariaMonoBehaviour> observers = new List<PlanetariaMonoBehaviour>();
        private List<BlockCollision> current_collisions = new List<BlockCollision>();
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