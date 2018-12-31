using UnityEngine;
using Planetaria;

namespace DebrisNoirs
{
    /// <summary>
    /// Debris are the space rocks / asteroids flying around on screen. The three types (small, medium, large) can be created with different variables.
    /// </summary>
    public class Debris : PlanetariaMonoBehaviour
    {
        protected override void OnConstruction() { }
        protected override void OnDestruction() { }

        private void OnValidate()
        {
            planetaria_collider = this.GetComponent<PlanetariaCollider>();
            planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
            planetaria_transform = this.GetComponent<PlanetariaTransform>();
            planetaria_renderer_foreground = this.GetComponent<AreaRenderer>();
            planetaria_renderer_background = planetaria_transform.Find("Silhouette").GetComponent<AreaRenderer>();

            planetaria_collider.shape = PlanetariaShape.Create(planetaria_transform.localScale);
            planetaria_renderer_foreground.scale = planetaria_transform.localScale;
            planetaria_renderer_background.scale = planetaria_transform.localScale;
        }

        private void Start()
        {
            planetaria_renderer_foreground.angle = UnityEngine.Random.Range(0, 2*Mathf.PI); // this is random rotation for the sprite image.
            planetaria_renderer_background.angle = planetaria_renderer_foreground.angle; // and the silhouette

            // apply random rotation
            float random_deviation = UnityEngine.Random.Range(-deviation_angle, +deviation_angle);
            Vector2 local_direction = new Vector2(Mathf.Sin(random_deviation), Mathf.Cos(random_deviation));
            Vector3 direction = this.gameObject.internal_game_object.transform.rotation * local_direction;
            planetaria_transform.direction = direction;

            // apply random speed multiplier
            float speed_multiplier = UnityEngine.Random.Range(1, speed_deviation_multiplier);
            speed *= speed_multiplier;

            // set velocity
            planetaria_rigidbody.relative_velocity = new Vector2(0, speed);

            OnFieldEnter.data = on_field_enter;
        }

        public void on_field_enter(PlanetariaCollider collider)
        {
            // Add to player score
            if (collider.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                score_keeper.add(points);
            }
            destroy_asteroid();
        }

        public void destroy_asteroid()
        {
            if (spawned_debris != null)
            {
                for (int space_rock = 0; space_rock < 2; ++space_rock)
                {
                    PlanetariaGameObject game_object = PlanetariaGameObject.Instantiate(spawned_debris, planetaria_transform.position, planetaria_transform.direction);
                    Debris debris = game_object.GetComponent<Debris>();
                    debris.speed = this.speed;
                    DebrisNoirs.live(game_object);
                }
            }
            DebrisNoirs.die(this.gameObject);
            PlanetariaGameObject.Destroy(this.gameObject);
        }

        
        [SerializeField] public ScoreKeeper score_keeper;
        [SerializeField] [HideInInspector] private AreaRenderer planetaria_renderer_foreground;
        [SerializeField] [HideInInspector] private AreaRenderer planetaria_renderer_background;
        [SerializeField] [HideInInspector] private PlanetariaCollider planetaria_collider;
        [SerializeField] [HideInInspector] private PlanetariaRigidbody planetaria_rigidbody;
        [SerializeField] [HideInInspector] private PlanetariaTransform planetaria_transform;

        [SerializeField] public /*static*/ GameObject spawned_debris;

        [SerializeField] private float speed = 1;
        [SerializeField] private float deviation_angle = Mathf.PI;
        [SerializeField] private float speed_deviation_multiplier = 1.25f;
        [SerializeField] private int points = 10;
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