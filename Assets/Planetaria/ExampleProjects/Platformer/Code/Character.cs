using System;
using UnityEngine;
using Planetaria;

public class Character : PlanetariaMonoBehaviour
{
    protected override void OnConstruction()
    {
        OnBlockStay.data = on_block_stay;
        OnBlockEnter.data = on_block_enter;
        OnBlockExit.data = on_block_exit;
        OnFieldStay.data = on_field_stay;
    }

    protected override void OnDestruction() { }

    private void Start()
    {
        planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
        this.GetComponent<PlanetariaCollider>().material = material;
        transform.direction = new NormalizedCartesianCoordinates(Vector3.up);
        transform.localScale = +0.01f;
    }

    private void Update()
    {
#if UNITY_EDITOR
        jump_pressed = Input.GetButtonDown("Jump");
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
#else
        jump_pressed = Input.GetButtonDown("OSVR_ThumbPress");
        horizontal = Input.GetAxis("OSVR_ThumbAxisX");
        vertical = Input.GetAxis("OSVR_ThumbAxisY");
#endif

        if (jump_pressed)
        {
            last_jump_attempt = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (!planetaria_rigidbody.colliding)
        {
            planetaria_rigidbody.absolute_velocity += Vector2.right * horizontal * Time.deltaTime * transform.scale * acceleration;
        }
    }

    private void on_block_stay(BlockCollision collision)
    {
        if (planetaria_rigidbody.colliding) // FIXME: GitHub issue #67
        {
            if (collision.magnetism != 0)
            {
                magnet_floor = true;
            }
            float velocity = planetaria_rigidbody.relative_velocity.x;

            velocity += horizontal * -planetaria_rigidbody.relative_velocity.y * transform.scale * acceleration * 20f; // Time.deltaTime omitted intentionally (included in relative_velocity.y)

            if (Mathf.Abs(velocity) > 3f*transform.scale)
            {
                velocity = Mathf.Sign(velocity)*3f*transform.scale;
            }
            planetaria_rigidbody.relative_velocity = new Vector2(velocity, 0);
            transform.direction = (NormalizedCartesianCoordinates) collision.geometry_visitor.normal();
            if (Time.time - last_jump_attempt < .2f)
            {
                planetaria_rigidbody.derail(0, 3*transform.scale);
            }
        }
        else
        {
            Debug.LogError("And yet this is happening?");
            planetaria_rigidbody.derail(0,1);
        }
    }

    private void on_block_enter(BlockCollision collision)
    {
    }

    private void on_block_exit(BlockCollision collision)
    {
        last_jump_attempt = -1;
        transform.direction = new NormalizedCartesianCoordinates(Vector3.up);
        magnet_floor = false;
    }

    private void on_field_stay(PlanetariaCollider collider)
    {
        if (vertical < -.8f)
        {
            LevelLoader.loader.activate_level(1);
        }
    }

    [SerializeField] public PlanetariaPhysicMaterial material;
    [SerializeField] private const float acceleration = 5f;

    [NonSerialized] private PlanetariaRigidbody planetaria_rigidbody;
    [NonSerialized] private float last_jump_attempt = -1;
    [NonSerialized] public bool magnet_floor = false;
    [NonSerialized] private bool jump_pressed;
    [NonSerialized] private float horizontal;
    [NonSerialized] private float vertical;
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