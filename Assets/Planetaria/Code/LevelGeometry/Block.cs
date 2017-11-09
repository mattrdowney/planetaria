using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class Block : MonoBehaviour, ISerializationCallbackReceiver // Consider: class Shape : List<Arc> : IEnumerable<Arc>
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
        curve_list.Add(curve);
        generate_arcs();
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
        foreach (GeospatialCurve curve in curve_list)
        {
            arc_list.Add(Arc.arc(curve));
        }
        recache();
    }

    private void recache()
    {
        PlanetariaCache.arc_cache.clear();
        PlanetariaCache.block_cache.clear();
        PlanetariaCache.field_cache.clear();
        Block[] blocks = GameObject.FindObjectsOfType<Block>();
        foreach (Block block in blocks)
        {
            block.gameObject.transform.clear_children();
            foreach (optional<Arc> arc in arc_list)
            {
                if (arc.exists)
                {
                    GameObject game_object = new GameObject("Collider");
                    game_object.transform.parent = block.gameObject.transform;

                    BoxCollider collider = game_object.AddComponent<BoxCollider>();
                    Bounds axis_aligned_bounding_box = Arc.get_axis_aligned_bounding_box(arc.data);
                    collider.center = axis_aligned_bounding_box.center;
                    collider.size = axis_aligned_bounding_box.size;
                    collider.isTrigger = true;

                    PlanetariaCache.arc_cache.cache(collider, arc.data);
                    PlanetariaCache.block_cache.cache(arc.data, block);
                }
            }
        }
    }

    private void Awake()
    {
        effects = this.GetComponents<BlockActor>();
        transform = new PlanetariaTransform(this.GetComponent<Transform>());
        active = true;
        arc_list = new List<optional<Arc>>();
        curve_list = new List<GeospatialCurve>();
        generate_arcs();
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
        this.gameObject.transform.clear_children(); // Prevent serialization of children (which would change on each save)
    }

    private void OnDestroy()
    {
        curve_list.Clear();
        arc_list.Clear();
        recache();
    }

    [SerializeField] private BlockActor[] effects; // previously optional<BlockActor>
    [SerializeField] private List<GeospatialCurve> curve_list;
    [SerializeField] private new PlanetariaTransform transform; // TODO: make arcs relative (for moving platforms)
    [System.NonSerialized] private List<optional<Arc>> arc_list; // FIXME: System.Collection.Immutable.ImmutableArray<Arc> not supported in current Unity version?
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