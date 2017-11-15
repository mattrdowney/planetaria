using UnityEngine;

// TODO: Multiton Design Pattern for multiple levels
public static class PlanetariaCache
{
    public static void cache(Block block)
    {
        if (Application.isPlaying)
        {
            foreach (optional<Arc> arc in block.iterator())
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
                    PlanetariaCache.block_cache.cache(collider, block);
                }
            }
        }
    }

    public static void uncache(Field field)
    {
        if (Application.isPlaying)
        {
            foreach (BoxCollider collider in field.gameObject.GetComponentsInChildren<BoxCollider>())
            {
                PlanetariaCache.field_cache.uncache(collider);
            }
        }
    }

    public static void uncache(Block block)
    {
        if (Application.isPlaying)
        {
            foreach (BoxCollider collider in block.gameObject.GetComponentsInChildren<BoxCollider>())
            {
                PlanetariaCache.arc_cache.uncache(collider);
            }
            foreach (BoxCollider collider in block.gameObject.GetComponentsInChildren<BoxCollider>())
            {
                PlanetariaCache.block_cache.uncache(collider);
            }
        }
    }
    
    [System.NonSerialized] public static PlanetariaSubcache<BoxCollider, Arc> arc_cache = new PlanetariaSubcache<BoxCollider, Arc>();
    [System.NonSerialized] public static PlanetariaSubcache<BoxCollider, Block> block_cache = new PlanetariaSubcache<BoxCollider, Block>();
    [System.NonSerialized] public static PlanetariaSubcache<BoxCollider, Field> field_cache = new PlanetariaSubcache<BoxCollider, Field>();
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