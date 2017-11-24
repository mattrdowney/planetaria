using UnityEngine;

namespace Planetaria
{
    public static class Precision
    {
        public const float just_below_one = 0.99999994f; // largest IEEE float32 number below 1
        public static readonly float just_above_zero = 1 - just_below_one;
        public static readonly float delta = 4 * Mathf.Pow(2, -23);
        public const float threshold = 1e-5f;
        public const float tolerance = 1e-6f;
        public const int float_bits = 8 * sizeof(float);

        public const float near_clip_plane = 0.5f;
        public const float far_clip_plane = 2.0f;
        public const float clip_a = far_clip_plane / ( far_clip_plane - near_clip_plane );
        public const float clip_b = far_clip_plane * near_clip_plane / ( near_clip_plane - far_clip_plane );
        public const int z_buffer_digits = (1 << sizeof(short));
        public static readonly float octahedron_face_distance = 1/Mathf.Sqrt(3);
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