using UnityEngine;
using Planetaria;
using System;

public class Elevator : PlanetariaTracker
{
    public override void cleanup() { }

    public override void setup()
    {
        start_position = self.position.data;
        NormalizedSphericalCoordinates spherical = self.position;
        NormalizedCartesianCoordinates cartesian = new NormalizedSphericalCoordinates(spherical.data.x - Mathf.PI*(2f/3f), spherical.data.y);
        end_position = cartesian.data;
    }

    public override void step()
    {
        float interpolation_fraction = Mathf.PingPong(Time.time/10f, 1); // FIXME: AnimationCurve (repeat) with optional hook for buttons
        Debug.Log(interpolation_fraction);
        Vector3 intermediate_position = Vector3.Slerp(start_position, end_position, interpolation_fraction); // FIXME: needs to work >=180 degrees
        self.position = new NormalizedCartesianCoordinates(intermediate_position);
        
        Debug.DrawLine(start_position, intermediate_position, Color.white);
        Debug.DrawLine(intermediate_position, end_position, Color.black);
    }

    public override void teleport() { }

    private Vector3 start_position;
    private Vector3 end_position;
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