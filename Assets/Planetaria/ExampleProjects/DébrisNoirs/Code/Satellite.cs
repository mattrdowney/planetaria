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

        private void OnValidate()
        {
            planetaria_collider = this.GetComponent<PlanetariaCollider>();
            planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
            satellite_renderer = this.transform.Find("Chasis").GetComponent<AreaRenderer>();
            silhouette_renderer = this.transform.Find("Chasis").Find("Silhouette").GetComponent<AreaRenderer>();
            internal_transform = this.transform.Find("Chasis").gameObject.internal_game_object.GetComponent<Transform>();

            planetaria_collider.shape = PlanetariaShape.Create(transform.localScale);
            satellite_renderer.scale = transform.localScale;
            silhouette_renderer.scale = transform.localScale;
        }

        private void Start()
        {
            stopwatch = GameObject.FindObjectOfType<ScoreKeeper>();
            debris_spawner = GameObject.FindObjectOfType<DebrisSpawner>();
            loading_disc = GameObject.FindObjectOfType<Image>();

            OnFieldEnter.data = on_field_enter;
        }

        private void FixedUpdate()
        {
            if (!dead || dead_time < 0)
            {
                Vector2 input = DebrisNoirsInput.get_axes();

                // add velocity based on input
                planetaria_rigidbody.relative_velocity += input * acceleration * Time.deltaTime;
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
            else if (dead && dead_time >= 0)
            {
                planetaria_rigidbody.absolute_velocity *= Mathf.Pow(0.25f, Time.deltaTime);
                dead_time -= Time.deltaTime;
            }
        }

        private void Update()
        {
            // apply Sprite rotation (won't be seen unless visible)
            Vector2 input_direction = DebrisNoirsInput.get_direction();
            if (input_direction.sqrMagnitude > 0)
            {
                float target_angle = Mathf.Atan2(input_direction.y, input_direction.x) - Mathf.PI/2;
                float current_angle = internal_transform.localEulerAngles.z;
                float interpolator = 360 * 3 / Mathf.Abs(Mathf.DeltaAngle(current_angle, target_angle * Mathf.Rad2Deg)) * Time.deltaTime;
                internal_transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(current_angle, target_angle * Mathf.Rad2Deg, interpolator));
            }

            if (dead)
            {
                if (dead_time < 0)
                {
                    satellite_renderer.enabled = true;
                }
                respawn_time += Time.deltaTime;
                if (DebrisNoirsInput.get_axes().magnitude > 0.3f)
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
            if (!dead) // There's technically a small bug here, where the player can collide with debris that has already collided (with a projectile)
            {
                die();
                Debris debris = collider.GetComponent<Debris>();
                debris.destroy_asteroid(); // should not assign points, since the player died.
            }
        }

        private void die()
        {
            dead = true;
            dead_time = 3f;
            respawn_time = 0;
            planetaria_collider.enabled = false;
            satellite_renderer.enabled = false;
            stopwatch.stop_clock();
            debris_spawner.stop();
            satellite_renderer.sprite = ghost_sprite;
        }

        public float life()
        {
            if (!dead)
            {
                return 1;
            }
            else if (dead && dead_time >= 0)
            {
                return 0;
            }
            return 0.25f;
        }

        private void respawn()
        {
            DebrisNoirs.heat_death();
            debris_spawner.spawn();
            stopwatch.reset_clock();
            stopwatch.start_clock();
            dead = false;
            planetaria_collider.enabled = true;
            satellite_renderer.enabled = true;
            loading_disc.fillAmount = 0;
            satellite_renderer.sprite = satellite_sprite;
        }

        [SerializeField] public Sprite satellite_sprite;
        [SerializeField] public Sprite ghost_sprite;
        [SerializeField] private float acceleration = 2f;
        
        [SerializeField] private Transform internal_transform;
        [SerializeField] private PlanetariaCollider planetaria_collider;
        [SerializeField] private AreaRenderer satellite_renderer;
        [SerializeField] private AreaRenderer silhouette_renderer;
        [SerializeField] private PlanetariaRigidbody planetaria_rigidbody;

        [SerializeField] private ScoreKeeper stopwatch;
        [SerializeField] private DebrisSpawner debris_spawner;
        [SerializeField] private Image loading_disc;

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