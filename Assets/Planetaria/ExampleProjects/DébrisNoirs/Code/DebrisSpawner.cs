using System;
using System.Collections;
using UnityEngine;
using Planetaria;

[Serializable]
public class DebrisSpawner : PlanetariaMonoBehaviour
{
    protected override void OnConstruction() { }
    protected override void OnDestruction() { }

    void Start ()
    {
		ship = PlanetariaGameObject.Find("Satellite").transform;
        large_debris_spawner = spawn_large_debris();
        StartCoroutine(large_debris_spawner);
        medium_debris_spawner = spawn_medium_debris();
        StartCoroutine(medium_debris_spawner);
	}

    public IEnumerator spawn_large_debris()
    {
        while (true)
        {
            yield return delay;
            if (!spawn_debris(false)) // since large debris is permanent, you can stop at 50.
            {
                StopCoroutine(large_debris_spawner);
            }
        }
    }

    public IEnumerator spawn_medium_debris()
    {
        yield return new WaitForSeconds(2*25+1-51); // spawn 25 large asteroids before spawning first medium asteroid, then alternate between 26 medium and remaining 25 large. 
        while (true)
        {
            spawn_debris(true);
            yield return delay;
        }
    }

    private bool spawn_debris(bool ephemeral)
    {
        if (DebrisNoirs.request_life(ephemeral))
        {
            Vector3 random_position = UnityEngine.Random.onUnitSphere;
            if (Vector3.Dot(random_position, ship.position) > 0)
            {
                random_position *= -1;
            }
            PlanetariaGameObject debris = PlanetariaGameObject.Instantiate(ephemeral ? medium_debris : large_debris, random_position);
            DebrisNoirs.live(debris, ephemeral);
            return true;
        }
        return false;
    }

    private static WaitForSeconds delay = new WaitForSeconds(2f);

    IEnumerator large_debris_spawner;
    IEnumerator medium_debris_spawner; // while it may not appear you need this thread, you do for "restarting it".

    [SerializeField] public GameObject large_debris;
    [SerializeField] public GameObject medium_debris;
    [NonSerialized] private PlanetariaTransform ship;
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