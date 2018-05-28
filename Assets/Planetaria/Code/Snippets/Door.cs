using UnityEngine;

namespace Planetaria
{
    public class Door : PlanetariaMonoBehaviour
    {
        public int target_level = 1;

        private void Awake()
        {
            OnFieldStay.data = on_field_stay;
        }

        protected override void OnConstruction()
        {
        }

        protected override void OnDestruction()
        {
        }

        private void on_field_stay(PlanetariaCollider collider)
        {
            if (Input.GetAxis("Vertical") == 1)
            {
                LevelLoader.loader.activate_level(target_level);
            }
        }
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/