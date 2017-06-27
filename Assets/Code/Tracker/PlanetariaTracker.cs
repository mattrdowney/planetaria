using System.Collections.Generic;

public class PlanetariaTracker
{
    // Ironically, the tracker should be notifying the camera (I had it wrong, conceptually).
    private List<Observer> observers; // Use Observer pattern to allow objects to be notified of Transform changes.
    protected PlanetariaLookingStrategy looking_strategy;
    protected PlanetariaTiltingStrategy tilting_strategy;

    public void on_teleport()
    {
        foreach (PlanetariaTrackingStrategy observer in observers)
        {
            observer.on_update();
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