using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StereoscopicProjectionCoordinates : MonoBehaviour
{
    public Vector2 data
    {
        get { return data_; }
        set { data_ = value; }
    }

    /// <summary>
    /// Constructor - Stores stereoscopic projection coordinates in a wrapper class.
    /// </summary>
    /// <param name="stereoscopic_projection">The stereoscopic projection coordinates on plane z=0.</param>
    public StereoscopicProjectionCoordinates(Vector2 stereoscopic_projection)
    {
        data_ = stereoscopic_projection;
    }

    /// <summary>
    /// Inspector - Converts from stereoscopic projection coordinates to Cartesian coordinates.
    /// </summary>
    /// <param name="stereoscopic_projection">The coordinates of the stereoscopic projection on plane z=0.</param>
    /// <returns>The normalized Cartesian coordinates.</returns> 
    public static implicit operator NormalizedCartesianCoordinates(StereoscopicProjectionCoordinates stereoscopic_projection)
    {
        float X = stereoscopic_projection.data.x;
        float Y = stereoscopic_projection.data.y;

        float denominator = (1 + X*X + Y*Y);

        float x = (2 * X) / denominator;
        float y = (2 * Y) / denominator;
        float z = (-1 + X*X + Y*Y) / denominator;

        return new NormalizedCartesianCoordinates(new Vector3(x, y, z));
    }

    // TODO: implement remainder of conversions

    
    Vector2 data_;
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