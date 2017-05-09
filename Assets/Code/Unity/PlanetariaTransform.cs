using UnityEngine;

public class PlanetariaTransform
{
    public PlanetariaTransform(Transform internal_transform)
    {
        Cartesian_transform = internal_transform;
    }

    public NormalizedSphericalCoordinates position
    {
        get
        {
            return position_;
        }

        set
        {
            dirty_position = true;
            previous_position_ = position_;
            position_ = value;
        }
    }

    public NormalizedSphericalCoordinates previous_position
    {
        get
        {
            return position_;
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
            dirty_position = true;
            rotation_ = value;
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
            dirty_scale = true;
            scale_ = value;
        }
    }

    public void Move()
    {
        if (dirty_position)
        {
            Cartesian_transform.rotation = Quaternion.AngleAxis(rotation_, position_.data);
            dirty_position = false;
        }

        if (dirty_scale)
        {
            Cartesian_transform.localScale = Mathf.Sin(scale)*Vector3.one;
            dirty_scale = false;
        }
    }

    Transform Cartesian_transform;
    NormalizedSphericalCoordinates position_;
    float rotation_;
    float scale_;

    bool dirty_position;
    bool dirty_scale;

    NormalizedSphericalCoordinates previous_position_;
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