using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// BarycentricCoordinates system for spherical triangles. Primarily intended for UV interpolation in spherical coordinates.
    /// </summary>
    [Serializable]
	public struct SphericalBarycentricCoordinates // http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.61.3792 // some positive aspects, and negative aspects
	{
        // TODO: implement and see if this works for exact UV coordinates for tesselated octahedrons (between 8 and ~500 triangles when the math works out imprecisely)
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