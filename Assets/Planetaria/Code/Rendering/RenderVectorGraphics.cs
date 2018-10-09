using UnityEngine;

namespace Planetaria
{
    public static class RenderVectorGraphics
    {
        public static Material render(TextAsset svg)
        {
            Material result = new Material(Shader.Find("Unlit/Transparent"));

            ISVGDevice rendering_device = new SVGDeviceFast();
            Implement rendering_implementation = new Implement(svg, rendering_device);
            rendering_implementation.StartProcess();

            Texture2D renderered_svg = rendering_implementation.GetTexture();
            renderered_svg.wrapMode   = TextureWrapMode.Clamp;
            renderered_svg.filterMode =  FilterMode.Bilinear;
            renderered_svg.anisoLevel = 0;
            result.mainTexture = renderered_svg;

            return result;
        }
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