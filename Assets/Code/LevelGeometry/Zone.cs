using System.Collections.Generic;
using UnityEngine;

public class Zone : Component
{
    List<Plane> plane_list;
    
    /// <summary>
    /// Constructor - Generates a zone using a .ssvg file.
    /// </summary>
    /// <param name="ssvg_file">
    /// The .ssvg (spherical scalable vector graphics) file that will generate the zone.
    /// Special note: the .ssvg MUST be convex or all behavior is undefined.
    /// </param>
    /// <returns>The GameObject reference with an attached Zone component.</returns>
    public static GameObject CreateZone(string ssvg_file) // TODO: add convex check asserts.
    {
        GameObject result = new GameObject();
        Zone zone = result.AddComponent<Zone>();

        zone.plane_list = new List<Plane>();

        return result;
    }

    /// <summary>
    /// Checks if the center of mass (position) is below all of the planes. 
    /// Special note: if a portion of the extruded volume is inside all planes, this function might still return false.
    /// </summary>
    /// <param name="position">A position on a unit-sphere.</param>
    /// <param name="radius">The radius [0,PI/2] to extrude.</param>
    /// <returns>True if position is below (inside) all of the planes; false otherwise.</returns>
    public bool contains(Vector3 position, float radius = 0f)
    {
        foreach (Plane plane in plane_list)
        {
            if (plane.GetSide(position))
            {
                return false;
            }
        }

        return true;
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