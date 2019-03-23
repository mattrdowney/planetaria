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
        }

        // Properties

        public int childCount
        {
            get { return internal_transform.childCount; }
        }

        /// <summary>
        /// Property - direction/"facing"/"forward"/"up".
        /// </summary>
        public Vector3 direction
        {
            get
            {
                return internal_transform.up;
            }
            set
            {
                internal_transform.rotation = Quaternion.LookRotation(position, value);
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
        
        public NormalizedCartesianCoordinates localPosition // FIXME: really fix this...
        {
            get { return new NormalizedCartesianCoordinates(internal_transform.forward); }
            set { internal_transform.rotation = Quaternion.LookRotation(internal_transform.forward, Vector3.up); }
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

        /// <summary>
        /// Property - position (on a unit sphere). Maintains old direction/"facing"/"forward"/"up" direction if set.
        /// </summary>
        public Vector3 position
        {
            get
            {
                return internal_transform.forward;
            }
            set
            {
                Vector3 current_position = internal_transform.forward;
                Vector3 next_position = value;
                if (next_position == Vector3.zero)
                {
                    Debug.LogError("Critical Error" + this.gameObject.name + ":" + current_position);
                }
                if (current_position != next_position)
                {
                    Quaternion delta_rotation = Quaternion.FromToRotation(current_position, next_position);
                    Vector3 adjusted_direction = delta_rotation * direction;
                    internal_transform.rotation = Quaternion.LookRotation(next_position, adjusted_direction);
                }
            }
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
            return Miscellaneous.GetOrAddComponent<PlanetariaTransform>(internal_transform.Find(name));
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
            internal_transform.rotation = Quaternion.LookRotation(internal_transform.forward, target.position);
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

        public void SetDirection(Vector3 next_direction)
        {
            direction = next_direction;
        }

        public void SetParent(PlanetariaTransform transformation)
        {
            internal_transform.SetParent(transformation.internal_transform);
        }

        public void SetPosition(Vector3 next_position)
        {
            position = next_position;
        }

        /// <summary>
        /// Mutator - Make object stand in "next_position" and face towards "next_direction".
        /// </summary>
        /// <param name="next_position">The position of the object on a unit sphere.</param>
        /// <param name="next_direction">The direction the object faces. The object will rotate towards "direction". (E.g. arc.normal() [dynamic] or Vector3.up [static].)</param>
        public void SetPositionAndDirection(Vector3 next_position, Vector3 next_direction)
        {
            internal_transform.rotation = Quaternion.LookRotation(next_position, next_direction);
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
        [SerializeField] private float scale_variable; // I thought this could be combined with transform.localScale/lossyScale, but it can't apparently
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