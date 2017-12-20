using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRigidbody : MonoBehaviour
    {
        private void Awake()
        {
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            internal_rigidbody = this.GetOrAddComponent<Rigidbody>();
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
            position = transform.position.data;
            get_acceleration();
        }

        private void FixedUpdate()
        {
            Debug.Log("Happening");
            position = transform.position.data;

            Debug.Log(position);
            Debug.Log(velocity);
            Debug.Log(acceleration);
            Debug.Log(gravity_wells[0]);

            // I am making a bet this is relevant in spherical coordinates (it is Euler isn't it?): http://openarena.ws/board/index.php?topic=5100.0
            velocity = velocity + acceleration * (Time.deltaTime / 2);
            position = PlanetariaMath.slerp(position, velocity.normalized, velocity.magnitude * Time.deltaTime); // Note: when velocity = Vector3.zero, it luckily still returns "position" intact.
            get_acceleration();
            velocity = velocity + acceleration * (Time.deltaTime / 2);
            transform.position = new NormalizedCartesianCoordinates(position);
        }

        private void get_acceleration()
        {
            acceleration = Vector3.zero; // in theory this could mess things up

            for (int force = 0; force < gravity_wells.Length; ++force)
            {
                acceleration += Bearing.attractor(position, gravity_wells[force])*gravity_wells[force].magnitude;
            }
        }

        // position
        private Vector3 position; // magnitude = 1
        private Vector3 velocity; // magnitude in [0, infinity]
        private Vector3 acceleration; // magnitude in [0, infinity]

        // gravity
        [SerializeField] public Vector3[] gravity_wells;

        private new PlanetariaTransform transform;
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