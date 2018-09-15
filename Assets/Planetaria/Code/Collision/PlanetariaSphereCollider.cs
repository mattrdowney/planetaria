using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    /// <summary>
    /// Inspector - Immutable Sphere struct
    /// </summary>
    public struct PlanetariaSphereCollider
    {
        public bool collides_with(PlanetariaSphereCollider other, Quaternion shift_from_self_to_other)
        {
            if (float.IsInfinity(this.radius) || float.IsInfinity(other.radius)) // optimization for always_collide()/never_collide()
            {
                bool never_collide = float.IsNegativeInfinity(this.radius) || float.IsNegativeInfinity(other.radius);
                return never_collide == false;
            }
            Vector3 this_center = this.center;
            Vector3 other_center = other.center;
            if (shift_from_self_to_other != Quaternion.identity)
            {
                this_center = shift_from_self_to_other * this_center;
            }
            float magnitude_squared = (this_center - other_center).sqrMagnitude;
            float sum_of_radii = this.radius + other.radius;
            return magnitude_squared < sum_of_radii*sum_of_radii;
        }

        public Vector3 center
        {
            get
            {
                return center_variable;
            }
        }

        public float radius
        {
            get
            {
                return radius_variable;
            }
        }

        public PlanetariaSphereCollider(Vector3 center, float radius)
        {
            center_variable = center;
            radius_variable = radius;
        }

        /// <summary>
        /// Constructor - always collides with any PlanetariaSphereCollider (when properly formed i.e. intersects a unit-sphere)
        /// </summary>
        /// <returns>A collider that always collides.</returns>
        public static PlanetariaSphereCollider always { get { return always_collide; } } // TODO: verify

        /// <summary>
        /// Constructor - never collides with any PlanetariaSphereCollider (when properly formed)
        /// </summary>
        /// <returns>A collider that never collides.</returns>
        public static PlanetariaSphereCollider never { get { return never_collide; } } // TODO: verify

        [SerializeField] private readonly Vector3 center_variable;
        [SerializeField] private readonly float radius_variable;

        static readonly PlanetariaSphereCollider always_collide = new PlanetariaSphereCollider(Vector3.zero, Mathf.Infinity);
        static readonly PlanetariaSphereCollider never_collide = new PlanetariaSphereCollider(Vector3.zero, Mathf.NegativeInfinity);
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