using System;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
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
            if (planetaria_rigidbody_data == null)
            {
                planetaria_rigidbody_data = gameObject.internal_game_object.GetComponent<PlanetariaRigidbodyData>();
            }
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
            planetaria_rigidbody_data.previous_position = get_position();
            planetaria_rigidbody_data.acceleration = get_acceleration();
        }

        /*
        private void FixedUpdate()
        {
            planetaria_rigidbody_data.previous_position = get_position();
            if (observer.exists && observer.data.colliding()) // grounded
            {
                grounded_track(Time.fixedDeltaTime/2);
                grounded_position();
                grounded_accelerate(Time.fixedDeltaTime);
            }
            else // non-grounded / "aerial"
            {
                // I am making a bet this is relevant in spherical coordinates (it is Euler isn't it?): http://openarena.ws/board/index.php?topic=5100.0
                // Wikipedia: https://en.wikipedia.org/wiki/Leapfrog_integration
                // "This is especially useful when computing orbital dynamics, as many other integration schemes, such as the (order-4) Runge-Kutta method, do not conserve energy and allow the system to drift substantially over time." - Wikipedia
                // http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
                // http://www.richardlord.net/presentations/physics-for-flash-games.html
                planetaria_rigidbody_data.velocity += planetaria_rigidbody_data.acceleration * (Time.fixedDeltaTime/2);
                aerial_move(planetaria_rigidbody_data.velocity.magnitude * Time.fixedDeltaTime);
                planetaria_rigidbody_data.acceleration = get_acceleration();
            }

            if (observer.exists && observer.data.colliding()) // grounded
            {
                grounded_track(Time.fixedDeltaTime/2);
            }
            else // non-grounded / "aerial"
            {
                planetaria_rigidbody_data.velocity += planetaria_rigidbody_data.acceleration * (Time.fixedDeltaTime/2);
            }
        }
        */

        public Vector3 get_acceleration()
        {
            return Bearing.attractor(get_position(), planetaria_rigidbody_data.gravity.normalized)*planetaria_rigidbody_data.gravity.magnitude;
        }

        public bool collide(BlockCollision collision, CollisionObserver observer)
        {
            if (this.observer.exists)
            {
                this.observer.data.clear_block_collision();
            }

            aerial_move(-collision.overshoot); // this only (truly) works with perpendicular vectors?

            this.observer = observer;
            this.collision = collision;
            
            planetaria_rigidbody_data.horizontal_velocity = Vector3.Dot(planetaria_rigidbody_data.velocity, Bearing.right(get_position(), collision.geometry_visitor.normal()));
            planetaria_rigidbody_data.vertical_velocity = Vector3.Dot(planetaria_rigidbody_data.velocity, collision.geometry_visitor.normal());
            if (planetaria_rigidbody_data.vertical_velocity < 0)
            {
                planetaria_rigidbody_data.vertical_velocity *= -collision.elasticity;
            }

            grounded_accelerate(0);

            return this.observer.exists;
        }

        private void aerial_move(float delta)
        {
            Vector3 next_position = PlanetariaMath.spherical_linear_interpolation(get_position(), planetaria_rigidbody_data.velocity.normalized, delta); // Note: when velocity = Vector3.zero, it luckily still returns "position" intact.
            Vector3 next_velocity = PlanetariaMath.spherical_linear_interpolation(get_position(), planetaria_rigidbody_data.velocity.normalized, delta + Mathf.PI/2);
            
            transform.position = next_position;
            planetaria_rigidbody_data.velocity = next_velocity.normalized * planetaria_rigidbody_data.velocity.magnitude; // FIXME: I thought this was numerically stable, but it seems to create more energy.
            //velocity = Vector3.ProjectOnPlane(velocity, get_position()); // TODO: CONSIDER: ensure velocity and position are orthogonal - they seem to desynchronize
            //Debug.DrawRay(get_position(), velocity, Color.green); // draw new velocity (not old one)
        }

        private void grounded_position()
        {
            collision.geometry_visitor.move_position(planetaria_rigidbody_data.horizontal_velocity * Time.fixedDeltaTime);
            transform.position = collision.geometry_visitor.position(); // NOTE: required so get_acceleration() functions
            // project velocity
        }

        private void grounded_track(float delta_time)
        {
            planetaria_rigidbody_data.horizontal_velocity += planetaria_rigidbody_data.horizontal_acceleration * delta_time;
            float friction = Mathf.Abs(collision.friction * -planetaria_rigidbody_data.vertical_velocity);
            float speed = Mathf.Abs(planetaria_rigidbody_data.horizontal_velocity);
            planetaria_rigidbody_data.horizontal_velocity -= Mathf.Sign(planetaria_rigidbody_data.horizontal_velocity)*Mathf.Min(speed, friction);
            planetaria_rigidbody_data.vertical_velocity = 0;
        }

        private void grounded_accelerate(float delta)
        {
            Vector3 normal = collision.geometry_visitor.normal();
            Vector3 right = Bearing.right(collision.geometry_visitor.position(), normal);

            planetaria_rigidbody_data.acceleration = get_acceleration();
            planetaria_rigidbody_data.horizontal_acceleration = Vector3.Dot(planetaria_rigidbody_data.acceleration, right);
            planetaria_rigidbody_data.vertical_acceleration = Vector3.Dot(planetaria_rigidbody_data.acceleration, normal) - collision.magnetism;
            planetaria_rigidbody_data.vertical_velocity += planetaria_rigidbody_data.vertical_acceleration*Time.fixedDeltaTime;
            if (!collision.grounded(internal_velocity)) // TODO: check centripedal force
            {
                derail(0, planetaria_rigidbody_data.vertical_acceleration*delta); // Force OnCollisionExit, "un-collision" (and accelerate for a frame)
            }
        }
        
        public void derail(float x_velocity, float y_velocity)
        {
            if (observer.exists && observer.data.colliding())
            {
                BlockCollision collision = observer.data.collisions()[0];

                collision.geometry_visitor.move_position(0, transform.scale/2 * (1 + 1e-3f)); // extrude the player so they do not accidentally re-collide (immediately) // FIXME: magic number, move to Precision.*
                x_velocity += planetaria_rigidbody_data.horizontal_velocity;
                //y_velocity += vertical_velocity;
                transform.position = collision.geometry_visitor.position();
                Vector3 normal = collision.geometry_visitor.normal();
                Vector3 right = Bearing.right(get_position(), normal);
                planetaria_rigidbody_data.velocity = right*x_velocity + normal*y_velocity;
                Debug.DrawRay(get_position(), planetaria_rigidbody_data.velocity, Color.yellow, 1f);
                planetaria_rigidbody_data.acceleration = get_acceleration();
                // TODO: accelerate vertically
                
                observer.data.clear_block_collision();
                observer = new optional<CollisionObserver>();
            }
        }

        private void synchronize_velocity_ground_to_air()
        {
            if (observer.exists)
            {
                Vector3 x = planetaria_rigidbody_data.horizontal_velocity * Bearing.right(get_position(), collision.geometry_visitor.normal());
                Vector3 y = planetaria_rigidbody_data.vertical_acceleration * Time.fixedDeltaTime * collision.geometry_visitor.normal();
                planetaria_rigidbody_data.velocity = x + y;
            }
        }

        private void synchronize_velocity_air_to_ground()
        {
            if (observer.exists)
            {
                planetaria_rigidbody_data.horizontal_velocity = Vector3.Dot(planetaria_rigidbody_data.velocity, Bearing.right(get_position(), collision.geometry_visitor.normal()));
                planetaria_rigidbody_data.vertical_velocity = Vector3.Dot(planetaria_rigidbody_data.velocity, collision.geometry_visitor.normal());
            }
        }
       
        public bool colliding
        {
            get
            {
                return observer.exists;
            }
        }

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
                planetaria_rigidbody_data.velocity = x + y;
                synchronize_velocity_air_to_ground();
            }
        }

        public Vector3 internal_velocity
        {
            get
            {
                synchronize_velocity_ground_to_air();
                return planetaria_rigidbody_data.velocity;
            }
        }

        public Vector3 get_position()
        {
            // TODO: while this isn't useful now, I could use caching later
            return transform.position;
        }

        public Vector3 get_previous_position()
        {
            return planetaria_rigidbody_data.previous_position;
        }

        /// <summary>
        /// public Vector2 velocity - set velocity based on absolute values; up is north, right is east
        /// public Vector3 position - set velocity based on relative values; begin attractor is south, end repeller is north
        /// </summary>

        [SerializeField] [HideInInspector] private PlanetariaRigidbodyData planetaria_rigidbody_data;
        [SerializeField] [HideInInspector] private Transform internal_transform;
        [SerializeField] [HideInInspector] private Rigidbody internal_rigidbody;
        [SerializeField] [HideInInspector] private optional<CollisionObserver> observer;
        [SerializeField] [HideInInspector] private BlockCollision collision;
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