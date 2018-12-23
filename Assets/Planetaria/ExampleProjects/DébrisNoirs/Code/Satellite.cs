using System;
using UnityEngine;
using UnityEngine.UI;
using Planetaria;

namespace DebrisNoirs
{
    public class Satellite : PlanetariaMonoBehaviour
    {
        protected override void OnConstruction() { }

        protected override void OnDestruction() { }

        private void Start()
        {
            main_character = GameObject.FindObjectOfType<Satellite>().gameObject.internal_game_object.transform;
            main_controller = GameObject.FindObjectOfType<PlanetariaActuator>().gameObject.internal_game_object.transform;
            planetaria_collider = this.GetComponent<PlanetariaCollider>();
            planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
            ship_renderer = this.GetComponent<AreaRenderer>();
            crosshair_renderer = PlanetariaGameObject.Find("Hand").GetComponent<AreaRenderer>();
            stopwatch = GameObject.FindObjectOfType<DebrisNoirsStopwatch>();
            debris_spawner = GameObject.FindObjectOfType<DebrisSpawner>();
            loading_disc = GameObject.FindObjectOfType<Image>();

            OnFieldEnter.data = on_field_enter;
        }

        private void Update()
        {
            if (!dead || dead_time < 0)
            {
                Vector2 input_direction = DebrisNoirsInput.movement();
                crosshair_renderer.scale = input_direction.magnitude * crosshair_size;
                if (input_direction.magnitude > 0)
                {
                    float target_angle = Mathf.Atan2(input_direction.y, input_direction.x) - Mathf.PI / 2;
                    float current_angle = ship_renderer.angle * Mathf.Rad2Deg;
                    float interpolator = 360 * 3 / Mathf.Abs(Mathf.DeltaAngle(current_angle, target_angle * Mathf.Rad2Deg)) * Time.deltaTime;
                    ship_renderer.angle = Mathf.LerpAngle(current_angle, target_angle * Mathf.Rad2Deg, interpolator) * Mathf.Deg2Rad;
                }

                // TODO: verify (pretty likely to have at least one error) // Bunch of errors, not elegant
                // Design specs:
                // Aim forward along velocity: no drag (i.e. drag coefficient of 1) and accelerate.
                // Aim backwards against velocity: full drag (e.g. drag coefficient of .5) and "decelerate"
                // No input: partial drag (e.g. drag coefficient of .75) and "decelerate"
                // Aim perpendicular to velocity (left/right): partial drag (e.g. coefficient of .75) but take a percentage of the momentum that would be lost and apply it along input_direction

                // add velocity based on input
                planetaria_rigidbody.relative_velocity += input_direction * acceleration * Time.deltaTime;
                Vector2 velocity = planetaria_rigidbody.relative_velocity;

                // drag in coincident direction varies from coefficient of 1->~0.8->~0.5
                float similarity = Vector2.Dot(velocity.normalized, input_direction);
                float drag_modifier = Mathf.Lerp(0.6f, 1.0f, (similarity + 1f) / 2);
                Vector2 coincident_velocity = input_direction != Vector2.zero ? (Vector2)Vector3.Project(velocity, input_direction) : velocity;
                coincident_velocity *= Mathf.Pow(drag_modifier, Time.deltaTime);

                // get perpendicular velocity (unmodified)
                Vector2 perpendicular_velocity = input_direction != Vector2.zero ? Vector3.ProjectOnPlane(velocity, input_direction) : Vector3.zero;

                // apply velocity changes
                planetaria_rigidbody.relative_velocity = coincident_velocity + perpendicular_velocity;

                // apply unusued drag
                //planetaria_rigidbody.relative_velocity *= Mathf.Pow(0.8f, Time.deltaTime * (1f - input_direction.magnitude));
            }
            if (dead)
            {
                if (dead_time < 0)
                {
                    ship_renderer.enabled = true;
                    ship_renderer.color = Color.grey;
                }
                else
                {
                    planetaria_rigidbody.absolute_velocity *= Mathf.Pow(0.25f, Time.deltaTime);
                    dead_time -= Time.deltaTime;
                }
                respawn_time += Time.deltaTime;
                if (DebrisNoirsInput.movement().magnitude > 0.3f)
                {
                    respawn_time = 0;
                }
                loading_disc.fillAmount = respawn_time;
                if (respawn_time >= 1)
                {
                    respawn();
                }
            }
        }

        public void on_field_enter(PlanetariaCollider collider)
        {
            Debug.Log("Satellite Colliding"); // Why isn't this being called? w/e being lazy...
            die();
        }

        private void die()
        {
            dead = true;
            dead_time = 3f;
            respawn_time = 0;
            planetaria_collider.enabled = false;
            ship_renderer.enabled = false;
            stopwatch.stop_clock();
        }

        public bool is_dead()
        {
            return dead;
        }

        private void respawn()
        {
            DebrisNoirs.heat_death();
            debris_spawner.heat_death();
            stopwatch.reset_clock();
            stopwatch.start_clock();
            dead = false;
            planetaria_collider.enabled = true;
            ship_renderer.enabled = true;
            ship_renderer.color = Color.white;
            loading_disc.fillAmount = 0;
        }

        [SerializeField] private float crosshair_size = 1f;
        [SerializeField] private float acceleration = 2f;

        [NonSerialized] private Transform main_character;
        [NonSerialized] private Transform main_controller;
        [NonSerialized] private PlanetariaCollider planetaria_collider;
        [NonSerialized] private AreaRenderer crosshair_renderer;
        [NonSerialized] private AreaRenderer ship_renderer;
        [NonSerialized] private PlanetariaRigidbody planetaria_rigidbody;
        [NonSerialized] private DebrisNoirsStopwatch stopwatch;
        [NonSerialized] private DebrisSpawner debris_spawner;
        [NonSerialized] private Image loading_disc;
        [NonSerialized] private float horizontal;
        [NonSerialized] private float vertical;

        [NonSerialized] private bool dead = false;
        [NonSerialized] private float dead_time = 0;
        [NonSerialized] private float respawn_time = 0;
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