using UnityEngine;
using Unity.Entities;
using UnityEngine.Experimental.PlayerLoop;

namespace Planetaria
{
    [UpdateBefore(typeof(FixedUpdate))] // TODO: verify this compiles as intended
    [UpdateBefore(typeof(Rigidbody))]
    [UpdateBefore(typeof(PlanetariaTransform))]
    public class PlanetariaRigidbodySystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            foreach (PlanetariaRigidbodyComponent component in GetEntities<PlanetariaRigidbodyComponent>())
            {
                component.planetaria_rigidbody_data.previous_position = component.transform.position;
                /*
                if (observer.exists && observer.data.colliding()) // grounded
                {
                    grounded_track(component, Time.fixedDeltaTime/2);
                    grounded_position(component);
                    grounded_accelerate(component, Time.fixedDeltaTime);
                }
                else // non-grounded / "aerial"
                {
                */
                // I am making a bet this is relevant in spherical coordinates (it is Euler isn't it?): http://openarena.ws/board/index.php?topic=5100.0
                // Wikipedia: https://en.wikipedia.org/wiki/Leapfrog_integration
                // "This is especially useful when computing orbital dynamics, as many other integration schemes, such as the (order-4) Runge-Kutta method, do not conserve energy and allow the system to drift substantially over time." - Wikipedia
                // http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
                // http://www.richardlord.net/presentations/physics-for-flash-games.html
                component.planetaria_rigidbody_data.velocity += component.planetaria_rigidbody_data.acceleration*(Time.fixedDeltaTime/2);
                aerial_move(component, component.planetaria_rigidbody_data.velocity.magnitude*Time.fixedDeltaTime);
                component.planetaria_rigidbody_data.acceleration = get_acceleration(component);
                //}
                /*
                if (observer.exists && observer.data.colliding()) // grounded
                {
                    grounded_track(component, Time.fixedDeltaTime/2);
                }
                else // non-grounded / "aerial"
                {
                */
                component.planetaria_rigidbody_data.velocity += component.planetaria_rigidbody_data.acceleration*(Time.fixedDeltaTime/2);
                //}
            }
        }
        
        private void aerial_move(PlanetariaRigidbodyComponent component, float delta)
        {
            //x_axis * Mathf.Cos(radians) + y_axis * Mathf.Sin(radians);
            Vector3 current_position = component.transform.position;
            Vector3 current_velocity = component.planetaria_rigidbody_data.velocity;
            float x = Mathf.Cos(delta);
            float y = Mathf.Sin(delta);
            Vector3 next_position = current_position*x + current_velocity.normalized*y; // Note: when velocity = Vector3.zero, it luckily still returns "position" intact.
            Vector3 next_velocity = current_position*y + current_velocity.normalized*x; // inverse to get derivative; you want to compute the position at (delta + PI/2) not (delta) 
            
            component.transform.position = next_position;
            component.planetaria_rigidbody_data.velocity = next_velocity.normalized * component.planetaria_rigidbody_data.velocity.magnitude; // FIXME: I thought this was numerically stable, but it seems to create more energy.
            //velocity = Vector3.ProjectOnPlane(velocity, get_position()); // TODO: CONSIDER: ensure velocity and position are orthogonal - they seem to desynchronize
            //Debug.DrawRay(get_position(), velocity, Color.green); // draw new velocity (not old one)
        }

        private Vector3 get_acceleration(PlanetariaRigidbodyComponent component)
        {
            if (component.planetaria_rigidbody_data.gravity != Vector3.zero)
            {
                return Vector3.ProjectOnPlane(component.planetaria_rigidbody_data.gravity.normalized, component.transform.position) * component.planetaria_rigidbody_data.gravity.magnitude;
            }
            return Vector3.zero;
        }

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