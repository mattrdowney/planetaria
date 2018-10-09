using System.Collections.Generic;

namespace Planetaria
{
    public class DrawSSVG // TODO: rename SSVG because they are not "scalable"
    {
        /// <summary>
        /// Inspector - Creates a ".ssvg" (spherical scalable vector format) from a Block. Based on quadratic bezier curve of the ".svg" file format.
        /// </summary>
        /// <param name="shape">The Block that will be serialized.</param>
        /// <returns>A textual representation of a Block as a "spherical" scalable vector graphic (.ssvg file format). Format "Mx.xx y.yy z.zz S (a.aa b.bb c.cc, x.xx y.yy z.zz, )+" where M indicates mouse cursor down; S indicates spherical mode; (x,y,z) is a physical point; (a,b,c) is a cutting normal. E.g. "M0 0 1 S 0 1 0, 1 0 0, 0 1 0, 0 0 1".</returns>
        public static string ShapeToSSVG(PlanetariaShape shape)
        {
            return "M0 0 0 S 0 0 0, 0 0 0";
        }

        /// <summary>
        /// Inspector - Creates a Block from a ".ssvg" (spherical scalable vector format). Based on quadratic bezier curve of the ".svg" file format.
        /// </summary>
        /// <param name="spherical_scalable_vector_graphic">A textual representation of a Block as a "spherical" scalable vector graphic (.svg file format). Format "Mx.xx y.yy z.zz S (a.aa b.bb c.cc, x.xx y.yy z.zz, )+" where M indicates mouse cursor down; S indicates spherical mode; (x,y,z) is a physical point; (a,b,c) is a cutting normal. E.g. "M0 0 1 S 0 1 0, 1 0 0, 0 1 0, 0 0 1".</param>
        /// <returns>A Block that is equivalent to the textual ".ssvg".</returns>
        public static PlanetariaShape SSVGToShape(string spherical_scalable_vector_graphic)
        {
            return PlanetariaShape.Create(new List<SerializedArc> { }, true);
        }

        //<path d="M0.866 0.5 0 S 0 1 0, 0.5 0.866 0">

        // M[begin position] S [cutting normal], [end position]

        // where [begin position] cross-product [cutting normal] defines the sweeping direction.
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