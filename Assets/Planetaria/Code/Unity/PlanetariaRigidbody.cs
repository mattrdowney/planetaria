using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRigidbody : MonoBehaviour
    {
        private void Awake()
        {
            internal_rigidbody = this.GetOrAddComponent<Rigidbody>();
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
        }

        // position
        private NormalizedCartesianCoordinates position;

        // velocity
        private NormalizedCartesianCoordinates velocity_pivot; // used when orbit_gravity_well = false, recalculates every frame?
        private float horizontal_velocity;
        private float vertical_velocity; // used when orbit_gravity_well = true

        // gravity/acceleration
        private NormalizedCartesianCoordinates gravity_well; // The south pole towards which the object will accelerate
        private float gravity; // gravity is constant
        private bool orbit_gravity_well; // is the object's velocity relative to gravity_well.

        private Rigidbody internal_rigidbody;
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