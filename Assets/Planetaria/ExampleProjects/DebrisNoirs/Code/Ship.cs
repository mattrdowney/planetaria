using System;
using UnityEngine;
using Planetaria;

public class Ship : PlanetariaMonoBehaviour
{
    protected override void OnConstruction()
    {
        OnFieldStay.data = on_field_stay;
    }

    protected override void OnDestruction() { }

    private void Start()
    {
        planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
        planetaria_renderer = this.GetComponent<AreaRenderer>();
        transform.direction = new NormalizedCartesianCoordinates(Vector3.up);
        transform.localScale = +0.1f;
        last_position = transform.position.data;
    }

    private void Update()
    {
#if UNITY_EDITOR
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
#else
        horizontal = Input.GetAxis("OSVR_ThumbAxisX");
        vertical = Input.GetAxis("OSVR_ThumbAxisY");
#endif
        Vector2 input_direction = new Vector2(horizontal, vertical);
        if (input_direction.sqrMagnitude > 1) // FIXME: doesn't work for unbounded input types
        {
            input_direction.Normalize();
        }

        if (input_direction.sqrMagnitude > 0)
        {
            float current_angle = planetaria_renderer.angle*Mathf.Rad2Deg;
            float target_angle = Mathf.Atan2(input_direction.y, input_direction.x)*Mathf.Rad2Deg;
            float interpolator = 360*5 / Mathf.Abs(Mathf.DeltaAngle(current_angle, target_angle)) * Time.deltaTime;
            planetaria_renderer.angle = Mathf.LerpAngle(current_angle, target_angle, interpolator)*Mathf.Deg2Rad;
        }
        
        // TODO: verify (pretty likely to have at least one error)

        // add velocity based on input
        planetaria_rigidbody.relative_velocity += input_direction * Time.deltaTime;

        // get supplementary information for drag calculations
        Vector2 perpendicular_direction = new Vector2(-input_direction.y, input_direction.x);
        Vector2 velocity = planetaria_rigidbody.relative_velocity;

        // drag on velocity perpendicular to acceleration
        Vector2 perpendicular_velocity = velocity * Vector2.Dot(perpendicular_direction, velocity.normalized);
        perpendicular_velocity *= Mathf.Pow(0.8f, Time.deltaTime);

        // drag in coincident direction varies from coefficient of 1->~0.8->~0.5
        float similarity = Vector2.Dot(velocity.normalized, input_direction);
        float drag_modifier = Mathf.Lerp(0.6f, 1.0f, (similarity + 1f) / 2);
        Vector2 coincident_velocity = velocity * Vector2.Dot(input_direction, velocity.normalized);
        coincident_velocity *= Mathf.Pow(drag_modifier, Time.deltaTime);

        // apply uniform drag for any unused movement speed
        planetaria_rigidbody.relative_velocity *= Mathf.Pow(0.8f, Time.deltaTime * (1f - input_direction.magnitude));

        last_position = transform.position.data;
    }

    private void on_field_stay(PlanetariaCollider collider)
    {
        // Destroy asteroid and self
    }
    
    [SerializeField] private const float acceleration = 5f;

    [NonSerialized] private AreaRenderer planetaria_renderer;
    [NonSerialized] private PlanetariaRigidbody planetaria_rigidbody;
    [NonSerialized] private float horizontal;
    [NonSerialized] private float vertical;
    [NonSerialized] private Vector3 last_position;
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