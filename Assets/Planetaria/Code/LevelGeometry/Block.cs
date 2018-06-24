﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public class Block : MonoBehaviour // TODO: PlanetariaComponent
    {
        /// <summary>
        /// Constructor - Creates a block matching a curves list.
        /// </summary>
        /// <returns>A block matching its blueprint.</returns>
        public static GameObject block(List<GeospatialCurve> curves)
        {
            GameObject result = new GameObject("Block");
            Block block = result.AddComponent<Block>();
            block.shape_variable = new Shape(curves, true, true);
            block.ignore.Add(block);
            return result;
        }

        public bool active
        {
            get
            {
                return active_variable;
            }
            set
            {
                active_variable = value;
            }
        }

        public Vector3 center_of_mass() // FIXME: proper volume integration (for convex hulls)
        {
            Vector3 result = Vector3.zero;
            foreach (optional<Arc> arc in shape_variable.arcs)
            {
                if (arc.exists)
                {
                    result += arc.data.begin();
                }
            }
            result.Normalize();
            return result; // FIXME: Vector3.zero can be returned
        }

        public Shape shape
        {
            get
            {
                return shape_variable;
            }
        }

        private void Start()
        {
            initialize();
            PlanetariaCache.self.cache(this);
        }

        private void Reset()
        {
            shape_variable = new Shape(new List<GeospatialCurve>(), true, true);
            ignore = new List<Block>();
            active = true;
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
                internal_transform = this.GetComponent<Transform>();
            }
        }

        private void OnDestroy()
        {
            PlanetariaCache.self.uncache(this);
        }

        public static PlanetariaPhysicMaterial fallback;
        [SerializeField] public bool active_variable;
        [SerializeField] public bool is_dynamic; // FIXME: move to PlanetariaRigidbody
        [SerializeField] public bool is_platform;
        [SerializeField] public PlanetariaPhysicMaterial material = fallback;
        [SerializeField] [HideInInspector] public new PlanetariaTransform transform;
        [SerializeField] [HideInInspector] public Transform internal_transform;
        [SerializeField] private Shape shape_variable;
        [SerializeField] public List<Block> ignore;
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