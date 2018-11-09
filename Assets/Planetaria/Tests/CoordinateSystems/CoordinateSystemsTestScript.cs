using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Planetaria;

public class CoordinateSystemsTestScripts
{
    [Test]
    public void CoordinateSystemsTestScriptsSimplePasses()
    {
        // Use the Assert class to test conditions.
        for (int test = 0; test < tests; ++test)
        {
            NormalizedCartesianCoordinates cartesian = new NormalizedCartesianCoordinates(Random.onUnitSphere);
            NormalizedCartesianCoordinates reconverted_cartesian;

            // Test cubemap
            CubeUVCoordinates cubemap = cartesian;
            reconverted_cartesian = cubemap;
            Assert.IsTrue(Miscellaneous.approximately(cartesian.data, reconverted_cartesian.data), cartesian.data + " versus " + reconverted_cartesian.data);

            // Test octahedron map
            OctahedronUVCoordinates octahedron_map = cartesian;
            reconverted_cartesian = octahedron_map;
            Assert.IsTrue(Miscellaneous.approximately(cartesian.data, reconverted_cartesian.data), cartesian.data + " versus " + reconverted_cartesian.data);

            // Test spherical coordinates
            NormalizedSphericalCoordinates sphere = cartesian;
            reconverted_cartesian = sphere;
            Assert.IsTrue(Miscellaneous.approximately(cartesian.data, reconverted_cartesian.data), cartesian.data + " versus " + reconverted_cartesian.data);

            // Test stereoscopic coordinates
            StereoscopicProjectionCoordinates stereoscopic = cartesian;
            reconverted_cartesian = stereoscopic;
            Assert.IsTrue(Miscellaneous.approximately(cartesian.data, reconverted_cartesian.data), cartesian.data + " versus " + reconverted_cartesian.data);

            // Test spherical rectangle
            float x1 = Random.Range(-Mathf.PI/10, +Mathf.PI/10); // FIXME: [-PI, +PI]
            float x2 = Random.Range(-Mathf.PI/10, +Mathf.PI/10);
            float y1 = Random.Range(-Mathf.PI/10, +Mathf.PI/10);
            float y2 = Random.Range(-Mathf.PI/10, +Mathf.PI/10);
            Vector2 minimum = new Vector2(Mathf.Min(x1, x2), Mathf.Min(y1, y2));
            Vector2 size = new Vector2(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
            Rect canvas = new Rect(minimum, size);
            SphericalRectangleUVCoordinates spherical_rectangle = cartesian.to_spherical_rectangle(canvas);
            reconverted_cartesian = spherical_rectangle;
            Assert.IsTrue(Miscellaneous.approximately(cartesian.data, reconverted_cartesian.data), cartesian.data + " versus " + reconverted_cartesian.data + " intermediate: " + spherical_rectangle.canvas.ToString("F4") + " " + spherical_rectangle.uv.ToString("F4"));
            
            // Test normalized octahedron
            NormalizedOctahedronCoordinates octahedron = cartesian;
            bool same_signs = Mathf.Sign(octahedron.data.x) == Mathf.Sign(cartesian.data.x) &&
                    Mathf.Sign(octahedron.data.y) == Mathf.Sign(cartesian.data.y) &&
                    Mathf.Sign(octahedron.data.z) == Mathf.Sign(cartesian.data.z);
            Assert.IsTrue(same_signs, cartesian.data + " versus " + reconverted_cartesian.data);

            // Test normalized cube
            NormalizedCubeCoordinates cube = cartesian;
            same_signs = Mathf.Sign(cube.data.x) == Mathf.Sign(cartesian.data.x) &&
                    Mathf.Sign(cube.data.y) == Mathf.Sign(cartesian.data.y) &&
                    Mathf.Sign(cube.data.z) == Mathf.Sign(cartesian.data.z);
            Assert.IsTrue(same_signs, cartesian.data + " versus " + reconverted_cartesian.data);
        }
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator CoordinateSystemsTestScriptsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }

    private int tests = 100;
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