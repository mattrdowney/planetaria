using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    public abstract class PlanetariaActor : PlanetariaMonoBehaviour
    {
        protected sealed override void Awake()
        {
            set_delegates();
            Debug.Log("Happened");
            transform = this.GetOrAddComponent<PlanetariaTransform>();
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
    
        protected sealed override void FixedUpdate() // always calling FixedUpdate is less than ideal
        {
            if (on_every_frame.exists)
            {
                on_every_frame.data();
            }
        }

        protected abstract void set_delegates();

        protected sealed override void Update() { }
        protected sealed override void LateUpdate() { }

        protected sealed override void OnTriggerEnter(Collider collider) { }
        protected sealed override void OnTriggerStay(Collider collider) { }
        protected sealed override void OnTriggerExit(Collider collider) { } // To account for object deletion, this (or other code) must be defined.

        protected sealed override void OnCollisionEnter(Collision collision) { }
        protected sealed override void OnCollisionStay(Collision collision) { }
        protected sealed override void OnCollisionExit(Collision collision) { }
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