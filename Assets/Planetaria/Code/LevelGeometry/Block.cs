using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class Block : MonoBehaviour // Consider: class Shape : List<Arc> : IEnumerable<Arc>
{
    /// <summary>
    /// Constructor (default) - Creates an empty block
    /// </summary>
    /// <returns>An empty block with zero arcs and zero corners</returns>
    public static GameObject block()
    {
        GameObject result = new GameObject("Shape");
        Block block = result.AddComponent<Block>();

        return result;
    }

    public void add(GeospatialCurve curve)
    {
        if (!Application.isPlaying)
        {
            curve_list.Add(curve);
            generate_arcs();
        }
    }

    /// <summary>
    /// Returns the index of any existing arc within the block that matches the external reference. Null arcs are never found.
    /// </summary>
    /// <param name="arc">The reference to the external arc that will be compared to the block's arc list.</param>
    /// <returns>The index of the match if the arc exists in the container and is not null; a nonexistent index otherwise.</returns>
    public optional<ArcVisitor> arc_visitor(Arc arc)
    {
        int arc_list_index = arc_list.IndexOf(arc);

        if (arc_list_index == -1)
        {
            return new optional<ArcVisitor>();
        }

        return ArcVisitor.arc_visitor(arc_list, arc_list_index);
    }

    public IEnumerable<optional<Arc>> iterator()
    {
        return arc_list;
    }

    public bool active { get; set; }

    public bool empty()
    {
        return curve_list.Count == 0;
    }

    private void generate_arcs()
    {
        arc_list = new List<optional<Arc>>();
        for (int edge = 0; edge < curve_list.Count; ++edge)
        {
            Arc left_arc = Arc.arc(curve_list[edge]);
            Arc right_arc = Arc.arc(curve_list[(edge+1)%curve_list.Count]);
            arc_list.Add(left_arc);
            arc_list.Add(Arc.corner(left_arc, right_arc));
        }
    }

    private void Awake()
    {
        active = true;
        generate_arcs();
        if (Application.isPlaying)
        {
            effects = this.GetComponents<BlockActor>();
            transform = new PlanetariaTransform(this.GetComponent<Transform>());
            PlanetariaCache.cache(this);
        }
    }

    private void Reset()
    {
        generate_arcs();
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            PlanetariaCache.uncache(this);
        }
    }

    [SerializeField] private List<GeospatialCurve> curve_list = new List<GeospatialCurve>();
    [System.NonSerialized] private BlockActor[] effects; // previously optional<BlockActor>
    [System.NonSerialized] private new PlanetariaTransform transform; // TODO: make arcs relative (for moving platforms)
    [System.NonSerialized] private List<optional<Arc>> arc_list;
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