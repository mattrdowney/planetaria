using UnityEngine;

public class NormalizedCartesianCoordinates
{
    public Vector3 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    /// <summary>
    /// Constructor - Stores Cartesian coordinates in a wrapper class.
    /// </summary>
    /// <param name="Cartesian">The Cartesian coordinates. Note: matches Unity's default Vector3 definition.</param>
    public NormalizedCartesianCoordinates(Vector3 Cartesian) // Note: this is an example of where there is ambiguity between variables and types under new naming convention
    {
        data_ = Cartesian;
        Normalize(); 
    }

    /// <summary>
    /// Inspector - Converts Cartesian coordinates into octahedral coordinates.
    /// </summary>
    /// <param name="Cartesian">The coordinates in Cartesian space that will be converted</param>
    /// <returns>The octahedral coordinates.</returns> 
    public static implicit operator NormalizedOctahedralCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        return new NormalizedOctahedralCoordinates(Cartesian.data);
    }

    /// <summary>
    /// Inspector - Converts Cartesian coordinates into spherical coordinates.
    /// </summary>
    /// <param name="Cartesian">The coordinates in Cartesian space that will be converted</param>
    /// <returns>The spherical coordinates.</returns> 
    public static implicit operator NormalizedSphericalCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        float elevation = Mathf.Acos(-Cartesian.data.y);
        float azimuth = Mathf.Atan2(Cartesian.data.z, Cartesian.data.x);
        return new NormalizedSphericalCoordinates(elevation, azimuth);
    }

    /// <summary>
    /// Inspector - Converts Cartesian coordinates into octahedron UV space.
    /// </summary>
    /// <param name="Cartesian">The coordinates in Cartesian space that will be converted</param>
    /// <returns>The UV coordinates for an octahedron.</returns> 
    public static implicit operator OctahedralUVCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        NormalizedOctahedralCoordinates octahedral = Cartesian;
        return octahedral;
    }

    Vector3 data_;

    /// <summary>
    /// Inspector - Find the distance from the origin (i.e. magnitude) times itself.
    /// </summary>
    /// <returns>The magnitude squared, where the magnitude is the distance from the origin.</returns>
    float magnitude_squared()
    {
        return Mathf.Abs(data_.x) * Mathf.Abs(data_.x) +
                Mathf.Abs(data_.y) * Mathf.Abs(data_.y) +
                Mathf.Abs(data_.z) * Mathf.Abs(data_.z);
    }

    /// <summary>
    /// Mutator - Project the Cartesian coordinates onto a unit sphere.
    /// </summary>
    void Normalize()
    {
        float approximate_length = magnitude_squared();
        float approximate_error = Mathf.Abs(approximate_length-1);
        if (approximate_error < Precision.tolerance)
        {
            return;
        }
 
        float length = (float) Mathf.Sqrt(approximate_length);
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