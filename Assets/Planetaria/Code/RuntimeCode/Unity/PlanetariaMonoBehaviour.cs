using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public abstract class PlanetariaActor : PlanetariaComponent
    {
        protected abstract void on_construction(); // I can now transition to snake_case ^0^ libre
        protected abstract void on_destruction();

        protected delegate void CollisionDelegate(BlockCollision block_information);
        protected delegate void TriggerDelegate(PlanetariaCollider field_information);

        protected optional<CollisionDelegate> on_block_enter = null;
        protected optional<CollisionDelegate> on_block_exit = null;
        protected optional<CollisionDelegate> on_block_stay = null;

        protected optional<TriggerDelegate> on_field_enter = null;
        protected optional<TriggerDelegate> on_field_exit = null;
        protected optional<TriggerDelegate> on_field_stay = null;

        protected override sealed void Awake()
        {
            base.Awake();
            foreach (PlanetariaCollider collider in this.GetComponentsInChildren<PlanetariaCollider>()) // FIXME:
            {
                collider.register(this);
                observers.Add(collider.get_observer());
            }
            // FIXME: still need to cache (properly)
            on_construction();
        }

        protected override sealed void OnDestroy()
        {
            on_destruction();
            // FIXME: still need to un-cache (properly)
            foreach (PlanetariaCollider collider in this.GetComponentsInChildren<PlanetariaCollider>())
            {
                collider.unregister(this);
            }
        }

        public void enter_block(BlockCollision collision)
        {
            if (on_block_enter.exists)
            {
                on_block_enter.data(collision);
            }
        }
        
        public void stay_block(BlockCollision collision)
        {
            if (on_block_stay.exists)
            {
                on_block_stay.data(collision);
            }
        }

        public void exit_block(BlockCollision collision)
        {
            if (on_block_exit.exists)
            {
                on_block_exit.data(collision);
            }
        }

        public void enter_field(PlanetariaCollider field)
        {
            if (on_field_enter.exists)
            {
                on_field_enter.data(field);
            }
        }

        public void stay_field(PlanetariaCollider field)
        {
            if (on_field_stay.exists)
            {
                on_field_stay.data(field);
            }
        }

        public void exit_field(PlanetariaCollider field)
        {
            if (on_field_exit.exists)
            {
                on_field_exit.data(field);
            }
        }
        
        [NonSerialized] [HideInInspector] private List<CollisionObserver> observers = new List<CollisionObserver>();
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