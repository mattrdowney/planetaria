using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public class Field : PlanetariaComponent
    {   
        public static GameObject field(List<GeospatialCurve> curves) // TODO: add convex check asserts.
        {
            GameObject result = new GameObject("Field");
            Field field = result.AddComponent<Field>(); // FIXME: implement
            field.shape_variable = new Shape(curves, true, false); // CONSIDER: TODO: should corners be generated?

            return result;
        }

        protected override void Awake()
        {
            base.Awake();
            initialize();
        }

        private void Start()
        {
            initialize();
            PlanetariaCache.self.cache(this);
        }

        protected override void Reset()
        {
            base.Reset();
            initialize();
        }

        private void initialize()
        {
            if (transform == null)
            {
                transform = Miscellaneous.GetOrAddComponent<PlanetariaTransform>(this);
            }
            if (internal_transform == null)
            {
                internal_transform = gameObject.internal_game_object.GetComponent<Transform>();
            }
        }

        protected override void OnDestroy()
        {
            PlanetariaCache.self.uncache(this);
        }

        public Shape shape
        {
            get
            {
                return shape_variable;
            }
        }

        public bool active { get; set; }
        
        public bool is_dynamic;
        [NonSerialized] public new PlanetariaTransform transform;
        [SerializeField] [HideInInspector] public Transform internal_transform;
        [SerializeField] private Shape shape_variable;
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