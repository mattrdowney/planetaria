using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    [Serializable]
	public struct PolarCoordinates // TODO: combine PolarCoordinates with NormalizedSphericalCoordinates (as LongitudeLatitudeCoordinates)?
	{
		// Properties (Public)

        public float angle
        {
            get
            {
                return angle_variable;
            }
        }

        public float radius
        {
            get
            {
                return radius_variable;
            }
        }
		
		// Methods (Public)

        /// <summary>
        /// Inspector - Converts polar coordinates to normalized cartesian coordinates.
        /// </summary>
        /// <param name="polar">The polar coordinates (radians) to be converted relative to Quaternion.identity's forward.</param>
        /// <returns>The normalized cartesian coordinates (point on a unit-sphere).</returns>
        public static implicit operator NormalizedCartesianCoordinates(PolarCoordinates polar)
        {
            Vector3 direction = Bearing.bearing(Vector3.forward, Vector3.up, polar.angle);
            Vector3 cartesian = PlanetariaMath.spherical_linear_interpolation(Vector3.forward, direction, polar.radius);
            return new NormalizedCartesianCoordinates(cartesian);
        }

        /// <summary>
        /// Inspector - Creates a spherical circle QV (Quadrant-ValenceShell) coordinate from a point (implicit caller) and a radius (explicit).
        /// </summary>
        /// <param name="radius">A radius (measuring radians) representing the angle of the circle from Vector3.forward. Range: [0, +PI].</param>
        /// <returns>QV (Quadrant-ValenceShell) Coordinates for a spherical circle.</returns>
        public SphericalCircleQVCoordinates to_spherical_circle(float radius)
        {
            return SphericalCircleQVCoordinates.polar_to_spherical_circle(this, radius);
        }

        /// <summary>
        /// Constructor (Named) - A wrapper class for polar coordinates made from a cartesian point (x,y).
        /// </summary>
        /// <param name="cartesian">The cartesian coordinates to be converted.</param>
        /// <returns>The polar coordinates (wrapper struct).</returns>
        public static PolarCoordinates rectangular(Vector2 cartesian)
        {
            return new PolarCoordinates(cartesian.magnitude, Mathf.Atan2(cartesian.y, cartesian.x));
        }

        /// <summary>
        /// Constructor (Named) - A wrapper class for polar coordinates made from a radius and an angle.
        /// </summary>
        /// <param name="radius">The radius of the point in polar coordinates.</param>
        /// <param name="angle">The angle of the point in radians.</param>
        /// <returns>The polar coordinates (wrapper struct).</returns>
        public static PolarCoordinates polar(float radius, float angle)
        {
            return new PolarCoordinates(radius, angle);
        }

		// Methods (Non-Public)

        /// <summary>
        /// Constructor (Named) - A wrapper class for polar coordinates.
        /// </summary>
        /// <param name="radius">The radius of the point in polar coordinates.</param>
        /// <param name="angle">The angle of the point in radians.</param>
        /// <returns>The polar coordinates (wrapper struct).</returns>
        private PolarCoordinates(float radius, float angle)
        {
            radius_variable = radius;
            angle_variable = angle;
        }

		// Variables (non-Public)
		
        [SerializeField] private float radius_variable;
        [SerializeField] private float angle_variable;
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