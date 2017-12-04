using UnityEngine;

namespace Planetaria
{
    public class Sphere
    {
        public static Sphere sphere(optional<Transform> transformation, Vector3 position, float radius)
        {
            return new Sphere(transformation, position, radius);
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