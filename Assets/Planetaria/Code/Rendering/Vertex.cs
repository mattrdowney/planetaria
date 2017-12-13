using UnityEngine;

namespace Planetaria
{
    public struct Vertex
    {
        public Vertex generate(OctahedralUVCoordinates coordinates, ushort unique_identifier)
        {
            return new Vertex(coordinates, unique_identifier);
        }

        private Vertex(OctahedralUVCoordinates coordinates, ushort unique_identifier)
        {
            position = ((NormalizedCartesianCoordinates) coordinates).data;
            uv = coordinates.data;
            identifier = unique_identifier;
        }

        public Vector3 position { get; private set; }
        public Vector2 uv { get; private set; }
        public ushort identifier { get; private set; }
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