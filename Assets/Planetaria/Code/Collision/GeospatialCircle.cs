using UnityEngine;

namespace Planetaria
{
    public struct GeospatialCircle
    {
        public Vector3 center { get; private set; }
        public float radius { get; private set; }

        public static GeospatialCircle circle(Vector3 center, float radius)
        {
            return new GeospatialCircle(center, radius);
        }

        private GeospatialCircle(Vector3 center, float radius)
        {
            this.center = center.normalized;
            this.radius = Mathf.Clamp(radius, -Mathf.PI/2, Mathf.PI/2);
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