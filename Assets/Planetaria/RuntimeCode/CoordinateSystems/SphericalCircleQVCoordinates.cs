using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct SphericalCircleQVCoordinates
    {
        public Vector2 qv
        {
            get { return qv_variable; }
        }

        public float radius
        {
            get { return radius_variable; }
        }

        public bool valid() // instead, create a QV clamp/wrap mechanism based on an input parameter e.g. QVMode.Clamp, QVMode.Wrap
        {
            return 0 <= qv.x && qv.x < 4 &&
                    0 <= qv.y && qv.y <= 1;
        }

        /// <summary>
        /// Inspector - Find the UV coordinates given the QV coordinates and a resolution (in pixels).
        /// </summary>
        /// <param name="resolution">The width or height of the texture (in pixels). Assert: width = height = 2n (even # of pixels).</param>
        /// <returns>The texture coordinates (two integer indices as floats) for the given texture (not quite the UV point).</returns>
        public Vector2 to_texture_coordinate(int resolution) // FIXME: elegance based on quadrant/sign/reflection
        {
            int full_radius = resolution/2; // NOTE: if resolution is odd, the algorithm should cut off the last row and column of pixels.
            int valence_radius = Mathf.FloorToInt(qv.y*full_radius);
            int pixels_per_quadrant = (1 + 2*(valence_radius)); // (1, 3, 5, 7, 9, 11... ) - resolution is variable with radius
            int pixel_order = Mathf.FloorToInt(qv.x*pixels_per_quadrant);
            int u = 0;
            int v = 0;
            if (pixel_order <= pixels_per_quadrant/2) // angle = [0, ~PI/4]
            {
                u = (full_radius + valence_radius); // max = (resolution/2) + (resolution/2-1)
                v = full_radius + pixel_order; // max = (resolution/2) + (perimeter/8 or resolution/2)
            }
            else if (pixel_order <= 3*pixels_per_quadrant/2) // angle = [~PI/4, ~3PI/4]
            {
                u = (full_radius + valence_radius) - (pixel_order - pixels_per_quadrant/2); // min = (resolution - 1) - (3*perimeter/2 - 1*perimeter/2 or ~resolution)
                v = (full_radius + valence_radius);
            }
            else if (pixel_order <= 5*pixels_per_quadrant/2) // angle = [~3PI/4, ~5PI/4]
            {
                u = (full_radius - valence_radius - 1); // min = (resolution/2-1) - (resolution/2-1)
                v = (full_radius + valence_radius) - (pixel_order - 3*pixels_per_quadrant/2);
            }
            else if (pixel_order <= 7*pixels_per_quadrant/2) // angle = [~5PI/4, ~7PI/4]
            {
                u = (full_radius - valence_radius - 1) + (pixel_order - 5*pixels_per_quadrant/2); // max = (7*perimeter/2 - 5*perimeter/2)
                v = (full_radius - valence_radius - 1);
            }
            else // angle = [~7PI/4, 2PI)
            {
                u = (full_radius + valence_radius);
                v = (full_radius - valence_radius - 1) + (pixel_order - 7*pixels_per_quadrant/2);
            }
            return new Vector2(u, v); 
        }

        /// <summary>
        /// Constructor - Stores the spherical circle's QV (Quadrant-ValenceShell) coordinates in a wrapper class. Q Range: [0- 4+) - representing the four quadrants - the decimal portion representing their angle; V Range [0, 1) - representing the distance from the center of the texture.
        /// </summary>
        /// <param name="qv">The (Quadrant-ValenceShell) coordinates. Q Range: [0- 4+) - representing the four quadrants - the decimal portion representing their angle; V Range [0, 1) - representing the distance from the center of the texture.</param>
        /// <param name="radius">The radius of the spherical circle. Range: [0, +PI].</param>
        public SphericalCircleQVCoordinates(Vector2 qv, float radius)
        {
            qv_variable = qv;
            radius_variable = radius;
        }

        /// <summary>
        /// Inspector - Converts spherical circle QV coordinates into polar coordinates.
        /// </summary>
        /// <param name="qv">The coordinates in spherical circle QV space that will be converted.</param>
        /// <returns>The polar coordinates on the surface of a unit-sphere (relative to Quaternion.identity's forward).</returns>
        public static implicit operator PolarCoordinates(SphericalCircleQVCoordinates qv)
        {
            return spherical_circle_to_polar(qv.qv_variable, qv.radius_variable);
        }

        /// <summary>
        /// Inspector (Cache mutator) - Creates a spherical circle QV (Quadrant-ValenceShell) coordinate set from a point on a unit sphere and a circle radius.
        /// </summary>
        /// <param name="polar">Polar coordinates on the surface of a unit-sphere (relative to Quaternion.identity's forward).</param>
        /// <param name="radius">The radius of the spherical circle. Range: [0, +PI].</param>
        /// <returns>QV Coordinates for a spherical circle. Q Range: [0, 4) - representing the four quadrants - the decimal portion representing their angle; V Range [0, 1+) - representing the distance from the center of the texture - or valence shell.</returns>
        public static SphericalCircleQVCoordinates polar_to_spherical_circle(PolarCoordinates polar, float radius)
        {
            float quadrant = 4 * (polar.angle / (Mathf.PI*2));
            float valence_shell = polar.radius / radius;
            return new SphericalCircleQVCoordinates(new Vector2(quadrant, valence_shell), radius);
        }

        /// <summary>
        /// Inspector (Cache mutator) - Creates a polar coordinates point on the surface of a unit-sphere from a qv coordinate and a circle radius.
        /// </summary>
        /// <param name="qv">QV Coordinates for a spherical circle. Q Range: [0- 4+) - representing the four quadrants - the decimal portion representing their angle; V Range [0, 1) - representing the distance from the center of the texture.</param>
        /// <param name="radius">The radius of the spherical circle. Range: [0, +PI].</param>
        /// <returns>The polar coordinates on the surface of a unit-sphere (relative to Quaternion.identity's forward).</returns>
        public static PolarCoordinates spherical_circle_to_polar(Vector2 qv, float radius)
        {
            float angle = ( (qv.x/4) * (Mathf.PI*2) );
            float polar_radius = qv.y * radius;

            return PolarCoordinates.polar(polar_radius, angle);
        }

        // CONSIDER: re-add normalize() for scaling width/height? Clamp vs Wrap

        [SerializeField] private Vector2 qv_variable; // quadrant: [0,4); valence_shell (previously velocity) [0,1+]
        [SerializeField] private float radius_variable;
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