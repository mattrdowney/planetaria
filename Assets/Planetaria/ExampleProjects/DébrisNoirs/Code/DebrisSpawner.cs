using System;
using System.Collections;
using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    [Serializable]
    public class DebrisSpawner : PlanetariaMonoBehaviour
    {
        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        void Start()
        {
            satellite = PlanetariaGameObject.Find("Satellite").transform;
            spawn();
        }

        public IEnumerator spawn_debris_subroutine()
        {
            while (true)
            {
                yield return second_delay;
                if (DebrisNoirs.empty())
                {
                    yield return five_second_delay; // give the player time to refresh
                    for (int countdown = 5; countdown >= 1; countdown -= 1) // print countdown on user interface
                    {
                        text.text = countdown.ToString(); // display countdown
                        yield return second_delay;
                    }
                    text.text = ""; // clear the screen
                    round += 1; // enter next round
                    spawn_debris();
                }
            }
        }

        public void stop()
        {
            StopAllCoroutines();
        }

        public void spawn()
        {
            round = 0;
            StartCoroutine(spawn_debris_subroutine());
        }

        private void spawn_debris()
        {
            const int maximum_large_debris = 100; // most debris possible is 400 small debris (unlikely)
            int debris_to_spawn = Mathf.Min(round, maximum_large_debris);
            // Spawn 1 piece of debris on first round and n pieces on nth round.
            for (int debris = 0; debris < debris_to_spawn; debris += 1)
            {
                Vector3 random_position = UnityEngine.Random.onUnitSphere;
                if (Vector3.Dot(random_position, satellite.position) > 0)
                {
                    random_position *= -1;
                }
                DebrisNoirs.live(PlanetariaGameObject.Instantiate(large_debris, random_position));
            }
        }

        private static WaitForSeconds second_delay = new WaitForSeconds(1f);
        private static WaitForSeconds five_second_delay = new WaitForSeconds(5f);

        public int round = 0;
        [SerializeField] private GameObject large_debris;
        [SerializeField] private UnityEngine.UI.Text text; 
        [NonSerialized] private PlanetariaTransform satellite;
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