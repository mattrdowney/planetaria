using System.Collections.Generic;
using UnityEngine;

public class Block : Component
{
    List<Arc> arc_list;

    /// <summary>
    /// Constructor - Generates a block using a .ssvg file.
    /// </summary>
    /// <param name="ssvg_file">The .ssvg (spherical scalable vector graphics) file that will generate the block.</param>
    /// <returns>The GameObject reference with an attached Block component.</returns>
    public static GameObject CreateBlock(string ssvg_file)
    {
        GameObject result = new GameObject();
        Block block = result.AddComponent<Block>();

        block.arc_list = new List<Arc>();

        return result;
    }

    /// <summary>
    /// Checks if any of the arcs contain the position extruded by radius.
    /// This does NOT check if the point is inside the convex hull, so points below the floor will not be matched.
    /// </summary>
    /// <param name="position">A position on a unit-sphere.</param>
    /// <param name="radius">The radius [0,PI/2] to extrude.</param>
    /// <returns>True if any of the arcs contain the point extruded by radius.</returns>
    public bool contains(Vector3 position, float radius = 0f)
    {
        foreach (Arc arc in arc_list)
        {
            if (arc)
            {
                if (arc.contains(position, radius))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the index of any existing arc within the block that matches the external reference. Null arcs are never found.
    /// </summary>
    /// <param name="arc">The reference to the external arc that will be compared to the block's arc list.</param>
    /// <returns>The index of the match if the arc exists in the container and is not null; a nonexistent index otherwise.</returns>
    public optional<int> arc_index(Arc arc)
    {
        if (!arc)
        {
            return new optional<int>();
        }

        for (int index = 0; index < arc_list.Count; ++index)
        {
            if (arc == arc_list[index])
            {
                return index;
            }
        }
        return new optional<int>();
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