using System;
using UnityEngine;
using Planetaria;

public class Debris : PlanetariaMonoBehaviour
{
    protected override void OnConstruction() { }

    private void Start()
    {
        planetaria_collider = this.GetComponent<PlanetariaCollider>();
        planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
        planetaria_transform = this.GetComponent<PlanetariaTransform>();
        planetaria_renderer = this.GetComponent<AreaRenderer>();
        transform.localScale = +0.1f;
        planetaria_renderer.angle = UnityEngine.Random.Range(0, 2*Mathf.PI);
        
        // set sprite image
        planetaria_renderer.sprite = debris_sprites[(int)stage];

        // set collider size
        planetaria_collider.scale = debris_sizes[(int)stage];

        // apply random rotation
        float deviation_angle = UnityEngine.Random.Range(-maximum_deviation_angles[(int)stage], +maximum_deviation_angles[(int)stage]);
        Vector2 local_direction = new Vector2(Mathf.Sin(deviation_angle), Mathf.Cos(deviation_angle));
        Vector3 direction = this.gameObject.internal_game_object.transform.rotation * local_direction;
        planetaria_transform.direction = new NormalizedCartesianCoordinates(direction);

        // apply random speed multiplier
        float speed_multiplier = UnityEngine.Random.Range(1.2f, 2f);
        speed *= speed_multiplier;

        // set velocity
        planetaria_rigidbody.relative_velocity = new Vector2(0, speed);

        OnFieldEnter.data = on_field_enter;
    }

    protected override void OnDestruction() { }
    
    public void on_field_enter(PlanetariaCollider collider)
    {
        PlanetariaGameObject.Destroy(collider.gameObject);
        if (this.stage != SpaceRockSize.Small && collider.gameObject)
        {
            for (int space_rock = 0; space_rock < 2; ++space_rock)
            {
                PlanetariaGameObject game_object = PlanetariaGameObject.Instantiate(prefabricated_debris, planetaria_transform.position.data, planetaria_transform.direction.data);
                Debris debris = game_object.GetComponent<Debris>();
                debris.speed = this.speed;
                debris.stage = this.stage + 1;
            }
        }
        PlanetariaGameObject.Destroy(this.gameObject);
    }

    public enum SpaceRockSize { Large = 0, Medium = 1, Small = 2 };

    [SerializeField] private float speed = 0.5f;
    [SerializeField] private SpaceRockSize stage = SpaceRockSize.Large;

    [NonSerialized] private AreaRenderer planetaria_renderer;
    [NonSerialized] private PlanetariaCollider planetaria_collider;
    [NonSerialized] private PlanetariaRigidbody planetaria_rigidbody;
    [NonSerialized] private PlanetariaTransform planetaria_transform;

    [SerializeField] public /*static*/ GameObject prefabricated_debris;
    [SerializeField] public /*static*/ Sprite[] debris_sprites;
    [SerializeField] public /*static*/ float[] maximum_deviation_angles;
    [SerializeField] public /*static*/ float[] debris_sizes;
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