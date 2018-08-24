using System;
using System.Linq;
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

        public optional<Block> block_fetch(SphereCollider key)
        {
            if (!block_cache.ContainsKey(key))
            {
                return new optional<Block>();
            }
            return block_cache[key];
        }

        public optional<PlanetariaCollider> collider_fetch(SphereCollider key)
        {
            if (!collider_cache.ContainsKey(key))
            {
                return new optional<PlanetariaCollider>();
            }
            return collider_cache[key];
        }

        public optional<int> index(SphereCollider key)
        {
            if (!index_cache.ContainsKey(key))
            {
                return new optional<int>();
            }
            return index_cache[key];
        }

        public void cache(Block block)
        {
            int arc_index = 0;
            Transform child;
            foreach (Arc arc in block.shape.arcs) // TODO: verify that concave corners work as expected
            {
                if (arc.type != GeometryType.ConcaveCorner)
                {
                    child = block.GetOrAddChild("Collider" + arc_index, false).transform; // TODO: Zero out transform.position for GitHub Issue #37
                    PlanetariaCollider collider = Miscellaneous.GetOrAddComponent<PlanetariaCollider>(child.gameObject);
                    SphereCollider sphere_collider = collider.get_sphere_collider();

                    Sphere[] colliders = Sphere.arc_collider(block.internal_transform, arc);
                    collider.set_colliders(colliders);
                    collider.is_field = false;
                    collider.material = block.material;
                    sphere_collider.isTrigger = true;

                    PlanetariaCache.self.index_cache.Add(sphere_collider, arc_index);
                    PlanetariaCache.self.block_cache.Add(sphere_collider, block);
                    PlanetariaCache.self.collider_cache.Add(sphere_collider, collider);
                    ++arc_index;
                }
            }
            while (child = block.internal_transform.Find("Collider" + arc_index)) // clear out extra (old) colliders with while loop
            {
                Destroy(child.gameObject);
            }
        }

        public void cache(Field field)
        {
            GameObject game_object = field.GetOrAddChild("Collider", false);
            PlanetariaCollider collider = Miscellaneous.GetOrAddComponent<PlanetariaCollider>(game_object);
            SphereCollider sphere_collider = collider.get_sphere_collider();

            collider.is_field = true;
            sphere_collider.isTrigger = true;

            Sphere[] colliders = new Sphere[Enumerable.Count(field.shape.arcs)];
            int current_index = 0;
            foreach (Arc arc in field.shape.arcs)
            {
                colliders[current_index] = Sphere.uniform_collider(field.internal_transform, arc.floor());
                ++current_index;
            }
            collider.set_colliders(colliders);

            PlanetariaCache.self.collider_cache.Add(sphere_collider, collider);
        }

        public void uncache(MonoBehaviour script)
        {
            foreach (PlanetariaCollider collider in script.GetComponentsInChildren<PlanetariaCollider>())
            {
                Destroy(collider.gameObject.internal_game_object);
            }
        }

        public void uncache(PlanetariaCollider collider)
        {
            PlanetariaCache.self.index_cache.Remove(collider.get_sphere_collider());
            PlanetariaCache.self.block_cache.Remove(collider.get_sphere_collider());
            PlanetariaCache.self.collider_cache.Remove(collider.get_sphere_collider());
        }

        public void cache_all()
        {
            // FIXME: only destroy/create GameObjects on arc_list change to prevent unneccessary level edits between user sessions.
            uncache_all();
            foreach (Block block in GameObject.FindObjectsOfType<Block>())
            {
                PlanetariaCache.self.cache(block);
            }
            foreach (Field field in GameObject.FindObjectsOfType<Field>())
            {
                PlanetariaCache.self.cache(field);
            }
        }

        public void uncache_all()
        {
            index_cache.Clear();
            block_cache.Clear();
            collider_cache.Clear();
        }

        [NonSerialized] private Dictionary<SphereCollider, int> index_cache = new Dictionary<SphereCollider, int>();
        [NonSerialized] private Dictionary<SphereCollider, Block> block_cache = new Dictionary<SphereCollider, Block>();
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