using UnityEngine;

public abstract class LoadingStrategy : Component
{
    /// <summary>
    /// Inspector - Gets the progress on the level loading.
    /// </summary>
    /// <param name="level_index">The index of the level that will be loaded. (Should match Unity level index.)</param>
    /// <returns>A value [0, 1] inclusive, where 0 means 0% loaded and 1 means 100% loaded.</returns>
    public abstract float fraction_loaded(int level_index);

    /// <summary>
    /// Mutator - Starts loading the level (and--if necessary--unloads other levels).
    /// </summary>
    /// <param name="level_index">The index of the level that will be loaded. (Should match Unity level index.)</param>
    public abstract void request_level(int level_index);

    /// <summary>
    /// Mutator - Will instantaneously switch level geometry and transition out the graphics for level gradually.
    /// </summary>
    /// <param name="level_index">The index of the level that will be focused, implicitly unfocusing previous level. (Should match Unity level index.)</param>
    public abstract void focus_level(int level_index);

    // A footnote regarding graphics loading: until this point I planned on doing a "camera shutter blink to load in/out levels, but I will probably implement two versions:
    // 1) camera shutter blink (for casual players).
    // 2) hexagonal shutter blink (for people that hate themselves).
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