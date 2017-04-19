using UnityEngine;

public class PlanetariaTransform
{
    public NormalizedSphericalCoordinates position
    {
        get
        {
            return position_;
        }

        set
        {
            position_ = value; Move();
        }
    }

    public float rotation
    {
        get
        {
            return rotation_;
        }

        set
        {
            rotation_ = value; Move();
        }
    }

    public float scale
    {
        get
        {
            return scale_;
        }

        set
        {
            scale_ = value;
        }
    }

    private void Move()
    {
        Cartesian_transform.rotation = Quaternion.AngleAxis(rotation_, position_.data);
    }

    Transform Cartesian_transform;
    NormalizedSphericalCoordinates position_;
    float rotation_;
    float scale_;
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