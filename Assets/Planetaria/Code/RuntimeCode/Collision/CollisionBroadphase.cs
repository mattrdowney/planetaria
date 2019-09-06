using UnityEngine;

public class CollisionBroadphase : MonoBehaviour
{
    void Update()
    {
        // Ideally, there should be some space partitioning for dynamic objects and a reasonable solution for dynamic objects.
        // Collision layers should be considered. Worlds/Levels could also be considered.
        // (For both objects) If a callback exists, call it.
        // Hopefully there's early return logic in a few places.
        // Subscription/Observer pattern
        // Blah, blah, blah
        // I might just be lazy and do an InsertionSort based on k partitioning dimensions. The obvious one being k=3 and x,y,z.
        // There can be a lot of optimizations here.
        // Essentially you do the following:
        //    Take old sorted list with marked nodes where they changed.
        //    Scan left to right, if a marked node can be trivially reinserted into the sequence unmark it (unless it was marked for deletion)
        //    Take the marked and unmarked sequences as two new lists
        //    Sort the unmarked sequence
        //    Merge the lists

        // While this may seem like a good idea, you still have to do more work to detect collisions
        // Oh hey, essentially I've been describing a worse version of https://en.wikipedia.org/wiki/Sweep_and_prune
        // If you apply the optimizations of "sweep and prune" you can make the performance a lot better.

        // Bounding cone hierarchy is definitely useful for static geometry
        // Dynamic geometry:
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.