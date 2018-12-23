using UnityEngine;
using Planetaria;

namespace Platformer
{
    public class Elevator : PlanetariaTracker
    {
        public override void cleanup() { }

        public override void setup()
        {
            Vector3 start_position = self.GetComponent<PlanetariaCollider>().shape.center_of_mass();
            NormalizedSphericalCoordinates spherical = new NormalizedCartesianCoordinates(start_position);
            NormalizedSphericalCoordinates shifted_spherical = new NormalizedSphericalCoordinates(spherical.elevation + Mathf.PI * 0.75f, spherical.azimuth);
            NormalizedCartesianCoordinates shifted_cartesian = shifted_spherical;
            Vector3 end_position = shifted_cartesian.data;

            rotator = Quaternion.FromToRotation(start_position, end_position);
        }

        public override void step()
        {
            float interpolation_fraction = Mathf.PingPong(Time.time / 10f, 1); // FIXME: AnimationCurve (repeat) with optional hook for buttons
            Quaternion intermediate_position = Quaternion.Slerp(Quaternion.identity, rotator, interpolation_fraction); // FIXME: needs to work >=180 degrees
            self.SetPositionAndDirection(intermediate_position * Vector3.forward, intermediate_position * Vector3.up);
        }

        public override void teleport() { }

        private Quaternion rotator;
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
