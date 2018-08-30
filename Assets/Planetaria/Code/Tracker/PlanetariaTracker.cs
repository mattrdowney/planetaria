using UnityEngine;

namespace Planetaria
{
    public abstract class PlanetariaTracker : PlanetariaComponent
    {
        public abstract void setup();
        public abstract void cleanup();
        public abstract void step();
        public abstract void teleport();

        protected PlanetariaTransform self;
        protected optional<PlanetariaTransform> target;
        protected optional<PlanetariaCameraShutter> shutter
        {
            get
            {
                return internal_shutter;
            }
            set
            {
                if (internal_shutter.exists)
                {
                    internal_shutter.data.blink_event -= teleport;
                }
                internal_shutter = value;
                if (internal_shutter.exists)
                {
                    internal_shutter.data.blink_event += teleport;
                }
            }
        }

        private void Start()
        {
            self = this.GetComponent<PlanetariaTransform>();
            setup();
        }

        private void Update()
        {
            step();
        }

        protected override void OnDestroy()
        {
            cleanup();
            shutter = new optional<PlanetariaCameraShutter>(); // deregisters from blink_event
        }

        private optional<PlanetariaCameraShutter> internal_shutter;
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