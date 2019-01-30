using System;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    [RequireComponent(typeof(GameObjectEntity))]
    public sealed class PlanetariaTransform : PlanetariaComponent
    {
        protected override sealed void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override sealed void OnDestroy()
        {
            base.OnDestroy();
            /*
            Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
            if (entity_manager.Exists(entity))
            {
                entity_manager.RemoveComponent(entity, typeof(PlanetariaPosition));
                entity_manager.RemoveComponent(entity, typeof(PlanetariaPreviousPosition));
                entity_manager.RemoveComponent(entity, typeof(PlanetariaDirection));
                entity_manager.RemoveComponent(entity, typeof(PlanetariaDirectionDirty));
                entity_manager.RemoveComponent(entity, typeof(PlanetariaScale));
            }
            */
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
            /*
            if (!internal_collider.exists) // FIXME: components added at runtime won't link properly
            {
                internal_collider = internal_transform.GetComponent<PlanetariaCollider>(); // TODO: better way to do this - observer pattern
            }
            */
            if (!internal_renderer.exists)
            {
                internal_renderer = internal_transform.GetComponent<PlanetariaRenderer>();
            }
            /*
            if (entity_manager == null)
            {
                entity_manager = World.Active.GetOrCreateManager<EntityManager>();
            }
            Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
            */
            //GameObjectEntity.AddToEntityManager()
            //entity_manager.AddComponent(entity, typeof(PlanetariaPosition));
            /*
            var data = something.AddComponent<ComponentDataType>();
            data.data = new ComponentDataType { data = intialized value };
            GameObjectEntity.AddToEntityManager(entity_manager, data);
            */
            //entity_manager.AddComponent(entity, typeof(PlanetariaPreviousPosition));
            //entity_manager.AddComponent(entity, typeof(PlanetariaDirection));
            //entity_manager.AddComponent(entity, typeof(PlanetariaDirectionDirty));
            //entity_manager.AddComponent(entity, typeof(PlanetariaScale));
            //entity_manager.RemoveComponent(entity, typeof(CopyTransformToGameObjectComponent));
            //entity_manager.RemoveComponent(entity, typeof(CopyTransformFromGameObjectComponent));
            local_position = internal_transform.forward;
            local_direction = internal_transform.up;
            local_scale = 1f;
        }

        // Properties

        public int childCount
        {
            get { return internal_transform.childCount; }
        }

        public void clean()
        {
            dirty_variable = false;
            position_dirty_variable = false;
            direction_dirty_variable = false;
            scale_dirty_variable = false;
        }

        /// <summary>
        /// Property - direction/"facing"/"forward"/"up".
        /// </summary>
        public Vector3 direction
        {
            get
            {
                throw new Exception();
            }
            set
            {
                throw new Exception();
            }
        }

        public bool direction_dirty
        {
            get
            {
                return direction_dirty_variable;
            }
        }

        public bool dirty
        {
            get
            {
                return dirty_variable;
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
        
        public Vector3 local_direction
        {
            get
            {
                throw new Exception();
            }
            set
            {
                direction_variable = value;
                direction_dirty_variable = true;
                dirty_variable = true;
            }
        }

        public Vector3 local_position
        {
            get
            {
                throw new Exception();
            }
            set
            {
                // FIXME: needs to modify velocity of rigidbody (if a rigidbody exists)
                position_variable = value;
                position_dirty_variable = true;
                dirty_variable = true;
            }
        }
        
        /// <summary>
        /// The diameter of the player - divide by two when you need the radius/extrusion.
        /// </summary>
        public float local_scale
        {
            get
            {
                throw new Exception();
            }
            set
            {
                scale_variable = value;
                scale_dirty_variable = true;
                dirty_variable = true;
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
                throw new Exception();
            }
            set
            {
                throw new Exception();
            }
        }

        public bool position_dirty
        {
            get
            {
                return position_dirty_variable;
            }
        }
        
        public PlanetariaTransform root
        {
            get { return internal_transform.root.GetComponent<PlanetariaTransform>(); }
        }

        public float scale // TODO: scale is [0,2PI] or maybe [-2PI, +2PI] as intended, but maybe I should make it [0,1] or [-1, +1] (with larger multipliers possible, but that is the code's intention). 
        {
            get
            {
                throw new Exception();
            }
            set
            {
                throw new Exception();
            }
        }

        public bool scale_dirty
        {
            get
            {
                return scale_dirty_variable;
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

        // CONSIDER: implement Translate() ? - probably, relative to direction move Vector2 distance.
        [SerializeField] [HideInInspector] private Transform internal_transform;
        //[SerializeField] [HideInInspector] private optional<PlanetariaCollider> internal_collider; // Observer pattern would be more elegant but slower
        [SerializeField] [HideInInspector] private optional<PlanetariaRenderer> internal_renderer;
        [SerializeField] [HideInInspector] private optional<PlanetariaRigidbody> internal_rigidbody; // FIXME: implement

        [SerializeField] private Vector3 position_variable;
        [SerializeField] private Vector3 direction_variable;
        [SerializeField] private float scale_variable;
        
        [SerializeField] [HideInInspector] private bool dirty_variable;
        [SerializeField] [HideInInspector] private bool position_dirty_variable;
        [SerializeField] [HideInInspector] private bool direction_dirty_variable;
        [SerializeField] [HideInInspector] private bool scale_dirty_variable;

        private static EntityManager entity_manager;
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