using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    [Serializable]
	public struct PolarCoordinates
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