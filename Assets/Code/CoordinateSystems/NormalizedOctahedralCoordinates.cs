using UnityEngine;

public struct NormalizedOctahedralCoordinates
{
    public Vector3 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    /// <summary>
    /// Constructor - Stores octahedral coordinates in a wrapper class.
    /// </summary>
    /// <param name="octahedral">The octahedral coordinates. Note: matches Unity's default Vector3 definition.</param>
    public NormalizedOctahedralCoordinates(Vector3 octahedral)
    {
        data_ = octahedral;
        Normalize();
    }

    /// <summary>
    /// Inspector - Converts octahedral coordinates into Cartesian coordinates.
    /// </summary>
    /// <param name="octahedral">The octahedral that will be converted</param>
    /// <returns>The Cartesian coordinates.</returns> 
    public static implicit operator NormalizedCartesianCoordinates(NormalizedOctahedralCoordinates octahedral)
    {
        return new NormalizedCartesianCoordinates(octahedral.data);
    }

    /// <summary>
    /// Inspector - Converts octahedral coordinates into octahedron UV coordinates.
    /// </summary>
    /// <param name="octahedral">The octahedral that will be converted</param>
    /// <returns>The UV coordinates of an octahedron.</returns> 
    public static implicit operator OctahedralUVCoordinates(NormalizedOctahedralCoordinates octahedral)
    {
        Vector2 UV = Octahedron.Cartesian_to_UV(octahedral.data);
        return new OctahedralUVCoordinates(UV.x, UV.y);
    }
    
    /// <summary>
    /// Inspector - Converts octahedral coordinates into spherical coordinates.
    /// </summary>
    /// <param name="octahedral">The octahedral that will be converted</param>
    /// <returns>The spherical coordinates.</returns> 
    public static implicit operator NormalizedSphericalCoordinates(NormalizedOctahedralCoordinates octahedral)
    {
        NormalizedCartesianCoordinates Cartesian = octahedral;
        return Cartesian;
    }

    Vector3 data_;

    /// <summary>
    /// Inspector - finds the Manhattan distance between two points
    /// </summary>
    /// <returns>The Manhattan distance.</returns>
    float Manhattan_distance()
    {
        return Mathf.Abs(data_.x) + Mathf.Abs(data_.x) + Mathf.Abs(data_.x);
    }

    /// <summary>
    /// Mutator - Normalizes Cartesian vector using Manhattan distance
    /// </summary>
    void Normalize()
    {
        float length = Manhattan_distance();
        float absolute_error = Mathf.Abs(length-1);
        if (absolute_error < Precision.tolerance)
        {
            return;
        }

        data_ /= length;
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