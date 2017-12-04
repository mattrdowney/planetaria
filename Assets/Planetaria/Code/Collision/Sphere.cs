using UnityEngine;

namespace Planetaria
{
    public class Sphere
    {
        /// <summary>
        /// The Sphere collider that envelops a axial "point" with a given planetaria_radius.
        /// </summary>
        /// <param name="transformation">An optional Transform that will be used to shift the center (if moved).</param>
        /// <param name="axis">The center of the circle (both a point and an axis).</param>
        /// <param name="planetaria_radius">The angle along the surface from circle center to circle boundary.</param>
        /// <returns>A Sphere that can be used as a collider.</returns>
        public static Sphere collider(optional<Transform> transformation, Vector3 axis, float planetaria_radius)
        {
            // Background:
            // imagine two spheres:
            // "left" sphere is of radius 1 and placed at the origin (0,0,0).
            // "right" sphere is of radius 2 and is placed at (3,0,0).
            // These spheres intersect at a point (1,0,0).
            // If slid closer together they will intersect at a circle of radius (0,1].
            // The goal is to find the distance between spheres to create a collider along a given circle.

            // Derivation:
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
            // which, according to Wolfram Alpha is: cos(left_angle) + sqrt(sin2(left_angle) + 3)

            float hemisphere = Mathf.Sign(Mathf.PI/2 - planetaria_radius); // determines orientation of sphere collisions
            if (hemisphere == -1)
            {
                planetaria_radius = Mathf.PI - planetaria_radius; // get supplementary angle (on other hemisphere)
            }
            planetaria_radius += Precision.collider_extrusion; // Collisions can theoretically (and practically) be missed due to rounding errors.

            float x = Mathf.Cos(planetaria_radius);
            float axial_distance = x + hemisphere*Mathf.Sqrt(x*x + 3);
            return new Sphere(transformation, axis*axial_distance, 2);
        }

        public static Sphere sphere(optional<Transform> transformation, Vector3 center, float radius)
        {
            return new Sphere(transformation, center, radius);
        }

        public float radius { get; private set; }
        public Vector3 center
        {
            get
            {
                if (transformation.exists)
                {
                    if (transformation.data.rotation == cached_rotation)
                    {
                        return cached_center;
                    }
                    cached_rotation = transformation.data.rotation;
                    // no need to adjust position if both objects are on the same Planetarium
                    cached_center = /* transformation.position + */ cached_rotation * center_variable; // TODO: check if matrix multiplication or built-in function does this faster
                    return cached_center;
                }
                return center_variable;
            }
            set
            {
                center_variable = value;
            }
        }
        public Vector3 debug_center
        {
            get
            {
                if (transformation.exists)
                {
                    return transformation.data.position + transformation.data.rotation * center_variable;
                }
                return center_variable;
            }
        }

        private Sphere(optional<Transform> transformation, Vector3 center, float radius)
        {
            this.transformation = transformation;
            this.center = center;
            this.radius = radius;

            this.cached_rotation = transformation.exists ? transformation.data.rotation : Quaternion.identity;
            this.cached_center = cached_rotation * center_variable;
        }

        private optional<Transform> transformation { get; set; }
        private Vector3 center_variable;
        private Quaternion cached_rotation;
        private Vector3 cached_center;
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