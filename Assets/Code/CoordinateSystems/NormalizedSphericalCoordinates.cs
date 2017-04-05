using UnityEngine;

public class NormalizedSphericalCoordinates
{
	public Vector2 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    /// <summary>
    /// Constructor - Stores a set of spherical coordinates on a unit sphere (i.e. rho=1) in a wrapper class.
    /// </summary>
    /// <param name="elevation">The angle in radians between the negative y-axis and the vector in Cartesian space.</param>
    /// <param name="azimuth">The angle in radians between the positive x-axis and the vector in Cartian space measured counterclockwise around the y-axis (viewing angle is downward along y-axis).</param>
    public NormalizedSphericalCoordinates(float elevation, float azimuth)
    {
        data_ = new Vector2(elevation, azimuth);
        Normalize();
    }

    /// <summary>
    /// Inspector - Converts spherical coordinates into Cartesian coordinates.
    /// </summary>
    /// <param name="spherical">The spherical coordinates that will be converted</param>
    /// <returns>The Cartesian coordinates.</returns> 
    public static implicit operator NormalizedCartesianCoordinates(NormalizedSphericalCoordinates octahedral)
    {
        Vector3 Cartesian = new Vector3();
        Cartesian.x = -Mathf.Sin(octahedral.data.x) * Mathf.Cos(octahedral.data.y);
        Cartesian.y = -Mathf.Cos(octahedral.data.x);
        Cartesian.z = -Mathf.Sin(octahedral.data.x) * Mathf.Sin(octahedral.data.y);
        return new NormalizedCartesianCoordinates(Cartesian);
    }

    /// <summary>
    /// Inspector - Converts spherical coordinates into octahedral coordinates.
    /// </summary>
    /// <param name="spherical">The spherical coordinates that will be converted</param>
    /// <returns>The octahedral coordinates.</returns> 
    public static implicit operator NormalizedOctahedralCoordinates(NormalizedSphericalCoordinates spherical)
    {
        NormalizedCartesianCoordinates Cartesian = spherical;
        return Cartesian;
    }

    /// <summary>
    /// Inspector - Converts spherical coordinates into octahedron UV space.
    /// </summary>
    /// <param name="spherical">The spherical coordinates that will be converted</param>
    /// <returns>The UV coordinates for an octahedron.</returns> 
    public static implicit operator OctahedralUVCoordinates(NormalizedSphericalCoordinates spherical)
    {
        NormalizedCartesianCoordinates Cartesian = spherical;
        return Cartesian;
    }

    Vector2 data_;

    /// <summary>
    /// Mutator - Wrap elevation and azimuth so they are within [0, PI] and [0, 2*PI) respectively. 
    /// </summary>
    void Normalize()
    {
        if (Mathf.Abs(data_.x - Mathf.PI/2) > Mathf.PI/2)
        {
            data_.x = Mathf.PingPong(data_.x, 2*Mathf.PI); //TODO: test that 1) Vector2 is properly assigned 2) PingPong works for negative numbers
            if (data_.x > Mathf.PI)
            {
                data_.x -= Mathf.PI;
                data_.y += Mathf.PI; // going through a pole changes the azimuth
            }
        }
        if (Mathf.Abs(data_.y - Mathf.PI) > Mathf.PI || data_.y == 2*Mathf.PI)
        {
            data_.y = PlanetariaMath.EuclideanDivisionModulo(data_.y, 2*Mathf.PI);
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