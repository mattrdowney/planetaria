using UnityEngine;

namespace Planetaria
{
    public class ColorBlender
    {
        public ColorBlender() { }

        public void blend(Color color)
        {
            red += color.r;
            green += color.g;
            blue += color.b;
            alpha += color.a;
            ++colors;
        }

        public static implicit operator Color(ColorBlender blender)
        {
            Color result = new Color(); // TODO: make sure this returns transparent by default
            if (blender.colors > 0)
            {
                result.r = blender.red/blender.colors;
                result.g = blender.green/blender.colors;
                result.b = blender.blue/blender.colors;
                result.a = blender.alpha/blender.colors;
            }
            return result;
        }

        private float red = 0;
        private float green = 0;
        private float blue = 0;
        private float alpha = 0; // FIXME: how would you even blend alpha the right way? strict averages get weird
        private int colors = 0;
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