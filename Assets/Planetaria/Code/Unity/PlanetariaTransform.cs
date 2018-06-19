using System;
using UnityEngine;

namespace Planetaria
{
    // FIXME: find a way to serialize
    [DisallowMultipleComponent]
    [Serializable]
    public sealed class PlanetariaTransform : PlanetariaComponent // CONSIDER: C#-style extension methods only: no need for separate object at risk of extra confusion; NOTE: some concepts like direction would no longer work in the same way
    {
        protected override void Awake()
        {
            initialize();
        }

        protected override void OnDestroy() { }

        private void Reset()
        {
            initialize();
        }

        private void initialize()
        {
            if (internal_transform == null)
            {
                internal_transform = transform.internal_transform;
            }
            if (!internal_collider.exists)
            {
                internal_collider = internal_transform.GetComponent<PlanetariaCollider>(); // TODO: better way to do this - observer pattern
            }
            if (!internal_renderer.exists)
            {
                internal_renderer = internal_transform.GetComponent<PlanetariaRenderer>();
            }
            position = new NormalizedCartesianCoordinates(internal_transform.forward);
            direction = new NormalizedCartesianCoordinates(internal_transform.up);
            scale = scale_variable;
        }

        public NormalizedCartesianCoordinates position
        {
            get
            {
                return new NormalizedCartesianCoordinates(position_variable);
            }
            set
            {
                position_variable = value.data;
                internal_transform.rotation = Quaternion.LookRotation(position_variable, direction_variable);
            }
        }

        /// <summary>
        /// The direction the object faces. The object will rotate towards "direction".
        /// </summary>
        /// <example>arc.normal() [dynamic] or Vector3.up [static]</example>
        public NormalizedCartesianCoordinates direction
        {
            get
            {
                return new NormalizedCartesianCoordinates(direction_variable);
            }
            set
            {
                direction_variable = value.data;
                internal_transform.rotation = Quaternion.LookRotation(position_variable, direction_variable);
            }
        }

        /// <summary>
        /// The diameter of the player - divide by two when you need the radius/extrusion.
        /// </summary>
        public float scale
        {
            get
            {
                return scale_variable;
            }
            set
            {
                scale_variable = value;
                if (internal_collider.exists)
                {
                    internal_collider.data.scale = scale; // TODO: check
                }
                if (internal_renderer.exists)
                {
                    internal_renderer.data.scale = scale; // TODO: check
                }
            }
        }

        [SerializeField] [HideInInspector] private Transform internal_transform; // FIXME: hide these 3
        [SerializeField] [HideInInspector] private optional<PlanetariaCollider> internal_collider; // Observer pattern would be more elegant but slower
        [SerializeField] [HideInInspector] private optional<PlanetariaRenderer> internal_renderer;

        //private Planetarium planetarium_variable; // cartesian_transform's position
        [SerializeField] [HideInInspector] private Vector3 position_variable = Vector3.forward; // CONSIDER: [HideInInspector] 
        [SerializeField] [HideInInspector] private Vector3 direction_variable = Vector3.up; // CONSIDER: how do non-normalized vectors affect Quaternion.LookRotation()? Vector3.zero is the biggest issue.
        [SerializeField] private float scale_variable; // CONSIDER: without editor OnValidate() setting Transform.scale, these methods are extremely misleading
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