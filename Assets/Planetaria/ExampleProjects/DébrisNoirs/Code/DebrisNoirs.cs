using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	/// Singleton - Game Manager for Debris Noirs.
    /// </summary>
    /// <seealso cref="https://www.arcade-history.com/?n=asteroids-upright-model&page=detail&id=126"/> // Extra information about Asteroids (1979)
	public static class DebrisNoirs // LAZY SINGLETON
	{
        public enum DebrisSize { Large = 0, Medium = 1, Small = 2 }

        public static void heat_death()
        {
            for (int type = 0; type <= (int) DebrisSize.Small; type += 1)
            {
                if (debris[type] != null)
                {
                    foreach (PlanetariaGameObject game_object in debris[type])
                    {
                        PlanetariaGameObject.Destroy(game_object);
                    }
                    debris[type].Clear();
                }
            }
        }

		public static void request_death(PlanetariaGameObject game_object, DebrisSize type)
        {
            if (debris[(int)type] == null)
            {
                debris[(int)type] = new HashSet<PlanetariaGameObject>();
            }
            debris[(int)type].Remove(game_object);
        }

        public static bool request_life(DebrisSize type)
        {
            if (debris[(int)type] == null)
            {
                debris[(int)type] = new HashSet<PlanetariaGameObject>();
            }
            return debris[(int)type].Count < maximum_debris_objects[(int)type];
        }

        public static void live(PlanetariaGameObject game_object, DebrisSize type)
        {
            if (debris[(int)type] == null)
            {
                debris[(int)type] = new HashSet<PlanetariaGameObject>();
            }
            debris[(int)type].Add(game_object);
        }

        private static readonly int[] maximum_debris_objects = new int[] { 50, 26, int.MaxValue };
        private static HashSet<PlanetariaGameObject>[] debris = new HashSet<PlanetariaGameObject>[3];
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