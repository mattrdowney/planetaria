using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    // TODO: Multiton Design Pattern for multiple levels?
    [DisallowMultipleComponent]
    public class PlanetariaCache : MonoBehaviour  // TODO: PlanetariaComponent
    {
        public static PlanetariaCache self
        {
            get
            {
                if (self_variable.exists)
                {
                    return self_variable.data;
                }
                GameObject game_object = Miscellaneous.GetOrAddObject("GameMaster");
                self_variable = Miscellaneous.GetOrAddComponent<PlanetariaCache>(game_object);
                return self_variable.data;
            }
        }

        public optional<Arc> arc_fetch(SphereCollider key)
        {
            optional<Block> block = PlanetariaCache.self.block_fetch(key);
            if (!block.exists)
            {
                return new optional<Arc>();
            }
            optional<int> arc_index = PlanetariaCache.self.index(key);
            if (!arc_index.exists)
            {
                return new optional<Arc>();
            }
            Arc arc = block.data.shape[arc_index.data];
            return arc;
        }

        public optional<PlanetariaCollider> collider_fetch(SphereCollider key)
        {
            if (!collider_cache.ContainsKey(key))
            {
                return new optional<PlanetariaCollider>();
            }
            return collider_cache[key];
        }

        public void cache(PlanetariaCollider collider)
        {
            PlanetariaCache.self.collider_cache.Add(collider.get_sphere_collider(), collider);
        }

        public void uncache(PlanetariaCollider collider)
        {
            PlanetariaCache.self.collider_cache.Remove(collider.get_sphere_collider());
        }
        
        [NonSerialized] private Dictionary<SphereCollider, PlanetariaCollider> collider_cache = new Dictionary<SphereCollider, PlanetariaCollider>();

        private static optional<PlanetariaCache> self_variable = new optional<PlanetariaCache>();
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