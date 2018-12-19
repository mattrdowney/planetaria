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
        StartCoroutine(spawn_debris_repeating()); // FIXME: string for function name means Visual Studio can't detect usage
	}

    public IEnumerator spawn_debris_repeating()
    {
        while (true)
        {
            yield return delay;
            spawn_debris();
        }
    }

    private void spawn_debris()
    {
        if (DebrisNoirs.request_life())
        {
            Vector3 random_position = UnityEngine.Random.onUnitSphere;
            if (Vector3.Dot(random_position, ship.position) > 0)
            {
                random_position *= -1;
            }
            PlanetariaGameObject debris = PlanetariaGameObject.Instantiate(prefabricated_debris, random_position);
            DebrisNoirs.live(debris);
        }
    }

    private static WaitForSeconds delay = new WaitForSeconds(1f);

    [SerializeField] public GameObject prefabricated_debris;
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