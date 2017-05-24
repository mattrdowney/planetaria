using System.Collections.Generic;

public delegate void OnShutterBlink();

public abstract class CameraShutterStrategy : CameraStrategy
{
    private List<CameraStrategy> observers; // Use Observer pattern to allow positional/rotational tracker to be notified.

    public void notify(CameraStrategy strategy)
    {
        observers.Add(strategy);
    }
    public void drop(CameraStrategy strategy)
    {
        observers.Remove(strategy);
    }

    public sealed override void OnShutterBlink()
    {
        foreach (CameraStrategy observer in observers)
        {
            observer.OnShutterBlink(); // STEP 1: add self to list of observers, STEP 2: ???, STEP 3: Profit? // The sheer possibility of this infinite loop is making me rethink my life decisions...
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