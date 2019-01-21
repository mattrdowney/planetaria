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
            entity_manager.AddComponent(entity, typeof(PlanetariaGravityComponent));
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
                synchronize_velocity_ground_to_air();
                //float x = Vector3.Dot(velocity, internal_transform.right);
                //float y = Vector3.Dot(velocity, internal_transform.up);
                // return new Vector2(x, y)
                Vector2 result = internal_transform.InverseTransformDirection(planetaria_rigidbody_data.velocity); // z-component unused (should be ~ zero)
                return result;
            }
            set
            {
                //Vector3 x = internal_transform.right*value.x;
                //Vector3 y = internal_transform.up*value.y;
                //velocity = x + y;
                planetaria_rigidbody_data.velocity = internal_transform.TransformDirection(value);
                synchronize_velocity_air_to_ground();
            }
        }

        /// <summary>
        /// Mutator - view velocity based on absolute values (positive y is "North"/"up", positive x is "East"/"right") with respect to the planetarium
        /// </summary>
        public Vector2 absolute_velocity
        {
            get
            {
                synchronize_velocity_ground_to_air();
                float x = Vector3.Dot(planetaria_rigidbody_data.velocity, Bearing.east(get_position()));
                float y = Vector3.Dot(planetaria_rigidbody_data.velocity, Bearing.north(get_position()));
                return new Vector2(x, y);
            }
            set
            {
                Vector3 x = Bearing.east(get_position()) * value.x;
                Vector3 y = Bearing.north(get_position()) * value.y;
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                entity_manager.SetComponentData<PlanetariaVelocityComponent>(entity, new PlanetariaVelocityComponent(x + y));
                synchronize_velocity_air_to_ground();
            }
        }

        public Vector3 internal_velocity
        {
            get
            {
                synchronize_velocity_ground_to_air();
                Entity entity = this.gameObject.internal_game_object.GetComponent<GameObjectEntity>().Entity;
                return entity_manager.GetComponentData<PlanetariaVelocityComponent>(entity).velocity;
            }
        }

        public Vector3 get_position()
        {
            // FIXME: needs to modify velocity of rigidbody
            return transform.position; // OPTIMIZE: this is crazy inefficient
        }

        public Vector3 get_previous_position()
        {
            return planetaria_rigidbody_data.previous_position;
        }

        [SerializeField] [HideInInspector] private Transform internal_transform;
        [SerializeField] [HideInInspector] private Rigidbody internal_rigidbody;
        [SerializeField] [HideInInspector] private optional<CollisionObserver> observer;
        [SerializeField] [HideInInspector] private BlockCollision collision;
        
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