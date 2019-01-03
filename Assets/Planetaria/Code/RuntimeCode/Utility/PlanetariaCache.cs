using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    // CONSIDER: Multiton Design Pattern for multiple levels?
    public static class PlanetariaCache
    {
        public static PlanetariaCollider collider_fetch(SphereCollider key)
        {
            if (!collider_cache.ContainsKey(key))
            {
                PlanetariaCollider planetaria_collider = key.GetComponent<PlanetariaCollider>();
                Debug.Assert(planetaria_collider, "SphereColliders must be matched with PlanetariaColliders");
                collider_cache.Add(key, planetaria_collider);
            }
            return collider_cache[key];
        }

        public static void cache(PlanetariaCollider collider)
        {
            PlanetariaCache.collider_cache.Add(collider.get_sphere_collider(), collider);
        }

        public static void uncache(PlanetariaCollider collider)
        {
            PlanetariaCache.collider_cache.Remove(collider.get_sphere_collider());
        }
        
        private static Dictionary<SphereCollider, PlanetariaCollider> collider_cache = new Dictionary<SphereCollider, PlanetariaCollider>();
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.