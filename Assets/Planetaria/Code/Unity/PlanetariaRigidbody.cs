using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRigidbody : MonoBehaviour
    {
        private void Awake()
        {
            transform = this.GetOrAddComponent<PlanetariaTransform>();
            internal_rigidbody = this.GetOrAddComponent<Rigidbody>();
            internal_rigidbody.isKinematic = true;
            internal_rigidbody.useGravity = false;
        }

        private void Start()
        {
            position = transform.position.data;
            get_acceleration();
        }

        private void FixedUpdate()
        {
            if (!collision.exists)
            {
                // I am making a bet this is relevant in spherical coordinates (it is Euler isn't it?): http://openarena.ws/board/index.php?topic=5100.0
                // Wikipedia: https://en.wikipedia.org/wiki/Leapfrog_integration
                // "This is especially useful when computing orbital dynamics, as many other integration schemes, such as the (order-4) Runge-Kutta method, do not conserve energy and allow the system to drift substantially over time." - Wikipedia
                // http://lolengine.net/blog/2011/12/14/understanding-motion-in-games
                // http://www.richardlord.net/presentations/physics-for-flash-games.html
                velocity = velocity + acceleration * (Time.deltaTime / 2);
                aerial_move(velocity.magnitude * Time.deltaTime);
                velocity = velocity + acceleration * (Time.deltaTime / 2);
            }
            else
            {
                horizontal_velocity += horizontal_acceleration * (Time.deltaTime / 2);
                grounded_move();
                horizontal_velocity += horizontal_acceleration * (Time.deltaTime / 2);
            }
        }

        public Vector3 get_acceleration()
        {
            Vector3 result = Vector3.zero; // in theory this could mess things up
            for (int force = 0; force < gravity_wells.Length; ++force)
            {
                result += Bearing.attractor(position, gravity_wells[force])*gravity_wells[force].magnitude;
            }
            return result;
        }

        public void collide(BlockCollision collision, CollisionObserver observer)
        {
            this.observer = observer;
            Vector3 original_position = position;
            aerial_move(-collision.overshoot); // this only (truly) works with perpendicular vectors?
            horizontal_velocity = Vector3.Dot(this.velocity, Bearing.right(this.position, collision.normal().data));

            float vertical_velocity = Vector3.Dot(this.velocity, collision.normal().data);
            if (vertical_velocity < 0)
            {
                vertical_velocity *= -collision.elasticity;
            }
            if (vertical_velocity > collision.magnetism)
            {
                vertical_velocity = 0;
            }
            else
            {
                derail(0, vertical_velocity); // Force OnCollisionExit, "un-collision"
            }

            this.collision = collision;
        }

        private void aerial_move(float delta)
        {
            Vector3 next_position = PlanetariaMath.slerp(position, velocity.normalized, delta); // Note: when velocity = Vector3.zero, it luckily still returns "position" intact.
            Vector3 next_velocity = PlanetariaMath.slerp(position, velocity.normalized, delta + Mathf.PI/2);
            
            // set position -> velocity -> acceleration (in that order)
            transform.position = new NormalizedCartesianCoordinates(next_position);
            position = transform.position.data; // get normalized data
            velocity = next_velocity.normalized * velocity.magnitude;
            acceleration = get_acceleration();
            // TODO: occasionally ensure velocity and position are orthogonal
        }

        private void grounded_move()
        {
            collision.data.move(horizontal_velocity * Time.deltaTime, transform.scale/2);
            transform.position = collision.data.position();
            position = transform.position.data; // NOTE: required so get_acceleration() functions
            // project velocity
            acceleration = get_acceleration();

            Vector3 normal = collision.data.normal().data;
            Vector3 right = Bearing.right(collision.data.position().data, normal);

            float vertical_acceleration = Mathf.Max(0, Vector3.Dot(acceleration, normal));
            horizontal_acceleration = Vector3.Dot(acceleration, right);
            
            float friction = collision.data.friction*vertical_acceleration*Time.deltaTime;
            if (Mathf.Sign(horizontal_velocity)*horizontal_velocity > friction)
            {
                horizontal_velocity -= Mathf.Sign(horizontal_velocity)*friction;
            }
            else
            {
                horizontal_velocity = 0;
                Debug.Log("Happening");
            }
            float angle = Mathf.Acos(vertical_acceleration/acceleration.magnitude);
            if (horizontal_velocity == 0 && angle < collision.data.threshold_angle)
            {
                acceleration = Vector3.zero;
                horizontal_acceleration = 0;
            }

            if (vertical_acceleration > collision.data.magnetism)
            {
                derail(0, vertical_acceleration*Time.deltaTime); // Force OnCollisionExit, "un-collision" (and accelerate for a frame)
            }
        }

        private void grounded_accelerate(float delta)
        {

        }
        
        public void derail(float horizontal_velocity, float vertical_velocity)
        {
            if (this.collision.exists && observer.exists)
            {
                horizontal_velocity += this.horizontal_velocity;
                position = collision.data.position().data;
                Vector3 normal = collision.data.normal().data;
                Vector3 right = Bearing.right(position, normal);
                velocity = right*horizontal_velocity + normal*vertical_velocity;
                acceleration = get_acceleration();
                observer.data.exit_block(collision.data);
                this.collision = new optional<BlockCollision>();
            }
        }

        // position
        private Vector3 position; // magnitude = 1
        private Vector3 velocity; // magnitude in [0, infinity]
        private Vector3 acceleration; // magnitude in [0, infinity]

        // gravity
        [SerializeField] public Vector3[] gravity_wells;

        private new PlanetariaTransform transform;
        private optional<CollisionObserver> observer;
        private Rigidbody internal_rigidbody;
        
        private optional<BlockCollision> collision;
        private float horizontal_velocity;
        private float horizontal_acceleration;
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