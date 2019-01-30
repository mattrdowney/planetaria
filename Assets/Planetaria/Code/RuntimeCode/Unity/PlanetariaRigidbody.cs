using System;
using UnityEngine;
using Unity.Entities;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    [RequireComponent(typeof(GameObjectEntity))]
    public sealed class PlanetariaRigidbody : PlanetariaComponent
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
                entity_manager.RemoveComponent(entity, typeof(PlanetariaVelocity));
                entity_manager.RemoveComponent(entity, typeof(PlanetariaGravity));
                if (entity_manager.HasComponent<PlanetariaAcceleration>(entity))
                {
                    entity_manager.RemoveComponent(entity, typeof(PlanetariaAcceleration));
                }
                if (entity_manager.HasComponent<PlanetariaRigidbodyUnrestrained>(entity))
                {
                    entity_manager.RemoveComponent(entity, typeof(PlanetariaRigidbodyUnrestrained));
                }
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
            //collider = this.GetOrAddComponent<PlanetariaCollider>();
            if (internal_rigidbody == null)
            {
                internal_rigidbody = Miscellaneous.GetOrAddComponent<Rigidbody>(this);
            }
            //if (entity_manager == null)
            //{
            //    entity_manager = World.Active.GetOrCreateManager<EntityManager>();
            //}
            //Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
            //entity_manager.AddComponent(entity, typeof(PlanetariaVelocity));
            //entity_manager.AddComponent(entity, typeof(PlanetariaAcceleration)); // FIXME: only needs to be attached if there is non-zero gravity
            //entity_manager.AddComponent(entity, typeof(PlanetariaGravity));
            //entity_manager.AddComponent(entity, typeof(PlanetariaRigidbodyUnrestrained));
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
        }

        /// <summary>
        /// Mutator - view velocity based on absolute values (positive y is "North"/"up", positive x is "East"/"right") with respect to the planetarium
        /// </summary>
        public Vector2 absolute_velocity
        {
            get
            {
                throw new Exception();
            }
            set
            {
                velocity_variable = value;
                velocity_dirty_variable = true;
                dirty_variable = true;
                velocity_type_variable = VelocityMode.Absolute;
            }
        }

        public void clean()
        {
            dirty_variable = false;
            gravity_dirty_variable = false;
            velocity_dirty_variable = false;
        }

        public bool dirty
        {
            get
            {
                return dirty_variable;
            }
        }

        /// <summary>
        /// Mutator - sets the gravity well of the object. The direction (after normalization) is where the object will move towards; the magnitude is how quickly it will accelerate towards that position.
        /// </summary>
        public Vector3 gravity
        {
            get
            {
                return gravity_variable;
            }
            private set
            {
                gravity_variable = value;
                gravity_dirty_variable = true;
                dirty_variable = true;
            }
        }

        public bool gravity_dirty
        {
            get
            {
                return gravity_dirty_variable;
            }
        }

        /// <summary>
        /// Mutator - view velocity based on relative values (positive y is local "up", positive x is local "right") with respect to the object
        /// </summary>
        public Vector2 relative_velocity
        {
            get
            {
                throw new Exception();
            }
            set
            {
                velocity_variable = value;
                velocity_dirty_variable = true;
                dirty_variable = true;
                velocity_type_variable = VelocityMode.Relative;
            }
        }

        public Vector3 internal_velocity
        {
            get
            {
                throw new Exception();
            }
            private set
            {
                throw new Exception();
            }
        }

        public Vector3 position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Vector3 previous_position
        {
            get
            {
                throw new Exception();
            }
        }

        public bool velocity_dirty
        {
            get
            {
                return velocity_dirty_variable;
            }
        }

        public VelocityMode velocity_type
        {
            get
            {
                return velocity_type_variable;
            }
        }

        public enum VelocityMode { Relative, Absolute }

        [SerializeField] [HideInInspector] private Transform internal_transform;
        [SerializeField] [HideInInspector] private Rigidbody internal_rigidbody;
        //[SerializeField] [HideInInspector] private optional<CollisionObserver> observer;
        //[SerializeField] [HideInInspector] private BlockCollision collision;

        [SerializeField] private Vector3 gravity_variable;
        [SerializeField] private Vector2 velocity_variable;

        [SerializeField] [HideInInspector] private VelocityMode velocity_type_variable;
        [SerializeField] [HideInInspector] private bool dirty_variable;
        [SerializeField] [HideInInspector] private bool gravity_dirty_variable;
        [SerializeField] [HideInInspector] private bool velocity_dirty_variable;

        
        //private static EntityManager entity_manager;
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