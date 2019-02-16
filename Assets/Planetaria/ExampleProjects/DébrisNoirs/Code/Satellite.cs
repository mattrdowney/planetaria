using System;
using UnityEngine;
using UnityEngine.UI;
using Planetaria;

namespace DebrisNoirs
{
    public class Satellite : MonoBehaviour // HACK: normally I would use PlanetariaMonoBehaviour, but this should drastically improve collision performance
    {

        // without aliens, the dominant strategy is mostly staying in place, but eh
        private void OnValidate()
        {
            internal_collider = this.GetComponent<SphereCollider>();
            planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
            satellite_renderer = this.transform.Find("Chasis").Find("PrimaryRenderer").GetComponent<SpriteRenderer>();
            silhouette_renderer = this.transform.Find("Chasis").Find("SilhouetteRenderer").GetComponent<SpriteRenderer>();
            internal_transform = this.transform.Find("Chasis").gameObject.GetComponent<Transform>();
            planetaria_transform = this.GetComponent<PlanetariaTransform>();

            //internal_collider.shape = PlanetariaShape.Create(planetaria_transform.localScale);
            satellite_renderer.size = Vector2.one * 0.03f; // FIXME: lazy magic number
            silhouette_renderer.size = Vector2.one * 0.03f;
        }

        private void Start()
        {
            stopwatch = GameObject.FindObjectOfType<ScoreKeeper>();
            debris_spawner = GameObject.FindObjectOfType<DebrisSpawner>();
            loading_disc = GameObject.Find("LoadingDisc").GetComponent<Image>();
        }

        private void FixedUpdate()
        {
            if (!dead)
            {
                satellite_fixed_update();
            }
            else if (dead_time < 0)
            {
                ghost_fixed_update();
            }
            else if (dead && dead_time >= 0)
            {
                dead_fixed_update();
            }
        }

        private void satellite_fixed_update()
        {
            float bearing = (internal_transform.localEulerAngles.z + 90) * Mathf.Deg2Rad;
            Vector2 input = new Vector2(Mathf.Cos(bearing), Mathf.Sin(bearing)) * DebrisNoirsInput.get_accelerate();

            // add velocity based on input
            planetaria_rigidbody.relative_velocity += input * Time.fixedDeltaTime;
            Vector2 velocity = planetaria_rigidbody.relative_velocity;

            // drag in coincident direction varies from coefficient of 1->~0.8->~0.5
            float similarity = Vector2.Dot(velocity.normalized, input);
            float drag_modifier = Mathf.Lerp(0.6f, 1.0f, (similarity + 1f) / 2);
            Vector2 coincident_velocity = input != Vector2.zero ? (Vector2)Vector3.Project(velocity, input) : velocity;
            coincident_velocity *= Mathf.Pow(drag_modifier, Time.deltaTime);

            // get perpendicular velocity (unmodified)
            Vector2 perpendicular_velocity = input != Vector2.zero ? Vector3.ProjectOnPlane(velocity, input) : Vector3.zero;

            // apply velocity changes
            planetaria_rigidbody.relative_velocity = coincident_velocity + perpendicular_velocity;

            // apply unusued drag
            //planetaria_rigidbody.relative_velocity *= Mathf.Pow(0.8f, Time.deltaTime * (1f - input_direction.magnitude));
        }

        private void dead_fixed_update()
        {
            planetaria_rigidbody.absolute_velocity *= Mathf.Pow(0.25f, Time.deltaTime);
        }

        private void ghost_fixed_update()
        {
            Vector2 input = DebrisNoirsInput.get_axes(); // HACK: double counting inside function call

            // add velocity based on input
            planetaria_rigidbody.relative_velocity += input * Time.deltaTime;
            Vector2 velocity = planetaria_rigidbody.relative_velocity;

            // drag in coincident direction varies from coefficient of 1->~0.8->~0.5
            float similarity = Vector2.Dot(velocity.normalized, input);
            float drag_modifier = Mathf.Lerp(0.6f, 1.0f, (similarity + 1f) / 2);
            Vector2 coincident_velocity = input != Vector2.zero ? (Vector2)Vector3.Project(velocity, input) : velocity;
            coincident_velocity *= Mathf.Pow(drag_modifier, Time.deltaTime);

            // get perpendicular velocity (unmodified)
            Vector2 perpendicular_velocity = input != Vector2.zero ? Vector3.ProjectOnPlane(velocity, input) : Vector3.zero;

            // apply velocity changes
            planetaria_rigidbody.relative_velocity = coincident_velocity + perpendicular_velocity;

            // apply unusued drag
            //planetaria_rigidbody.relative_velocity *= Mathf.Pow(0.8f, Time.deltaTime * (1f - input_direction.magnitude));
        }

        private void satellite_update()
        {
            float rotation_input = DebrisNoirsInput.get_rotate();
            if (rotation_input != 0)
            {
                float current_angle = internal_transform.localEulerAngles.z;
                internal_transform.localEulerAngles = new Vector3(0, 0, current_angle + rotation_input*Time.deltaTime*180f);
            }
        }

        private void dead_update()
        {

        }

        private void ghost_update()
        {
            Vector2 input_direction = DebrisNoirsInput.get_axes();
            if (input_direction.sqrMagnitude > 0)
            {
                float target_angle = Mathf.Atan2(input_direction.y, input_direction.x) - Mathf.PI/2;
                float current_angle = internal_transform.localEulerAngles.z;
                float delta_angle = Mathf.Abs(Mathf.DeltaAngle(current_angle, target_angle * Mathf.Rad2Deg));
                float interpolator = 360 * 3 / delta_angle * Time.deltaTime;
                float new_angle = Mathf.LerpAngle(current_angle, target_angle * Mathf.Rad2Deg, interpolator);
                internal_transform.localEulerAngles = new Vector3(0, 0, new_angle);
            }
        }

        private void Update()
        {
            if (!dead)
            {
                satellite_update();
            }
            else if (dead_time < 0)
            {
                ghost_update();
            }
            else if (dead_time >= 0)
            {
                dead_update();
            }

            if (dead)
            {
                if (dead_time < 0)
                {
                    satellite_renderer.enabled = true;
                }
                dead_time -= Time.deltaTime;
                respawn_time += Time.deltaTime/2;
                if (DebrisNoirsInput.get_axes().magnitude > 0.3f)
                {
                    respawn_time = 0;
                    satellite_renderer.sprite = ghost_sprite;
                }
                else
                {
                    satellite_renderer.sprite = satellite_sprite; // essentially an OnMouseHover() event where the ship looks like it is alive
                }
                loading_disc.fillAmount = respawn_time;
                if (respawn_time >= 1)
                {
                    respawn();
                }
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (!dead) // There's technically a small bug here, where the player can collide with debris that has already collided (with a projectile) - wraparound shots are basically impossible, so that would be for a shot on the ~ same frame
            {
                die();
                Debris debris = collider.GetComponent<Debris>();
                debris.destroy_debris(); // should not assign points, since the player died.
            }
        }

        private void die()
        {
            dead = true;
            dead_time = 3f;
            respawn_time = 0;
            internal_collider.enabled = false;
            satellite_renderer.enabled = false;
            stopwatch.stop_clock();
            debris_spawner.stop();
            satellite_renderer.sprite = ghost_sprite;
        }

        public bool alive()
        {
            return !dead;
        }

        public Vector2 acceleration_direction()
        {
            float current_angle = internal_transform.localEulerAngles.z;
            Vector2 direction = new Vector2(Mathf.Cos(current_angle*Mathf.Deg2Rad), Mathf.Sin(current_angle*Mathf.Deg2Rad));
            if (DebrisNoirsInput.get_accelerate() == 1)
            {
                fuel += Time.deltaTime;
            }
            else
            {
                fuel -= Time.deltaTime;
            }
            fuel = Mathf.Clamp01(fuel);
            return direction * fuel;
        }

        private void respawn()
        {
            DebrisNoirs.heat_death();
            debris_spawner.spawn();
            stopwatch.start_clock();
            dead = false;
            internal_collider.enabled = true;
            satellite_renderer.enabled = true;
            loading_disc.fillAmount = 0;
            satellite_renderer.sprite = satellite_sprite;
        }

        [SerializeField] public Sprite satellite_sprite;
        [SerializeField] public Sprite ghost_sprite;
        
        [SerializeField] private Transform internal_transform;
        [SerializeField] private PlanetariaTransform planetaria_transform;
        [SerializeField] private SphereCollider internal_collider;
        [SerializeField] private SpriteRenderer satellite_renderer; // Type intentially, since I will be switching sprites often
        [SerializeField] private SpriteRenderer silhouette_renderer;
        [SerializeField] private PlanetariaRigidbody planetaria_rigidbody;

        [SerializeField] private ScoreKeeper stopwatch;
        [SerializeField] private DebrisSpawner debris_spawner;
        [SerializeField] private Image loading_disc;

        [NonSerialized] private float horizontal;
        [NonSerialized] private float vertical;

        float fuel = 0;
        [NonSerialized] private bool dead = false;
        [NonSerialized] private float dead_time = 0;
        [NonSerialized] private float respawn_time = 0;

        private const float seconds_to_max_fuel = 0.15f;
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