using System.Collections.Generic;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    /// <summary>
    /// Singleton - Game Manager for Debris Noirs.
    /// </summary>
    /// <seealso cref="https://www.arcade-history.com/?n=asteroids-upright-model&page=detail&id=126"/> // Extra information about Asteroids (1979)
    public static class DebrisNoirs
    {
        public static bool empty()
        {
            return debris.Count == 0;
        }

        public static void heat_death()
        {
            foreach (PlanetariaGameObject game_object in debris)
            {
                PlanetariaGameObject.Destroy(game_object);
            }
            debris.Clear();
        }

        public static void die(PlanetariaGameObject game_object)
        {
            debris.Remove(game_object);
        }

        public static void live(PlanetariaGameObject game_object)
        {
            debris.Add(game_object);
        }

        private static DebrisSpawner debris_spawner;
        private static HashSet<PlanetariaGameObject> debris = new HashSet<PlanetariaGameObject>();
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