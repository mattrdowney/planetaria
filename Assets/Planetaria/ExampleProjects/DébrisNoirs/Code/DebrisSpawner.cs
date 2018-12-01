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
		ship = PlanetariaGameObject.Find("Ship").transform;
        StartCoroutine(detect_debris()); // FIXME: string for function name means Visual Studio can't detect usage
	}

    public IEnumerator detect_debris()
    {
        int seconds = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            seconds += 1;

            // Actually detect any remaining debris
            GameObject[] game_objects = GameObject.FindObjectsOfType<GameObject>();
            int debris_layer = LayerMask.NameToLayer("Debris");
            foreach (GameObject game_object in game_objects)
            {
                if (game_object.layer == debris_layer)
                {
                    seconds = 0; // if there is still debris, do not continue to the next round yet
                    break;
                }
            }
            if (seconds >= 1)
            {
                spawn_debris();
                seconds = 0;
            }
        }
    }

    private void spawn_debris()
    {
        for (int debris = 0; debris < 4; ++debris)
        {
            Vector3 random_position = UnityEngine.Random.onUnitSphere;
            if (Vector3.Dot(random_position, ship.position.data) > 0)
            {
                random_position *= -1;
            }
            PlanetariaGameObject.Instantiate(prefabricated_debris, random_position);
        }
    }

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