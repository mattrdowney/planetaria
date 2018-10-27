using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Planetaria;

public class MiscellaneousTestScript
{
    [Test]
    public void MiscellaneousTestScriptSimplePasses()
    {
        // Vector approximate equality comparison (with epsilon)
        Assert.IsTrue(Miscellaneous.approximately(Vector3.forward, Vector3.forward));
        Assert.IsFalse(Miscellaneous.approximately(Vector3.forward, Vector3.back));

        // Boolean counter
        Assert.IsTrue(Miscellaneous.count_true_booleans() == 0);
        Assert.IsTrue(Miscellaneous.count_true_booleans(false) == 0);
        Assert.IsTrue(Miscellaneous.count_true_booleans(true) == 1);
        Assert.IsTrue(Miscellaneous.count_true_booleans(false, false) == 0);
        Assert.IsTrue(Miscellaneous.count_true_booleans(true, true) == 2);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator MiscellaneousTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
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