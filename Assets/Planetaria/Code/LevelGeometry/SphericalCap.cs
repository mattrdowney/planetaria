using UnityEngine;

/// <summary>
/// A SphericalCap is a data structure that represents a (unique) SphericalCap volume formed by the intersection of a sphere and a plane.
/// SphericalCaps can go past the equator, but they always include one pole (and--only for spheres (i.e. offset -1)--sometimes contain both poles).
/// </summary>
public struct SphericalCap
{
    public SphericalCap complement()
    {
        return cap(-pole, -distance);
    }

    public Vector3 normal
    {
        get
        {
            return pole;
        }
    }

    public Vector3 base_center
    {
        get
        {
            return pole*distance;
        }
    }

    public float offset
    {
        get
        {
            return distance;
        }
    }

    /// <summary>
    /// Constructor - Creates a SphericalCap (i.e. a volume formed by cutting a sphere by a planar intersection) [may go past equator]
    /// </summary>
    /// <param name="pole">The pole included by the cutting plane</param>
    /// <param name="plane_distance">The distance along the pole/normal at which the plane intersects.</param>
    /// <returns>a SphericalCap (i.e. a volume formed by cutting a sphere by a planar intersection) [may go past equator]</returns>
    public static SphericalCap cap(Vector3 pole, float plane_distance)
    {
        pole.Normalize();
        return new SphericalCap(pole, plane_distance);
    }

    /// <summary>
    /// Constructor - Creates a SphericalCap (i.e. a volume formed by cutting a sphere by a planar intersection) [may go past equator]
    /// </summary>
    /// <param name="pole">The pole included by the cutting plane</param>
    /// <param name="plane_point">Any point at which the plane intersects.</param>
    /// <returns>a SphericalCap (i.e. a volume formed by cutting a sphere by a planar intersection) [may go past equator]</returns>
    public static SphericalCap cap(Vector3 pole, Vector3 plane_point)
    {
        pole.Normalize();
        return cap(pole, Mathf.Clamp(Vector3.Dot(pole, plane_point), -1, +1));
    }

    /// <summary>
    /// Constructor - Creates a SphericalCap (i.e. a volume formed by cutting a sphere by a planar intersection) [may go past equator]
    /// </summary>
    /// <param name="pole_normal">The pole included by the cutting plane as well as the normal of the cutting plane.</param>
    /// <param name="plane_distance">The distance along the pole/normal at which the plane intersects.</param>
    private SphericalCap(Vector3 pole_normal, float plane_distance)
    {
        pole = pole_normal;
        distance = plane_distance;
    }

    /// <summary>The normal of the cutting plane and the pole included in the sphere-plane intersection volume</summary>
    private Vector3 pole;

    /// <summary>The distance along the normal to arrive at the cutting plane.</summary>
    private float distance;
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