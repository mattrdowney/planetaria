using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    public class TemperatureCooling : PlanetariaMonoBehaviour
    {
        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        private void OnValidate()
        {
            planetaria_collider = this.GetComponent<PlanetariaCollider>();
            planetaria_renderer_foreground = this.GetComponent<AreaRenderer>();
        }

        private void Update()
        {
            planetaria_renderer_foreground.color = Color.Lerp(Color.grey, Color.white, cooldown_timer / 3); // FIXME: MAGIC NUMBERS:
            if (cooldown_timer < 0)
            {
                planetaria_collider.gameObject.layer = LayerMask.NameToLayer("EphemeralDebris");
                planetaria_renderer_foreground.color = Color.grey;
            }
            cooldown_timer -= Time.deltaTime;
        }

        [SerializeField] private float cooldown_timer = 3f;

        [SerializeField] [HideInInspector] private AreaRenderer planetaria_renderer_foreground;
        [SerializeField] [HideInInspector] private PlanetariaCollider planetaria_collider;
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