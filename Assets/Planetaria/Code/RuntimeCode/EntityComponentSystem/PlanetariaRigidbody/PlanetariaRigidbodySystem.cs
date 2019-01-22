using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;

namespace Planetaria
{
    [UpdateBefore(typeof(FixedUpdate))] // TODO: verify this compiles as intended
    [UpdateBefore(typeof(PlanetariaTransformSystem))]
    [UpdateAfter(typeof(PlanetariaRigidbody))]
    public class PlanetariaRigidbodySystem : JobComponentSystem
    {
        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaRigidbodyAerialComponent))]
        struct PlanetariaRigidbodyAerialMove : IJobProcessComponentData<PlanetariaPositionComponent, PlanetariaVelocityComponent>
        {
            public float delta_time;

            public void Execute(ref PlanetariaPositionComponent position,
                    ref PlanetariaVelocityComponent velocity) // NOTE: speed is [ReadOnly], direction is reevaluated for new position
            {
                float quarter_rotation = (float) math.PI/2;
                float current_speed = math.length(velocity.data);
                float displacement = current_speed * delta_time;
                float3 current_direction = velocity.data / current_speed;
                float3 next_position = position.data * math.cos(displacement) + current_direction * math.sin(displacement); // Note: when velocity = Vector3.zero, it luckily still returns "position" intact.
                float3 next_velocity = position.data * math.cos(displacement + quarter_rotation) + current_direction * math.sin(displacement + quarter_rotation);
                position = new PlanetariaPositionComponent { data = next_position };
                velocity = new PlanetariaVelocityComponent { data = math.normalizesafe(next_velocity) * current_speed }; // FIXME: I thought this was numerically stable, but it seems to create more energy.
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaRigidbodyAerialComponent))]
        struct PlanetariaRigidbodyAerialAccelerate : IJobProcessComponentData<PlanetariaVelocityComponent, PlanetariaAccelerationComponent>
        {
            public float delta_time; // intended for Time.fixedDeltaTime/2 (NOTE: division by two is related to integration strategy)

            public void Execute(ref PlanetariaVelocityComponent velocity,
                    [ReadOnly] ref PlanetariaAccelerationComponent acceleration)
            {
                velocity = new PlanetariaVelocityComponent { data = velocity.data + acceleration.data * (delta_time) };
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaRigidbodyAerialComponent))]
        struct PlanetariaRigidbodyAerialGravitate : IJobProcessComponentData<PlanetariaAccelerationComponent, PlanetariaGravityComponent, PlanetariaPositionComponent>
        {
            public void Execute(ref PlanetariaAccelerationComponent acceleration,
                    [ReadOnly] ref PlanetariaGravityComponent gravity,
                    [ReadOnly] ref PlanetariaPositionComponent position)
            {
                float3 gradient = math.normalizesafe(Vector3.ProjectOnPlane(gravity.data, position.data));
                float magnitude = math.length(gravity.data); // NOTE: you cannot combine normalize/length
                acceleration = new PlanetariaAccelerationComponent { data = gradient * magnitude };
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaRigidbodyAerialComponent))]
        struct PlanetariaRigidbodyAirToGroundVelocity : IJobProcessComponentData<PlanetariaVelocityComponent, PlanetariaPositionComponent, PlanetariaDirectionComponent>
        {
            public void Execute(ref PlanetariaVelocityComponent velocity,
                    [ReadOnly] ref PlanetariaPositionComponent position,
                    [ReadOnly] ref PlanetariaDirectionComponent direction) // FIXME: use collision normal (e.g. better for top down games)
            {
                float3 right = math.cross(direction.data, position.data);
                float horizontal_velocity = math.dot(velocity.data, right);
                float vertical_velocity = math.dot(velocity.data, direction.data);
                velocity = new PlanetariaVelocityComponent { data = new float3(horizontal_velocity, vertical_velocity, 0) };
            }
        }

        [BurstCompile]
        [RequireComponentTag(typeof(PlanetariaRigidbodyAerialComponent))]
        struct PlanetariaRigidbodyGroundToAirVelocity : IJobProcessComponentData<PlanetariaVelocityComponent, PlanetariaPositionComponent, PlanetariaDirectionComponent>
        {
            public void Execute(ref PlanetariaVelocityComponent velocity,
                    [ReadOnly] ref PlanetariaPositionComponent position,
                    [ReadOnly] ref PlanetariaDirectionComponent direction) // FIXME: use collision normal (e.g. better for top down games)
            {
                float3 right = math.cross(direction.data, position.data);
                float3 horizontal_velocity = velocity.data.x * right;
                float3 vertical_velocity = velocity.data.y * direction.data;
                velocity = new PlanetariaVelocityComponent { data = horizontal_velocity + vertical_velocity };
            }
        }

        protected override JobHandle OnUpdate(JobHandle input_dependencies)
        {
            // On the highest level, just the following four steps are performed:
            // velocity += acceleration * (delta_time/2)
            // position += velocity * (delta_time)
            // acceleration = f(n)
            // velocity += acceleration * (delta_time/2) // FIXME: this has to be 1) implement incorrectly or 2) not useful for Planetaria (or at least useful enough to justify it's existence)

            // I am making a bet this is relevant in spherical coordinates (it is Euler isn't it?): http://openarena.ws/board/index.php?topic=5100.0
            // Wikipedia: https://en.wikipedia.org/wiki/Leapfrog_integration
            // "This is especially useful when computing orbital dynamics, as many other integration schemes, such as the (order-4) Runge-Kutta method, do not conserve energy and allow the system to drift substantially over time." - Wikipedia
            // http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
            // http://www.richardlord.net/presentations/physics-for-flash-games.html

            // AERIAL
            var first_velocity_change = new PlanetariaRigidbodyAerialAccelerate
            {
                delta_time = Time.deltaTime/2, // divided by two because this happens twice
            };
            JobHandle aerial = first_velocity_change.Schedule<PlanetariaRigidbodyAerialAccelerate>(this, input_dependencies);
            var position_change = new PlanetariaRigidbodyAerialMove()
            {
                delta_time = Time.deltaTime,
            };
            aerial = position_change.Schedule<PlanetariaRigidbodyAerialMove>(this, aerial);
            var acceleration_change = new PlanetariaRigidbodyAerialGravitate();
            aerial = acceleration_change.Schedule<PlanetariaRigidbodyAerialGravitate>(this, aerial);

            // TODO: GROUNDED

            /*
            if (observer.exists && observer.data.colliding()) // grounded
            {
                JobHandle grounded = ABC.Schedule<T>(this, input_dependencies);
                grounded_track(component, Time.fixedDeltaTime/2);
                grounded = XYZ.Schedule<S>(this, grounded);
                grounded_position(component);
                grounded_accelerate(component, Time.fixedDeltaTime);
            }
            */

            // TODO: barrier for aerial -> grounded (and grounded -> aerial)
            
            // AERIAL
            
            var second_velocity_change = new PlanetariaRigidbodyAerialAccelerate
            {
                delta_time = Time.deltaTime/2,
            };
            aerial = first_velocity_change.Schedule<PlanetariaRigidbodyAerialAccelerate>(this, aerial); // FIXME: barrier

            // TODO: GROUNDED

            // grounded_track(component, Time.fixedDeltaTime/2);

            return aerial; // FIXME: JobHandle.CombineDependencies(aerial, grounded);
        }

        struct PlanetariaRigidbodyGroup
        {
            public ComponentArray<PlanetariaRigidbody> Component; //PlanetariaRigidbody is a PlanetariaMonoBehaviour component
            public EntityArray Entity;
            public int Length;
        }
 
        /*
        [Inject] PlanetariaRigidbodyGroup group;
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            for (int i = 0; i < group.Length; i++)
            {
                PostUpdateCommands.AddComponent(group.Entity[i], new PlanetariaVelocityComponent());
            }
            for (int i = 0; i < group.Length; i++)
            {
                PostUpdateCommands.AddComponent(group.Entity[i], new PlanetariaGravityComponent());
            }
            // add Aerial tag as well
        }
        */

        /*private void grounded_position(PlanetariaRigidbodyComponent component)
        {
            collision.geometry_visitor.move_position(component.planetaria_rigidbody_data.horizontal_velocity * Time.fixedDeltaTime);
            component.transform.position = collision.geometry_visitor.position(); // NOTE: required so get_acceleration() functions
            // project velocity
        }

        private void grounded_track(PlanetariaRigidbodyComponent component, float delta_time)
        {
            component.planetaria_rigidbody_data.horizontal_velocity += component.planetaria_rigidbody_data.horizontal_acceleration * delta_time;
            float friction = Mathf.Abs(collision.friction * -component.planetaria_rigidbody_data.vertical_velocity);
            float speed = Mathf.Abs(component.planetaria_rigidbody_data.horizontal_velocity);
            component.planetaria_rigidbody_data.horizontal_velocity -= Mathf.Sign(component.planetaria_rigidbody_data.horizontal_velocity)*Mathf.Min(speed, friction);
            component.planetaria_rigidbody_data.vertical_velocity = 0;
        }

        private void grounded_accelerate(PlanetariaRigidbodyComponent component, float delta)
        {
            Vector3 normal = collision.geometry_visitor.normal();
            Vector3 right = Bearing.right(collision.geometry_visitor.position(), normal);

            component.planetaria_rigidbody_data.acceleration = get_acceleration(component);
            component.planetaria_rigidbody_data.horizontal_acceleration = Vector3.Dot(component.planetaria_rigidbody_data.acceleration, right);
            component.planetaria_rigidbody_data.vertical_acceleration = Vector3.Dot(component.planetaria_rigidbody_data.acceleration, normal) - collision.magnetism;
            component.planetaria_rigidbody_data.vertical_velocity += component.planetaria_rigidbody_data.vertical_acceleration*Time.fixedDeltaTime;
            if (!collision.grounded(component.planetaria_rigidbody_data.velocity)) // TODO: check centripedal force
            {
                derail(0, component.planetaria_rigidbody_data.vertical_acceleration*delta); // Force OnCollisionExit, "un-collision" (and accelerate for a frame)
            }
        }*/
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