using System.Collections.Generic;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    /// <summary>
    /// Singleton - Game Manager for Debris Noirs.
    /// </summary>
    /// <seealso cref="https://www.arcade-history.com/?n=asteroids-upright-model&page=detail&id=126"/> // Extra information about Asteroids (1979)
    public class DebrisNoirs
    {
        public static void die(Debris debris)
        {
            debris.has_collided = false;
            DebrisNoirs.debris.Remove(debris);
            debris.gameObject.SetActive(false);
        }

        public static bool empty()
        {
            return debris.Count == 0;
        }

        public static void expand(int capacity)
        {
            int large_rocks_to_spawn = Mathf.Max(0, capacity - object_pool.Count);
            for (int spawned_root_node = 0; spawned_root_node < large_rocks_to_spawn; spawned_root_node += 1)
            {
                // Yeah, this is hardcoded boilerplate, but it gets the job done (rather than coding a recursive function for levels 1,2,3)

                // Large

                PlanetariaGameObject game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().large_debris, Vector3.forward, Vector3.up);
                Debris large = game_object.GetComponent<Debris>();
                object_pool.Add(large); // add the root node only

                // Medium

                game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().medium_debris, Vector3.forward, Vector3.up);
                Debris medium1 = game_object.GetComponent<Debris>();
                large.left_debris = medium1;

                game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().medium_debris, Vector3.forward, Vector3.up);
                Debris medium2 = game_object.GetComponent<Debris>();
                large.right_debris = medium2;

                // Small

                game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().small_debris, Vector3.forward, Vector3.up);
                Debris small1 = game_object.GetComponent<Debris>();
                medium1.left_debris = small1;

                game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().small_debris, Vector3.forward, Vector3.up);
                Debris small2 = game_object.GetComponent<Debris>();
                medium1.right_debris = small2;

                game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().small_debris, Vector3.forward, Vector3.up);
                Debris small3 = game_object.GetComponent<Debris>();
                medium2.left_debris = small3;

                game_object = PlanetariaGameObject.Instantiate(DebrisSpawner.self().small_debris, Vector3.forward, Vector3.up);
                Debris small4 = game_object.GetComponent<Debris>();
                medium2.right_debris = small4;

                // Disable game objects until they are needed

                large.gameObject.SetActive(false);
                medium1.gameObject.SetActive(false);
                medium2.gameObject.SetActive(false);
                small1.gameObject.SetActive(false);
                small2.gameObject.SetActive(false);
                small3.gameObject.SetActive(false);
                small4.gameObject.SetActive(false);
            }
        }

        public static void heat_death()
        {
            foreach (Debris debris in object_pool)
            {
                PlanetariaGameObject.Destroy(debris.left_debris.left_debris.gameObject);
                PlanetariaGameObject.Destroy(debris.left_debris.right_debris.gameObject);
                PlanetariaGameObject.Destroy(debris.left_debris.gameObject);
                PlanetariaGameObject.Destroy(debris.right_debris.left_debris.gameObject);
                PlanetariaGameObject.Destroy(debris.right_debris.right_debris.gameObject);
                PlanetariaGameObject.Destroy(debris.right_debris.gameObject);
                PlanetariaGameObject.Destroy(debris.gameObject);
            }
            object_pool.Clear(); // The object pool stores the root objects (and medium/small debris implicitly)
            debris.Clear(); // Debris stores active debris on-screen
        }

        public static Debris live()
        {
            Debris debris = object_pool[next_debris];
            next_debris = next_debris >= object_pool.Count ? 0 : next_debris + 1;
            live(debris);
            return debris;
        }

        public static void live(Debris debris)
        {
            DebrisNoirs.debris.Add(debris);
            debris.gameObject.SetActive(true);
        }

        public static DebrisNoirs internal_instance;

        private static int next_debris = 0;
        private static DebrisSpawner debris_spawner;
        private static HashSet<Debris> debris = new HashSet<Debris>();
        private static List<Debris> object_pool = new List<Debris>();
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