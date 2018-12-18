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
            game_objects.Remove(game_object);
        }

        public static bool request_life()
        {
            return game_objects.Count < maximum_game_objects;
        }

        public static void live(PlanetariaGameObject game_object)
        {
            game_objects.Add(game_object);
        }

        private const int maximum_game_objects = 100;
        private static HashSet<PlanetariaGameObject> game_objects = new HashSet<PlanetariaGameObject>();
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