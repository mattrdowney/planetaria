using UnityEngine;
using Planetaria;

public class PlanetariaCharacter : PlanetariaMonoBehaviour
{
    protected override void OnConstruction()
    {
        planetaria_rigidbody = this.GetComponent<PlanetariaRigidbody>();
        transform.scale = .1f;
        OnBlockStay.data = on_block_stay;
        OnBlockEnter.data = on_block_enter;
        OnBlockExit.data = on_block_exit;

        OnFieldStay.data = on_field_stay;

        transform.position = new NormalizedSphericalCoordinates(Mathf.PI/2 + 0.06f, Mathf.PI/2 + .01f);
    }

    protected override void OnDestruction()
    {
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            last_jump_attempt = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (!planetaria_rigidbody.colliding)
        {
            planetaria_rigidbody.absolute_velocity += Vector2.right * Input.GetAxis("Horizontal") * Time.deltaTime * transform.scale * acceleration * 1f;
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
            velocity += Input.GetAxis("Horizontal") * -planetaria_rigidbody.relative_velocity.y * transform.scale * acceleration * 20f;
            if (Mathf.Abs(velocity) > 3f*transform.scale)
            {
                velocity = Mathf.Sign(velocity)*3f*transform.scale;
            }
            planetaria_rigidbody.relative_velocity = new Vector2(velocity, 0);
            transform.direction = collision.normal();
            if (Time.time - last_jump_attempt < .2f)
            {
                planetaria_rigidbody.derail(0, 4*transform.scale);
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
        Debug.Log("Inside " + Time.time);
        if (Input.GetAxis("Vertical") == -1)
        {
            Debug.Log("Pressed " + Time.time);
            LevelLoader.loader.activate_level(1);
        }
    }

    private PlanetariaRigidbody planetaria_rigidbody;
    private float last_jump_attempt = -1;
    private const float acceleration = 5f;

    public bool magnet_floor = false;
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