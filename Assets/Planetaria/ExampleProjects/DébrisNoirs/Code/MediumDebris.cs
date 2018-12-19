using System;
using UnityEngine;
using Planetaria;

public class MediumDebris : PlanetariaMonoBehaviour
{
    protected override void OnConstruction() { }
    protected override void OnDestruction() { }
    
    private void OnValidate()
    {
        planetaria_collider = this.GetComponent<PlanetariaCollider>();
        planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
        planetaria_transform = this.GetComponent<PlanetariaTransform>();
        planetaria_renderer = this.GetComponent<AreaRenderer>();
    }

    private void Start()
    {
        planetaria_renderer.angle = UnityEngine.Random.Range(0, 2*Mathf.PI);

        // apply random rotation
        float deviation_angle = UnityEngine.Random.Range(-Mathf.PI, +Mathf.PI);
        Vector2 local_direction = new Vector2(Mathf.Sin(deviation_angle), Mathf.Cos(deviation_angle));
        Vector3 direction = this.gameObject.internal_game_object.transform.rotation * local_direction;
        planetaria_transform.direction = direction;

        // apply random speed multiplier
        float speed_multiplier = UnityEngine.Random.Range(1, 1.25f);
        speed *= speed_multiplier;

        // set velocity
        planetaria_rigidbody.relative_velocity = new Vector2(0, speed);

        OnFieldEnter.data = on_field_enter;
    }
    
    public void on_field_enter(PlanetariaCollider collider)
    {
        // if collision is detected with asteroid, get other asteroid's dot_product threshold (starts at -1 and progresses to +1 in ~1 second after spawning to prevent "double collisions") and average it with this asteroid's dot product threshold
        // if the (lossy) dot product of the velocities is less than the average threshold, then collide
        destroy_asteroid();
    }

    public void destroy_asteroid()
    {
        for (int space_rock = 0; space_rock < 2; ++space_rock)
        {
            PlanetariaGameObject game_object = PlanetariaGameObject.Instantiate(small_debris, planetaria_transform.position, planetaria_transform.direction);
            SmallDebris debris = game_object.GetComponent<SmallDebris>();
            debris.speed = this.speed;
        }
        PlanetariaGameObject.Destroy(this.gameObject);
    }

    [NonSerialized] private AreaRenderer planetaria_renderer;
    [NonSerialized] private PlanetariaCollider planetaria_collider;
    [NonSerialized] private PlanetariaRigidbody planetaria_rigidbody;
    [NonSerialized] private PlanetariaTransform planetaria_transform;

    [SerializeField] public /*static*/ GameObject small_debris;

    [SerializeField] private float speed = 1;
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