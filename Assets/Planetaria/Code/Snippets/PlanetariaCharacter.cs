using UnityEngine;
using Planetaria;

public class PlanetariaCharacter : PlanetariaMonoBehaviour
{
    void Start()
    {
        transform.scale = .1f;
        OnBlockStay.data = on_block_stay;
        OnBlockEnter.data = on_block_enter;
        OnBlockExit.data = on_block_exit;
        transform.position = new NormalizedSphericalCoordinates(Mathf.PI - .1f, Mathf.PI/2);
    }

    void Update()
    {
        if (!grounded)
        {
            //transform.position = new NormalizedSphericalCoordinates(Mathf.PI - Time.time*.5f, Mathf.PI/2);
        }
    }

    void on_block_stay(BlockCollision collision)
    {
        collision.move(0.4f*Time.deltaTime*Input.GetAxis("Horizontal"), transform.scale/2);
        transform.position = collision.position();
        transform.rotation = Bearing.angle(collision.position().data, collision.normal().data);
    }

    void on_block_enter(BlockCollision collision)
    {
        grounded = true;
    }

    void on_block_exit(BlockCollision collision)
    {
        grounded = false;
    }

    bool grounded = false;
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