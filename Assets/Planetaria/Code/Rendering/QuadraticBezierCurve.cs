using UnityEngine;

public struct QuadraticBezierCurve
{
    public Vector2 begin_uv
    {
        get
        {
            return begin_uv_variable;
        }
    }

    public Vector2 control_uv
    {
        get
        {
            return control_uv_variable;
        }
    }

    public Vector2 end_uv
    {
        get
        {
            return end_uv_variable;
        }
    }

    public QuadraticBezierCurve(Vector2 begin_uv, Vector2 control_uv, Vector2 end_uv)
    {
        begin_uv_variable = begin_uv;
        control_uv_variable = control_uv;
        end_uv_variable = end_uv;
    }

    private Vector2 begin_uv_variable;
    private Vector2 control_uv_variable;
    private Vector2 end_uv_variable;
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