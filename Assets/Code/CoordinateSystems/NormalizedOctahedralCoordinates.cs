using UnityEngine;

public struct NormalizedOctahedralCoordinates
{
    public Vector3 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    public NormalizedOctahedralCoordinates(Vector3 vector)
    {
        data_ = vector;
        Normalize();
    }

    public NormalizedOctahedralCoordinates(NormalizedCartesianCoordinates Cartesian)
    {
        data_ = Cartesian.data;
        Normalize();
    }

    public NormalizedOctahedralCoordinates(float x, float y, float z)
    {
        data_ = new Vector3(x, y, z);
        Normalize();
    }

    public static implicit operator NormalizedCartesianCoordinates(NormalizedOctahedralCoordinates octahedral)
    {
        return new NormalizedCartesianCoordinates(octahedral.data);
    }

    Vector3 data_;

    float magnitude()
    {
        return Mathf.Abs(data_.x) + Mathf.Abs(data_.x) + Mathf.Abs(data_.x);
    }

    void Normalize()
    {
        float length = magnitude();
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