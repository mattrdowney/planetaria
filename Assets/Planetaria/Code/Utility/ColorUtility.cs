using UnityEngine;

public static class ColorUtility
{
    // Helper functions
    // RGB to CIE-1931
    // Closest color to neighboring color[] definitions.
    // Black (different hue) always maps to exactly black
    // Transparent (different hue/intensity) always maps to exactly transparent
    // This means that CIE distances are scaled by intensity and opacity
    // In theory there would be non-uniform scaling by intensity and hue (for pupil color absorption), but that's not too important
    // CIE / intensity / opacity should have approximately equal weighting.
    // Can be stored in a vector4(X*intensity*opacity, Y*intensity*opacity, intensity[0,1]*opacity, opacity[0,1])

    // Over-engineering? I still think this is interesting enough to do for the hell of it
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