using System.Linq;

/// <summary>
/// A group of miscellaneous helper functions.
/// </summary>
public static class Miscellaneous
{
    /// <summary>
    /// Counts the true booleans in a sequence. E.g. count_true_booleans(true, false, true) returns 2.
    /// </summary>
    /// <param name="boolean_list">A comma separated list of (or array of) booleans.</param>
    /// <returns>The number of true values in the sequence.</returns>
    public static int count_true_booleans(params bool[] boolean_list)
    {
       return boolean_list.Count(is_true => is_true);
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