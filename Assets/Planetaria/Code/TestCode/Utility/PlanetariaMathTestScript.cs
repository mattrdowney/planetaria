using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Planetaria;

public class PlanetariaMathTestScript
{
    [Test]
    public void PlanetariaMathTestScriptSimplePasses()
    {
        // PlanetariaMath.modolo_using_euclidean_division()
        for (int test = 0; test < tests; ++test)
        {
            float dividend = Random.Range(float.MinValue, float.MaxValue);
            float divisor = Random.Range(float.MinValue, float.MaxValue);
            if (divisor != 0)
            {
                float modulus = PlanetariaMath.modolo_using_euclidean_division(dividend, divisor);
                Assert.IsTrue(0 <= modulus && modulus < Mathf.Abs(divisor), dividend + " mod " + divisor + " = " + modulus);
            }
        }
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(0.9f,1) == 0.9f);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(1,1) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(2,1) == 0);
        // positive dividend, positive divisor
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(0,10) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(1,10) == 1);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(2,10) == 2);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(9,10) == 9);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(10,10) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(11,10) == 1);
        // negative dividend, positive divisor
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-1,10) == 9);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-2,10) == 8);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-9,10) == 1);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-10,10) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-11,10) == 9);
        // positive dividend, negative divisor
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(0,-10) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(1,-10) == 1);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(2,-10) == 2);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(9,-10) == 9);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(10,-10) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(11,-10) == 1);
        // negative dividend, negative divisor
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-1,-10) == 9);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-2,-10) == 8);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-9,-10) == 1);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-10,-10) == 0);
        Assert.IsTrue(PlanetariaMath.modolo_using_euclidean_division(-11,-10) == 9);

        //
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator PlanetariaMathTestScriptWithEnumeratorPasses()
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