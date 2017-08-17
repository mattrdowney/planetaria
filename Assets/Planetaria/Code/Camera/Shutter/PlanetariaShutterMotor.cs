using UnityEngine;

[RequireComponent(typeof(PlanetariaCameraShutter))]
public class PlanetariaShutterMotor : MonoBehaviour
{
    PlanetariaCameraShutter shutter;

    float blink_duration = 0.3f;
    float blink_interval = 5f;

    void Start()
    {
        shutter = gameObject.GetComponent<PlanetariaCameraShutter>() as PlanetariaCameraShutter;
        shutter.initialize();
        shutter.set(0f);
    }

    void LateUpdate()
    {
        float interpolation_factor = Time.time % blink_interval;

        if (interpolation_factor > blink_duration)
        {
            interpolation_factor = 0f;
        }

        interpolation_factor = Mathf.PingPong(interpolation_factor, blink_duration/2)/(blink_duration/2);

        shutter.set(interpolation_factor);
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