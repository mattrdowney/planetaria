using UnityEngine;

public delegate void OnShutterBlink();

public abstract class PlanetariaCameraShutter : MonoBehaviour
{
    public enum ShutterPosition { Closing, Closed, Opening };

    public abstract ShutterPosition Blink();
    public abstract ShutterPosition Open();
    public abstract ShutterPosition Close();

    ShutterPosition shutter_position;

    void LateUpdate()
    {
        switch (shutter_position)
        {
            case ShutterPosition.Closing:
                shutter_position = Close();
                break;
            case ShutterPosition.Closed:
                shutter_position = Blink();
                break;
            case ShutterPosition.Opening:
                shutter_position = Open();
                break;
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