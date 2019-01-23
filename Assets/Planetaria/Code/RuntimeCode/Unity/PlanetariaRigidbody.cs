using System;
using UnityEngine;
using Unity.Entities;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    [RequireComponent(typeof(GameObjectEntity))]
    [RequireComponent(typeof(PlanetariaRigidbody1))] // this is so dumb
    [RequireComponent(typeof(PlanetariaRigidbody2))]
    [RequireComponent(typeof(PlanetariaRigidbody3))]
    [RequireComponent(typeof(PlanetariaRigidbody4))]
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
            Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
            if (entity_manager.Exists(entity))
            {
                entity_manager.RemoveComponent(entity, typeof(PlanetariaVelocityComponent));
                entity_manager.RemoveComponent(entity, typeof(PlanetariaGravityComponent));
                if (entity_manager.HasComponent<PlanetariaAccelerationComponent>(entity))
                {
                    entity_manager.RemoveComponent(entity, typeof(PlanetariaAccelerationComponent));
                }
                if (entity_manager.HasComponent<PlanetariaRigidbodyAerialComponent>(entity))
                {
                    entity_manager.RemoveComponent(entity, typeof(PlanetariaRigidbodyAerialComponent));
                }
            }
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
            if (entity_manager == null)
            {
                entity_manager = World.Active.GetOrCreateManager<EntityManager>();
            }
            Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
            entity_manager.AddComponent(entity, typeof(PlanetariaVelocityComponent));
            entity_manager.AddComponent(entity, typeof(PlanetariaAccelerationComponent)); // FIXME: only needs to be attached if there is non-zero gravity
            entity_manager.AddComponent(entity, typeof(PlanetariaGravityComponent));
            entity_manager.AddComponent(entity, typeof(PlanetariaRigidbodyAerialComponent));
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
        }

        /// <summary>
        /// Mutator - view velocity based on relative values (positive y is local "up", positive x is local "right") with respect to the object
        /// </summary>
        public Vector2 relative_velocity
        {
            get
            {
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                Vector3 position = entity_manager.GetComponentData<PlanetariaPositionComponent>(entity).data;
                Vector3 up = entity_manager.GetComponentData<PlanetariaDirectionComponent>(entity).data;
                Vector3 right = Vector3.Cross(up, position);
                float x = Vector3.Dot(internal_velocity, internal_transform.right);
                float y = Vector3.Dot(internal_velocity, internal_transform.up);
                return new Vector2(x, y);
            }
            set
            {
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                Vector3 position = entity_manager.GetComponentData<PlanetariaPositionComponent>(entity).data;
                Vector3 up = entity_manager.GetComponentData<PlanetariaDirectionComponent>(entity).data;
                Vector3 right = Vector3.Cross(up, position);
                Vector3 x = internal_transform.right*value.x;
                Vector3 y = internal_transform.up*value.y;
                internal_velocity = x + y;
            }
        }

        /// <summary>
        /// Mutator - view velocity based on absolute values (positive y is "North"/"up", positive x is "East"/"right") with respect to the planetarium
        /// </summary>
        public Vector2 absolute_velocity
        {
            get
            {
                float x = Vector3.Dot(internal_velocity, Bearing.east(position));
                float y = Vector3.Dot(internal_velocity, Bearing.north(position));
                return new Vector2(x, y);
            }
            set
            {
                Vector3 x = Bearing.east(position) * value.x;
                Vector3 y = Bearing.north(position) * value.y;
                internal_velocity = x + y;
            }
        }

        public Vector3 internal_velocity
        {
            get
            {
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                Vector3 velocity = entity_manager.GetComponentData<PlanetariaVelocityComponent>(entity).data;
                if (!entity_manager.HasComponent<PlanetariaRigidbodyAerialComponent>(entity))
                {
                    Vector3 position = entity_manager.GetComponentData<PlanetariaPositionComponent>(entity).data;
                    Vector3 up = entity_manager.GetComponentData<PlanetariaDirectionComponent>(entity).data;
                    Vector3 right = Vector3.Cross(up, position);
                    velocity = velocity.x*right + velocity.y*up;
                }
                return velocity;
            }
            private set
            {
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                Vector3 velocity = value;
                if (!entity_manager.HasComponent<PlanetariaRigidbodyAerialComponent>(entity))
                {
                    Vector3 position = entity_manager.GetComponentData<PlanetariaPositionComponent>(entity).data;
                    Vector3 up = entity_manager.GetComponentData<PlanetariaDirectionComponent>(entity).data;
                    Vector3 right = Vector3.Cross(up, position);
                    velocity = new Vector3(Vector3.Dot(velocity, right), Vector3.Dot(velocity, up), 0);
                }
                entity_manager.SetComponentData<PlanetariaVelocityComponent>(entity, new PlanetariaVelocityComponent { data = velocity });
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
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                return entity_manager.GetComponentData<PlanetariaPreviousPositionComponent>(entity).data;
            }
        }

        [SerializeField] [HideInInspector] private Transform internal_transform;
        [SerializeField] [HideInInspector] private Rigidbody internal_rigidbody;
        //[SerializeField] [HideInInspector] private optional<CollisionObserver> observer;
        //[SerializeField] [HideInInspector] private BlockCollision collision;
        
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