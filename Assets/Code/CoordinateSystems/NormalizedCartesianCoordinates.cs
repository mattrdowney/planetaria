using UnityEngine;

public class NormalizedCartesianCoordinates
{
    public Vector3 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    public NormalizedCartesianCoordinates(Vector3 vector)
    {
        data_ = vector;
        Normalize(); 
    }

    public NormalizedCartesianCoordinates(float x, float y, float z)
    {
        data_ = new Vector3(x, y, z);
        Normalize(); 
    }

    public static implicit operator NormalizedOctahedralCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        return new NormalizedOctahedralCoordinates(Cartesian.data);
    }

    
    public static implicit operator NormalizedSphericalCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        float inclination = Mathf.Acos(Cartesian.data.y);
        float azimuth = Mathf.Atan2(Cartesian.data.z, Cartesian.data.x);
        return new NormalizedSphericalCoordinates(inclination, azimuth);
    }

    public static implicit operator UVCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        float u = (0.5f + Mathf.Atan2(Cartesian.data.z, Cartesian.data.x)) / (2*Mathf.PI);
        float v = 0.5f - Mathf.Asin(Cartesian.data.y) / Mathf.PI;
        return new UVCoordinates(u, v);
    }

    Vector3 data_;

    float magnitude_squared()
    {
        return Mathf.Abs(data_.x) * Mathf.Abs(data_.x) +
                Mathf.Abs(data_.y) * Mathf.Abs(data_.y) +
                Mathf.Abs(data_.z) * Mathf.Abs(data_.z);
    }

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