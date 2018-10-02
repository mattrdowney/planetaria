using UnityEngine;

namespace Planetaria
{
    public struct PlanetariaArcCollider
    {
        public bool collides_with(PlanetariaArcCollider other, Quaternion shift_from_self_to_other) // TODO: implement as Unity Job (multithreaded)
        {
            for (int self_index = 0; self_index < 3; ++self_index)
            {
                for (int other_index = 0; other_index < 3; ++other_index)
                {
                    if (!this[self_index].collides_with(other[other_index], shift_from_self_to_other))
                    {
                        Debug.Log("Failed ArcCollider collision on self:" + self_index + " other:" + other_index);
                        return false;
                    }
                }
            }
            Debug.Log("ArcCollider collision succeeded");
            return true;
        }

        public bool collides_with(PlanetariaSphereCollider other, Quaternion shift_from_self_to_other) // TODO: implement as Unity Job (multithreaded)
        {
            for (int self_index = 0; self_index < 3; ++self_index)
            {
                if (!this[self_index].collides_with(other, shift_from_self_to_other))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Constructor (Named) - The intersection of two spheres is a circle, and adding a third sphere generates a set of colliders that define an arc
        /// </summary>
        /// <param name="arc">The arc for which the colliders will be generated.</param>
        /// <returns>A set of three Spheres that define an arc collision.</returns>
        public static PlanetariaArcCollider block(Arc arc)
        {
            if (arc.type == GeometryType.ConcaveCorner)
            {
                return new PlanetariaArcCollider(PlanetariaSphereCollider.never, PlanetariaSphereCollider.never,
                        PlanetariaSphereCollider.never);
            }
            PlanetariaSphereCollider boundary_collider = PlanetariaArcColliderUtility.boundary_collider(arc);
            PlanetariaSphereCollider[] elevation_colliders = PlanetariaArcColliderUtility.elevation_collider(arc.floor());
            return new PlanetariaArcCollider(elevation_colliders[0], boundary_collider, elevation_colliders[1]);
        }

        /// <summary>
        /// Constructor (Named) - Finds a Sphere that include "point" at the boundary and has a center along "center_axis"
        /// </summary>
        /// <param name="center_axis">The axis of the center point. The resulting center point (in return value) may vary.</param>
        /// <param name="point">The furthest point to include.</param>
        /// <returns>A sphere centered along "center_axis" and including "point" at its boundary.</returns>
        public static PlanetariaSphereCollider boundary(Vector3 center_axis, Vector3 point)
        {
            return PlanetariaArcColliderUtility.boundary_collider(center_axis, point);
        }

        /// <summary>
        /// Constructor (Named) - The intersection of two spheres is a circle, and adding a third sphere generates a set of colliders that define an arc
        /// </summary>
        /// <param name="arc">The arc for which the field will be generated.</param>
        /// <returns>A sphere representing a field collision at a single edge.</returns>
        public static PlanetariaSphereCollider field(Arc arc)
        {
            if (arc.type >= GeometryType.ConvexCorner) // FIXME: (?) lazy
            {
                return PlanetariaSphereCollider.always;
            }
            return PlanetariaArcColliderUtility.uniform_collider(arc.floor());
        }

        public PlanetariaSphereCollider this[int index] // Encapsulate, do not expose as application programming interface (API)
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return circle_boundary_variable;
                    case 1:
                        return arc_boundary_variable;
                    case 2: default:
                        return circle_boundary_complement_variable;
                }
            }
        }

        private PlanetariaArcCollider(PlanetariaSphereCollider circle_boundary,
                PlanetariaSphereCollider arc_boundary, PlanetariaSphereCollider circle_boundary_complement)
        {
            circle_boundary_variable = circle_boundary;
            arc_boundary_variable = arc_boundary;
            circle_boundary_complement_variable = circle_boundary_complement;
        }

        private readonly PlanetariaSphereCollider circle_boundary_variable;
        private readonly PlanetariaSphereCollider arc_boundary_variable;
        private readonly PlanetariaSphereCollider circle_boundary_complement_variable;
    }

    internal class PlanetariaArcColliderUtility
    {
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
        public static PlanetariaSphereCollider ideal_collider(SphericalCap cap)
        {
            float distance = extrude_distance(cap, Precision.collider_extrusion);
            float tangent = Mathf.Tan(Mathf.Acos(distance));
            float secant = 1 / distance;

            return new PlanetariaSphereCollider(cap.normal * secant, tangent);
        }

        /// <summary>
        /// Constructor - envelops a SphericalCap region with a size-2 Sphere.
        /// </summary>
        /// <param name="transformation">An optional Transform that will be used to shift the center (if moved).</param>
        /// <param name="cap">The SphericalCap that will be encapsulated by a Sphere (collider).</param>
        /// <returns>A Sphere that can be used as a collider.</returns>
        public static PlanetariaSphereCollider uniform_collider(SphericalCap cap) // TODO: FIXME: documentation
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
            return new PlanetariaSphereCollider(cap.normal*axial_distance, 2);
        }

        internal static PlanetariaSphereCollider boundary_collider(Arc arc)
        {
            Vector3 corner = arc.begin(); // end() could also be used
            Vector3 center = arc.position(0); // Center of arc is at zero
            return boundary_collider(center, corner);
        }

        internal static PlanetariaSphereCollider boundary_collider(Vector3 center_axis, Vector3 point)
        {
            PlanetariaSphereCollider result;
            SphericalCap cap = SphericalCap.cap(center_axis, point); // create a SphericalCap centered at "center" that captures both corners (2nd implicitly)

            float real_angle = Vector3.Angle(center_axis, point) * Mathf.Deg2Rad;
            if (real_angle < Precision.max_sphere_radius) // use a r<=1 sphere
            {
                result = PlanetariaArcColliderUtility.ideal_collider(cap);
            }
            else // use r=2 sphere
            {
                result = PlanetariaArcColliderUtility.uniform_collider(cap);
            }

            if (result.radius < Precision.threshold)
            {
                return new PlanetariaSphereCollider(center_axis, 0);
            }
            return result;
        }

        internal static PlanetariaSphereCollider[] elevation_collider(SphericalCap cap)
        {
            if (cap.offset > 1f - Precision.threshold)
            {
                return new PlanetariaSphereCollider[2] { ideal_collider(cap), PlanetariaSphereCollider.always };
            }
            PlanetariaSphereCollider[] colliders = new PlanetariaSphereCollider[2];
            colliders[0] = uniform_collider(cap);
            colliders[1] = uniform_collider(cap.complement());
            return colliders;
        }

        internal static float extrude_distance(SphericalCap cap, float extrusion)
        {
            return Mathf.Cos(Mathf.Acos(Mathf.Clamp(cap.offset, -1, +1)) - extrusion);
        }
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