using System;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public sealed class PlanetariaTransform : PlanetariaComponent // CONSIDER: struct? Disadvantage(?): multiple references don't know of each other's existence and directions (broken?) : can't access parent's direction
    {
        protected override sealed void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override sealed void Reset()
        {
            base.Reset();
            initialize();
        }

        private void initialize()
        {
            if (internal_transform == null)
            {
                internal_transform = gameObject.internal_game_object.GetComponent<Transform>();
            }
            if (!internal_collider.exists) // FIXME: components added at runtime won't link properly
            {
                internal_collider = internal_transform.GetComponent<PlanetariaCollider>(); // TODO: better way to do this - observer pattern
            }
            if (!internal_renderer.exists)
            {
                internal_renderer = internal_transform.GetComponent<PlanetariaRenderer>();
            }
            direction_variable = internal_transform.up;
            scale_variable = internal_transform.lossyScale.x;
        }

        // Properties

        public int childCount
        {
            get { return internal_transform.childCount; }
        }

        /// <summary>
        /// The direction the object faces. The object will rotate towards "direction".
        /// </summary>
        /// <example>arc.normal() [dynamic] or Vector3.up [static]</example>
        public NormalizedCartesianCoordinates direction // FIXME: TODO: localDirection, how do parents affect this?
        {
            get
            {
                return new NormalizedCartesianCoordinates(direction_variable);
            }
            set
            {
                direction_variable = value.data;
                internal_transform.rotation = Quaternion.LookRotation(internal_transform.forward, direction_variable);
            }
        }

        public bool hasChanged
        {
            get { return internal_transform.hasChanged; }
            set { internal_transform.hasChanged = value; }
        }

        public int hierarchyCapacity
        {
            get { return internal_transform.hierarchyCapacity; }
            set { internal_transform.hierarchyCapacity = value; }
        }

        public int hierarchyCount
        {
            get { return internal_transform.hierarchyCount; }
        }
        
        public NormalizedCartesianCoordinates localPosition // FIXME:
        {
            get { return new NormalizedCartesianCoordinates(internal_transform.forward); }
            set { internal_transform.rotation = Quaternion.LookRotation(internal_transform.forward, direction_variable); }
        }

        /// <summary>
        /// The diameter of the player - divide by two when you need the radius/extrusion.
        /// </summary>
        public float localScale
        {
            get { return scale_variable; }
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

        public Matrix4x4 localToWorldMatrix
        {
            get { return internal_transform.localToWorldMatrix; }
        }

        public PlanetariaTransform parent
        {
            get
            {
                return internal_transform.parent ? internal_transform.parent.GetComponent<PlanetariaTransform>() : null;
            }
            set { SetParent(value); }
        }

        public NormalizedCartesianCoordinates position
        {
            get { return new NormalizedCartesianCoordinates(internal_transform.forward); }
            set { internal_transform.rotation = Quaternion.LookRotation(value.data, direction_variable); }
        }

        public PlanetariaTransform root
        {
            get { return internal_transform.root.GetComponent<PlanetariaTransform>(); }
        }

        public float scale
        {
            get
            {
                if (parent == null) // base case
                {
                    return scale_variable;
                }
                return parent.scale * scale_variable; // recursive case
            }
            set
            {
                localScale = value;
                if (internal_transform.parent != null)
                {
                    localScale /= parent.scale; // TODO: verify
                }
            }
        }

        public Matrix4x4 worldToLocalMatrix
        {
            get { return internal_transform.worldToLocalMatrix; }
        }

        // Public Methods

        public void DetachChildren()
        {
            internal_transform.DetachChildren();
        }

        public PlanetariaTransform Find(string name)
        {
            return internal_transform.Find(name).GetComponent<PlanetariaTransform>();
        }

        public PlanetariaTransform GetChild(int index)
        {
            return internal_transform.GetChild(index).GetComponent<PlanetariaTransform>();
        }

        public int GetSiblingIndex()
        {
            return internal_transform.GetSiblingIndex();
        }
        
        public NormalizedCartesianCoordinates InverseTransformDirection(NormalizedCartesianCoordinates world_space)
        {
            return (NormalizedCartesianCoordinates) internal_transform.InverseTransformDirection(world_space.data);
        }

        public bool IsChildOf(PlanetariaTransform transformation)
        {
            return internal_transform.IsChildOf(transformation.internal_transform);
        }

        public void LookAt(PlanetariaTransform target)
        {
            direction_variable = target.position.data;
            internal_transform.rotation = Quaternion.LookRotation(internal_transform.forward, direction_variable);
        }
        
        // CONSIDER: implement RotateAround ?

        public void SetAsFirstSibling()
        {
            internal_transform.SetAsFirstSibling();
        }

        public void SetAsLastSibling()
        {
            internal_transform.SetAsLastSibling();
        }

        public void SetParent(PlanetariaTransform transformation)
        {
            internal_transform.SetParent(transformation.internal_transform);
        }

        public void SetPositionAndDirection(Vector3 position, Vector3 direction)
        {
            direction_variable = direction;
            internal_transform.rotation = Quaternion.LookRotation(position, direction);
        }

        public void SetSiblingIndex(int index)
        {
            internal_transform.SetSiblingIndex(index);
        }

        public NormalizedCartesianCoordinates TransformDirection(NormalizedCartesianCoordinates local_space)
        {
            return (NormalizedCartesianCoordinates)internal_transform.TransformDirection(local_space.data);
        }

        // CONSIDER: implement Translate() ?

        [SerializeField] [HideInInspector] private Transform internal_transform;
        [SerializeField] [HideInInspector] private optional<PlanetariaCollider> internal_collider; // Observer pattern would be more elegant but slower
        [SerializeField] [HideInInspector] private optional<PlanetariaRenderer> internal_renderer;
        [SerializeField] [HideInInspector] private optional<PlanetariaRigidbody> internal_rigidbody; // FIXME: implement

        //private Planetarium planetarium_variable; // cartesian_transform's position
        [SerializeField] private Vector3 direction_variable; // CONSIDER: how do non-normalized vectors affect Quaternion.LookRotation()? Vector3.zero is the biggest issue.
        [SerializeField] private float scale_variable; // I thought this could be combined with transform.localScale/lossyScale, but it can't apparently
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