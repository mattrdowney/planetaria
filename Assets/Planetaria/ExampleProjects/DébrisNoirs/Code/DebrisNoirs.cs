using System.Collections.Generic;

namespace Planetaria
{
    /// <summary>
	/// Singleton - Game Manager for Debris Noirs.
    /// </summary>
    /// <seealso cref="https://www.arcade-history.com/?n=asteroids-upright-model&page=detail&id=126"/> // Extra information about Asteroids (1979)
	public static class DebrisNoirs
	{
		public static void request_death(PlanetariaGameObject game_object)
        {
            large_debris.Remove(game_object);
            medium_debris.Remove(game_object);
        }

        public static bool request_life(bool ephemeral)
        {
            if (ephemeral)
            {
                return medium_debris.Count < maximum_medium_debris;
            }
            return large_debris.Count < maximum_large_debris;
        }

        public static void live(PlanetariaGameObject game_object, bool ephemeral)
        {
            if (ephemeral)
            {
                medium_debris.Add(game_object);
            }
            else
            {
                large_debris.Add(game_object);
            }
        }

        private const int maximum_large_debris = 50;
        private const int maximum_medium_debris = 26;
        private static HashSet<PlanetariaGameObject> large_debris = new HashSet<PlanetariaGameObject>();
        private static HashSet<PlanetariaGameObject> medium_debris = new HashSet<PlanetariaGameObject>();
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