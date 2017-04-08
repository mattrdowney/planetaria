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
    
    /// <summary>
    /// Inspector - Converts octahedron UV coordinates into octahedral coordinates.
    /// </summary>
    /// <param name="UV">The coordinates in octahedral UV space that will be converted</param>
    /// <returns>The octahedral coordinates.</returns> 
    public static implicit operator NormalizedOctahedralCoordinates(OctahedralUVCoordinates UV)
    {
        return new NormalizedOctahedralCoordinates(Octahedron.UV_to_Cartesian(UV.data));
    }

    /// <summary>
    /// Inspector - Converts octahedron UV coordinates into Cartesian coordinates.
    /// </summary>
    /// <param name="UV">The coordinates in octahedral UV space that will be converted</param>
    /// <returns>The Cartesian coordinates.</returns> 
    public static implicit operator NormalizedCartesianCoordinates(OctahedralUVCoordinates UV)
    {
        NormalizedOctahedralCoordinates octahedral = UV;
        return octahedral;
    }

    /// <summary>
    /// Inspector - Converts octahedron UV coordinates into spherical coordinates.
    /// </summary>
    /// <param name="UV">The coordinates in octahedral UV space that will be converted</param>
    /// <returns>The spherical coordinates.</returns> 
    public static implicit operator NormalizedSphericalCoordinates(OctahedralUVCoordinates UV)
    {
        NormalizedCartesianCoordinates Cartesian = UV;
        return Cartesian;
    }

    Vector2 data_;

    /// <summary>
    /// Mutator - Wrap UV coordinates so that neither value is outside of [0,1].
    /// </summary>
    void Normalize()
    {
        if (Mathf.Abs(data_.x - 0.5f) > 0.5f && data_.x != 1f)
        {
            data_.x = PlanetariaMath.modulo_using_Euclidean_division(data_.x, 1f); //TODO: test that 1) Vector2 is properly assigned
        }
        if (Mathf.Abs(data_.y - 0.5f) > 0.5f && data_.y != 1f)
        {
            data_.y = PlanetariaMath.modulo_using_Euclidean_division(data_.y, 1f);
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