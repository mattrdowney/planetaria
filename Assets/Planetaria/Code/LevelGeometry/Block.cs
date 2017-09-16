using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block : MonoBehaviour
{
    [SerializeField] BlockActor behavior;
    
    [SerializeField] List<Arc> arc_list;
    //[SerializeField] PlanetariaTransform transform; // TODO: make arcs relative (for moving platforms)

    /// <summary>
    /// Constructor - Generates a block using a .ssvg file.
    /// </summary>
    /// <param name="ssvg_file">The .ssvg (spherical scalable vector graphics) file that will generate the block.</param>
    /// <returns>The GameObject reference with an attached Block component.</returns>
    public static GameObject CreateBlock(string ssvg_file)
    {
        GameObject result = CreateBlock();

        return result;
    }

    /// <summary>
    /// Constructor (default) - Creates an empty block
    /// </summary>
    /// <returns>An empty block with zero arcs and zero corners</returns>
    public static GameObject CreateBlock()
    {
        GameObject result = new GameObject();
        Block block = result.AddComponent<Block>();

        block.arc_list = new List<Arc>();

        return result;
    }

    public void Add(optional<Arc> arc)
    {
        arc_list.Add(arc.data);
    }

    /// <summary>
    /// Returns the index of any existing arc within the block that matches the external reference. Null arcs are never found.
    /// </summary>
    /// <param name="arc">The reference to the external arc that will be compared to the block's arc list.</param>
    /// <returns>The index of the match if the arc exists in the container and is not null; a nonexistent index otherwise.</returns>
    public optional<int> arc_index(Arc arc)
    {
        int arc_list_index = arc_list.IndexOf(arc);

        if (arc_list_index == -1)
        {
            return new optional<int>();
        }

        return arc_list_index;
    }

    /// <summary>
    /// Inspector - Get the reference to the current arc.
    /// </summary>
    /// <param name="index">The (ordered sequence) index of the Arc.</param>
    /// <returns>The Arc at the given index.</returns>
    public optional<Arc> at(ref int index)
    {
        if (index < 0 || index >= arc_list.Count)
        {
            index = (int) PlanetariaMath.modulo_using_Euclidean_division(index, arc_list.Count);
        }

        return arc_list[index];
    }

    /// <summary>
    /// Checks if any of the arcs contain the position extruded by radius.
    /// This does NOT check if the point is inside the concave/convex hull, so points below the floor will not be matched.
    /// </summary>
    /// <param name="position">A position on a unit-sphere.</param>
    /// <param name="radius">The radius [0,PI/2] to extrude.</param>
    /// <returns>True if any of the arcs contain the point extruded by radius.</returns>
    public bool contains(Vector3 position, float radius = 0f)
    {
        foreach (optional<Arc> arc in arc_list)
        {
            if (arc.exists)
            {
                if (arc.data.contains(position, radius))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int size()
    {
        return arc_list.Count;
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