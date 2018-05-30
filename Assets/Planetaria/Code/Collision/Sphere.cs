using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct Sphere
    {
        public Vector3 center
        {
            get
            {
                if (transform_variable.exists)
                {
                    return transform_variable.data.rotation * center_variable;
                }
                return center_variable;
            }
        }

        public Vector3 debug_center
        {
            get
            {
                if (transform_variable.exists)
                {
                    return transform_variable.data.position + transform_variable.data.rotation * center_variable;
                }
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

        /// <summary>
        /// The intersection of two spheres is a circle, and adding a third sphere generates a set of colliders that define an arc
        /// </summary>
        /// <param name="transformation">For static objects no transform is better, otherwise the Transform-relative movement will be considered for dynamic objects.</param>
        /// <param name="arc">The arc for which the colliders will be generated.</param>
        /// <returns>A set of three Spheres that define an arc collision.</returns>
        public static Sphere[] arc_collider(optional<Transform> transformation, Arc arc) // FIXME: delegation, remove redundancy
        {
            Sphere[] boundary_colliders = boundary_collider(transformation, arc);
            Sphere[] elevation_colliders = elevation_collider(transformation, arc.floor());
            Sphere[] colliders = new Sphere[boundary_colliders.Length + elevation_colliders.Length];
            Array.Copy(elevation_colliders, colliders, elevation_colliders.Length);
            Array.Copy(boundary_colliders, 0, colliders, elevation_colliders.Length, boundary_colliders.Length);
            return colliders;
        }
        
        /// <summary>
        /// Constructor - envelops a SphericalCap region using focal point of cone/SphericalCap (perfect fit)
        /// </summary>
        /// <param name="transformation">An optional Transform that will be used to shift the center (if moved).</param>
        /// <param name="cap">The SphericalCap that will be encapsulated by a Sphere (collider).</param>
        /// <returns>A Sphere that can be used as a collider.</returns>
        /// <details>
        /// This collider has the special property that all sphere colliders will only collide if the planetaria sphere is intersecting.
        /// This is because the sphere is formed at the focal point of a cone from the planetaria sphere's tangent lines.
        /// </details>
        public static Sphere ideal_collider(optional<Transform> transformation, SphericalCap cap)
        {
            float distance = extrude_distance(cap, Precision.collider_extrusion);
            float tangent = Mathf.Tan(Mathf.Acos(distance));
            float secant = 1/distance;

            return new Sphere(transformation, cap.normal*secant, tangent);
        }

        /// <summary>
        /// Constructor - envelops a SphericalCap region with a size-2 Sphere.
        /// </summary>
        /// <param name="transformation">An optional Transform that will be used to shift the center (if moved).</param>
        /// <param name="cap">The SphericalCap that will be encapsulated by a Sphere (collider).</param>
        /// <returns>A Sphere that can be used as a collider.</returns>
        public static Sphere uniform_collider(optional<Transform> transformation, SphericalCap cap) // TODO: FIXME: documentation
        {
        // Background 1:
        // imagine two spheres:
        // "left" sphere is of radius 1 and placed at the origin (0,0,0).
        // "right" sphere is of radius 2 and is placed at (3,0,0).
        // These spheres intersect at a point (1,0,0).
        // If slid closer together they will intersect at a circle of radius (0,1].
        // The goal is to find the distance between spheres to create a collider along a given circle.

        // Derivation 1:
        // I created four variables:
        // left_height = sin(left_angle)
        // left_distance = 1 - cos(left_angle)
        // right_height = 2sin(right_angle)
        // right_distance = 2 - 2cos(right_angle)

        // While we are trying to find 3 - sum(distances), we know that left_height must equal right_height for the intersection to be valid
        // Therefore sin(left_angle) = left_height = right_height = 2sin(right_angle)
        // Since left_angle is the planetaria_radius (because we are dealing with a unit sphere) we can solve for right_angle:
        // right_angle = arcsin(sin(left_angle)/2)

        // Now that we know right_angle and left_angle we can solve all formulas:
        // left_distance = 1 - cos(left_angle)
        // right_distance = 2 - 2cos(arcsin(sin(left_angle)/2))
        // Next we just subtract these differences from 3 (the radii sum):
        // axial_distance = 3 - [1 - cos(left_angle)] - [2 - 2cos(arcsin(sin(left_angle)/2))]
        // axial_distance = 3 - 1 + cos(left_angle) - 2 + 2cos(arcsin(sin(left_angle)/2))
        // axial_distance = cos(left_angle) + 2cos(arcsin(sin(left_angle)/2))
        // which, according to Wolfram Alpha is: cos(left_angle) + sqrt(cos^2(left_angle) + 3)

        // Check negative values:
        // Background:
        // imagine two spheres:
        // "left" sphere is of radius 1 and placed at the origin (0,0,0).
        // "right" sphere is of radius 2 and is placed at (1,0,0).
        // These spheres intersect at a point (-1,0,0).
        // If slid further away they will intersect at a circle of radius (0,1].
        // Testing:
        // An input of x=-1 should result in axial_distance=1 (the center of the size-2 sphere):
        // Plugging this into our equation we get x + sqrt(x*x + 3) = -1 + sqrt(-1*-1 + 3) = -1 + sqrt(4) = -1 + 2 = 1
        // which is indeed equal to 1

            float x = extrude_distance(cap, Precision.collider_extrusion); // collisions can be missed for small objects due to floating point error
            float axial_distance = x + Mathf.Sqrt(x*x + 3);
            return new Sphere(transformation, cap.normal*axial_distance, 2);
        }

        private static Sphere[] boundary_collider(optional<Transform> transformation, Arc arc)
        {
            Sphere result;
            Vector3 corner = arc.begin();
            Vector3 center = arc.position(arc.angle()/2);
            SphericalCap cap = SphericalCap.cap(center, corner); // create a SphericalCap centered at "center" that captures both corners (2nd implicitly)

            float real_angle = Vector3.Angle(center, corner) * Mathf.Deg2Rad;
            if (real_angle < Precision.max_sphere_radius) // use a r<=1 sphere
            {
                result = ideal_collider(transformation, cap);
            }
            else // use r=2 sphere
            {
                result = uniform_collider(transformation, cap);
            }

            if (result.radius < Precision.threshold)
            {
                return new Sphere[0];
            }
            return new Sphere[1] { result };
        }

        private static Sphere[] elevation_collider(optional<Transform> transformation, SphericalCap cap)
        {
            if (cap.offset > 1f - Precision.threshold)
            {
                return new Sphere[1] { Sphere.ideal_collider(transformation, cap) };
            }
            Sphere[] colliders = new Sphere[2];
            colliders[0] = Sphere.uniform_collider(transformation, cap);
            colliders[1] = Sphere.uniform_collider(transformation, cap.complement());
            return colliders;
        }

        private static float extrude_distance(SphericalCap cap, float extrusion)
        {
            return Mathf.Cos(Mathf.Acos(Mathf.Clamp(cap.offset, -1, +1)) - extrusion);
        }

        private Sphere(optional<Transform> transformation, Vector3 center, float radius)
        {
            transform_variable = transformation;
            center_variable = center;
            radius_variable = radius;
        }

        [SerializeField] private optional<Transform> transform_variable;
        [SerializeField] private Vector3 center_variable;
        [SerializeField] private float radius_variable;
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