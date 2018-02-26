using System;
using System.Collections;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public abstract class PlanetariaCameraShutter : MonoBehaviour
    {
        public delegate void OnShutterBlink();
        public static event OnShutterBlink blink_event;

        public float blink_wait = 6f;
        public AnimationCurve blink_down = AnimationCurve.Linear(0, 0, 0.1f, 1);
        public float blink_hold = 0.1f;
        public AnimationCurve blink_up = AnimationCurve.Linear(0, 1, 0.1f, 0);

        protected abstract void initialize();
        protected abstract void set(float interpolation_factor);
        
        private void Start()
        {
            initialize();
            StartCoroutine(blink());
        }
        
        private IEnumerator blink()
        {
            while (true)
            {
                yield return new WaitForSeconds(blink_wait);
                yield return StartCoroutine(blink(blink_down));
                set(1.5f); // overshoot 1 to avoid graphical artifacts from almost closed shutter
                yield return new WaitForSeconds(blink_hold);
                if (blink_event != null) // FIXME: ew
                {
                    blink_event();
                }
                yield return StartCoroutine(blink(blink_up));
            }
        }

        private IEnumerator blink(AnimationCurve blink_curve)
        {
            for (float blink_time = blink_curve[0].time;
                    blink_time < blink_curve[blink_curve.length - 1].time;
                    blink_time += Time.deltaTime)
            {
                set(blink_curve.Evaluate(blink_time));
                yield return null;
                Debug.Log("inner: " + Time.time);
            }
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