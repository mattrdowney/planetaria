using UnityEngine;

public struct Discontinuity
{
    public Arc arc
    {
        get
        {
            return arc_variable;
        }
    }

    public Vector3 position
    {
        get
        {
            return position_variable;
        }
    }

    public float angle
    {
        get
        {
            return angle_variable;
        }
    }

    public Discontinuity(Arc arc, NormalizedCartesianCoordinates position)
    {
        arc_variable = arc;
        position_variable = position.data;
        angle_variable = arc.position_to_angle(position_variable);

        if (angle_variable < 0 || angle_variable > arc.angle())
        {
            Debug.Log("Weird angle: " + angle_variable + "/" + arc.angle());
        }
    }

    private Arc arc_variable;
    private Vector3 position_variable;
    private float angle_variable;
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