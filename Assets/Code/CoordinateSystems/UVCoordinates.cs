using UnityEngine;

public class UVCoordinates
{
	public Vector2 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    public UVCoordinates(float u, float v)
    {
        data_ = new Vector2(u, v);
        Normalize(); 
    }

    // implicit conversions: implement to_Cartesian and use it to make the other two functions!

    Vector2 data_;

    void Normalize()
    {
        if (Mathf.Abs(data_.x - 0.5f) > 0.5f)
        {
            data_.x = Mathf.PingPong(data_.x, 1f); //TODO: test that 1) Vector2 is properly assigned 2) PingPong works for negative numbers
        }
        if (Mathf.Abs(data_.y - 0.5f) > 0.5f)
        {
            data_.y = Mathf.PingPong(data_.y, 1f);
        }
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