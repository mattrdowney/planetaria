using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public abstract class PlanetariaActor : PlanetariaMonoBehaviour
    {
        protected sealed override void Awake()
        {
            set_delegates();
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            foreach (PlanetariaCollider channel in this.GetComponentsInChildren<PlanetariaCollider>())
            {
                channel.register(this);
            }
            // FIXME: still need to cache (properly)
            if (on_first_exists.exists)
            {
                on_first_exists.data();
            }
        }

        protected sealed override void Start()
        {
            if (on_time_zero.exists)
            {
                on_time_zero.data();
            }
        }
    
        protected sealed override void Update() // always calling Update is less than ideal
        {
            if (on_every_frame.exists)
            {
                on_every_frame.data();
            }
        }

        protected sealed override void FixedUpdate() // always calling FixedUpdate is less than ideal
        {
            if (on_before_physics.exists)
            {
                on_before_physics.data();
            }
        }

        protected sealed override void OnDestroy()
        {
            if (on_destroy.exists)
            {
                on_destroy.data();
            }
            // FIXME: still need to un-cache (properly)
            foreach (PlanetariaCollider channel in this.GetComponentsInChildren<PlanetariaCollider>())
            {
                channel.unregister(this);
            }
        }

        protected sealed override void OnCollisionStay()
        {
            if (on_field_stay.exists)
            {
                foreach (PlanetariaCollider field in fields)
                {
                    on_field_stay.data(field);
                }
            }
            if (on_block_stay.exists)
            {
                if (current_collision.exists)
                {
                    on_block_stay.data(current_collision.data);
                }
            }
        }

        public void enter_block(BlockCollision collision)
        {
            if (!current_collision.exists)
            {
                if (on_block_enter.exists)
                {
                    on_block_enter.data(collision);
                }
                current_collision = collision;
            }
            else
            {
                Debug.Log("Critical Error");
            }
        }

        public void exit_block(BlockCollision collision)
        {
            if (current_collision.exists && collision == current_collision.data) // FIXME: probably have to create proper equality function
            {
                if (on_block_exit.exists)
                {
                    on_block_exit.data(current_collision.data);
                }
                current_collision = new optional<BlockCollision>();
            }
            else
            {
                Debug.Log("Critical Error");
            }
        }

        public void enter_field(PlanetariaCollider field)
        {
            fields.Add(field);
            if (on_field_enter.exists)
            {
                on_field_enter.data(field);
            }
        }

        public void exit_field(PlanetariaCollider field)
        {
            if (fields.Remove(field))
            {
                if (on_field_exit.exists)
                {
                    on_field_exit.data(field);
                }
            }
            else
            {
                Debug.Log("Critical Error");
            }
        }

        protected abstract void set_delegates();
        
        protected sealed override void LateUpdate() { }

        protected sealed override void OnTriggerEnter() { }
        protected sealed override void OnTriggerStay() { }
        protected sealed override void OnTriggerExit() { }

        protected sealed override void OnCollisionEnter() { }
        //protected sealed override void OnCollisionStay() { } // used as a post-physics-update
        protected sealed override void OnCollisionExit() { }

        protected sealed override void OnTriggerEnter(Collider collider) { }
        protected sealed override void OnTriggerStay(Collider collider) { }
        protected sealed override void OnTriggerExit(Collider collider) { } // To account for object deletion, this (or other code) must be defined.

        protected sealed override void OnCollisionEnter(Collision collision) { }
        protected sealed override void OnCollisionStay(Collision collision) { }
        protected sealed override void OnCollisionExit(Collision collision) { }

        private optional<BlockCollision> current_collision = new optional<BlockCollision>();
        private List<PlanetariaCollider> fields = new List<PlanetariaCollider>();
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