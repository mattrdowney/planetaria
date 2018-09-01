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
    }

    private void Update()
    {
#if UNITY_EDITOR
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
#else
        horizontal = Input.GetAxis("OpenVR_ThumbAxisX");
        vertical = Input.GetAxis("OpenVR_ThumbAxisY");
#endif
        transform.direction = (NormalizedCartesianCoordinates) gameObject.internal_game_object.transform.up;
        Vector2 input_direction = new Vector2(horizontal, vertical);
        if (input_direction.sqrMagnitude > 1) // FIXME: doesn't work for unbounded input types
        {
            input_direction.Normalize();
        }

        if (input_direction.sqrMagnitude > 0)
        {
            float current_angle = planetaria_renderer.angle*Mathf.Rad2Deg;
            float target_angle = Mathf.Atan2(input_direction.y, input_direction.x)*Mathf.Rad2Deg;
            float interpolator = 360 / Mathf.Abs(Mathf.DeltaAngle(current_angle, target_angle)) * Time.deltaTime;
            planetaria_renderer.angle = Mathf.LerpAngle(current_angle, target_angle, interpolator)*Mathf.Deg2Rad;
        }

        if (input_direction.sqrMagnitude > 0)
        {
            planetaria_rigidbody.relative_velocity += input_direction * Time.deltaTime;
        }
        else
        {
            planetaria_rigidbody.relative_velocity *= Mathf.Pow(0.5f, Time.deltaTime); // FIXME: magic number
        }
    }

    private void FixedUpdate()
    {
        // Move appropriately (thrusters)
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