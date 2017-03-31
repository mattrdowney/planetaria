using UnityEngine;

public class OctahedralUVCoordinates
{
	public Vector2 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    /// <summary>
    /// Constructor - Stores the octahedron's UV coordinates in a wrapper class.
    /// </summary>
    /// <param name="u">The u coordinate in UV space for the octahedron. Range: [0,1]</param>
    /// <param name="v">The v coordinate in UV space for the octahedron. Range: [0,1]</param>
    public OctahedralUVCoordinates(float u, float v)
    {
        data_ = new Vector2(u, v);
        Normalize();
    }

    // implicit conversions: implement to_Cartesian and use it to make the other two functions!
    public static implicit operator NormalizedOctahedralCoordinates(OctahedralUVCoordinates UV)
    {
        Vector3[] pivots = new Vector3[3];
        Vector2[] anchors = new Vector2[3];

        // FIXME: use "data-driven" approach with Barycentric coordinates

        if (System.Math.Abs(UV.data_.x) + System.Math.Abs(UV.data_.y) < 1f) // inner diamond
        {
            if (System.Math.Sign(UV.data_.x) == +1)
            {
                if (System.Math.Sign(UV.data_.y) == +1) // top right
                {
                    pivots[0] = new Vector3(+1.0f, -1.0f, +0.0f);
                    pivots[1] = new Vector3(-1.0f, +0.0f, +1.0f);
                    pivots[2] = new Vector3(+0.0f, +1.0f, -1.0f);

                    anchors[0] = new Vector2(0.5f, 0.5f);
                    anchors[1] = new Vector2(1.0f, 0.5f);
                    anchors[2] = new Vector2(0.5f, 1.0f);
                }
                else // bottom right
                {
                    pivots[0] = new Vector3(+0.0f, -1.0f, -1.0f);
                    pivots[1] = new Vector3(+1.0f, +0.0f, +1.0f);
                    pivots[2] = new Vector3(-1.0f, +1.0f, +0.0f);
                    
                    anchors[0] = new Vector2(0.5f, 0.5f);
                    anchors[1] = new Vector2(0.5f, 0.0f);
                    anchors[2] = new Vector2(1.0f, 0.5f);
                }
            }
            else
            {
                if (System.Math.Sign(UV.data_.y) == +1) // top left
                {
                    pivots[0] = new Vector3(+0.0f, -1.0f, +1.0f);
                    pivots[1] = new Vector3(-1.0f, +0.0f, -1.0f);
                    pivots[2] = new Vector3(+1.0f, +1.0f, +0.0f);
                    
                    anchors[0] = new Vector2(0.5f, 0.5f);
                    anchors[1] = new Vector2(0.5f, 1.0f);
                    anchors[2] = new Vector2(0.0f, 0.5f);
                }
                else // bottom left
                {
                    pivots[0] = new Vector3(-1.0f, -1.0f, +0.0f);
                    pivots[1] = new Vector3(+1.0f, +0.0f, -1.0f);
                    pivots[2] = new Vector3(+0.0f, +1.0f, +1.0f);
                    
                    anchors[0] = new Vector2(0.5f, 0.5f);
                    anchors[1] = new Vector2(0.0f, 0.5f);
                    anchors[2] = new Vector2(0.5f, 0.0f);
                }
            }
        }
        else // outer corners
        {
            if (System.Math.Sign(UV.data_.x) == +1)
            {
                if (System.Math.Sign(UV.data_.y) == +1) // top right
                {
                    pivots[0] = new Vector3(+0.0f, +1.0f, +1.0f);
                    pivots[1] = new Vector3(+1.0f, +0.0f, -1.0f);
                    pivots[2] = new Vector3(-1.0f, -1.0f, +0.0f);
                    
                    anchors[0] = new Vector2(1.0f, 1.0f);
                    anchors[1] = new Vector2(0.5f, 1.0f);
                    anchors[2] = new Vector2(1.0f, 0.5f);
                }
                else // bottom right
                {
                    pivots[0] = new Vector3(+1.0f, +1.0f, +0.0f);
                    pivots[1] = new Vector3(-1.0f, +0.0f, -1.0f);
                    pivots[2] = new Vector3(+0.0f, -1.0f, +1.0f);
                    
                    anchors[0] = new Vector2(1.0f, 0.0f);
                    anchors[1] = new Vector2(1.0f, 0.5f);
                    anchors[2] = new Vector2(0.5f, 0.0f);
                }
            }
            else // top left
            {
                if (System.Math.Sign(UV.data_.y) == +1)
                {
                    pivots[0] = new Vector3(-1.0f, +1.0f, +0.0f);
                    pivots[1] = new Vector3(+1.0f, +0.0f, +1.0f);
                    pivots[2] = new Vector3(+0.0f, -1.0f, -1.0f);
                    
                    anchors[0] = new Vector2(0.0f, 1.0f);
                    anchors[1] = new Vector2(0.0f, 0.5f);
                    anchors[2] = new Vector2(0.5f, 1.0f);
                }
                else // bottom left
                {
                    pivots[0] = new Vector3(+0.0f, +1.0f, -1.0f);
                    pivots[1] = new Vector3(-1.0f, +0.0f, +1.0f);
                    pivots[2] = new Vector3(+1.0f, -1.0f, +0.0f);
                    
                    anchors[0] = new Vector2(0.0f, 0.0f);
                    anchors[1] = new Vector2(0.5f, 0.0f);
                    anchors[2] = new Vector2(0.0f, 0.5f);
                }
            }
        }
        Vector3 octahedral_vector = Vector3.zero;
        for (int dimension = 0; dimension < 3; ++dimension)
        {
            Vector2 begin = anchors[dimension];
            Vector2 end = anchors[(dimension + 1) % 3];

            float dot_product = Vector2.Dot((UV.data_ - begin).normalized, (begin - end).normalized); 
            octahedral_vector = octahedral_vector + pivots[dimension]*dot_product;
        }
        return new NormalizedOctahedralCoordinates(octahedral_vector);
    }

    Vector2 data_;

    /// <summary>
    /// Mutator - Wrap UV coordinates so that neither value is outside of [0,1].
    /// </summary>
    void Normalize()
    {
        if (Mathf.Abs(data_.x - 0.5f) > 0.5f && data_.x != 1f)
        {
            data_.x = PlanetariaMath.EuclideanDivisionModulo(data_.x, 1f); //TODO: test that 1) Vector2 is properly assigned
        }
        if (Mathf.Abs(data_.y - 0.5f) > 0.5f && data_.y != 1f)
        {
            data_.y = PlanetariaMath.EuclideanDivisionModulo(data_.y, 1f);
        }
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