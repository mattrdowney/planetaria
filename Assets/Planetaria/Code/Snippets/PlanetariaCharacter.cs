using UnityEngine;

public class PlanetariaCharacter : PlanetariaActor
{
    protected override void set_delegates()
    {
        on_first_exists.data = initialize;
        on_every_frame.data = main;
        on_block_enter.data = collide;
    }

    void initialize()
    {
        transform.scale = 0.1f;
    }

    void main()
    {
        transform.position = new NormalizedSphericalCoordinates(Mathf.PI - Time.time*.4f, Mathf.PI/2 + .01f);
    }

    void main2()
    {
        current_collision.data.move(0.4f*Time.deltaTime, transform.scale/2);
        transform.position = current_collision.data.position();
    }

    void collide(BlockCollision collision)
    {
        on_every_frame.data = main2;
    }
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