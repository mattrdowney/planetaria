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
    /// <param name="vector">The Cartesian coordinates. Note: matches Unity's default Vector3 definition.</param>
    public NormalizedCartesianCoordinates(Vector3 Cartesian)
    {
        data_ = Cartesian;
        Normalize(); 
    }

    /// <summary>
    /// Constructor - Stores Cartesian coordinates in a wrapper class.
    /// </summary>
    /// <param name="x">The Cartesian x coordinate. Negative is left; positive is right.</param>
    /// <param name="y">The Cartesian y coordinate. Negative is down; positive is up.</param>
    /// <param name="z">The Cartesian z coordinate. Negative is backward; positive is forward.</param>
    public NormalizedCartesianCoordinates(float x, float y, float z)
    {
        data_ = new Vector3(x, y, z);
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
    /// <returns>The UV coordinates for a octahedron.</returns> 
    public static implicit operator OctahedralUVCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        Vector2 UV;
        Vector2 xPivot, yPivot, zPivot;

        NormalizedOctahedralCoordinates octahedral = Cartesian;

        if (System.Math.Sign(octahedral.data.x) == +1) //FIXME: optimize this hardcoded stuff, Barycentric coordinate conversions are certainly capable of being elegant
        {
            if(System.Math.Sign(octahedral.data.z) == +1)
            {
                yPivot = new Vector2(1.0f, 1.0f);
                zPivot = new Vector2(0.5f, 1.0f);
            }
            else
            {
                yPivot = new Vector2(1.0f, 0.0f);
                zPivot = new Vector2(0.5f, 0.0f);
            }
            xPivot = new Vector2(1.0f, 0.5f);
        }
        else
        {
            if (System.Math.Sign(octahedral.data.z) == +1)
            {
                yPivot = new Vector2(0.0f, 1.0f);
                zPivot = new Vector2(0.5f, 1.0f);
            }
            else
            {
                yPivot = new Vector2(0.0f, 0.0f);
                zPivot = new Vector2(0.5f, 0.0f);
            }
            xPivot = new Vector2(0.0f, 0.5f);
        }
        if(System.Math.Sign(octahedral.data.y) == +1)
        {
            yPivot = new Vector2(0.5f, 0.5f);
        }
        UV = xPivot*Mathf.Abs(octahedral.data.x) + yPivot* Mathf.Abs(octahedral.data.y) + zPivot* Mathf.Abs(octahedral.data.z);
        return new OctahedralUVCoordinates(UV.x, UV.y);
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