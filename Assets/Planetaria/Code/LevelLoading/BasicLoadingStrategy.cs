using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BasicLoadingStrategy : LoadingStrategy
{
    /// <summary>
    /// Inspector - Return 100% loaded, since the basic loader loads all levels in Awake.
    /// </summary>
    /// <param name="level_index">The index of the level that would have been loaded. (Should match Unity level index.)</param>
    /// <returns>1, meaning 100% loaded.</returns>
    public override float fraction_loaded(int level_index)
    {
        return 1;
    }

    /// <summary>
    /// Inspector - Since all levels are loaded, do nothing.
    /// </summary>
    /// <param name="level_index">The index of the level that would have been loaded. (Should match Unity level index.)</param>
    public override void request_level(int level_index)
    {
    }

    /// <summary>
    /// Mutator - Switch level geometry and graphics.
    /// </summary>
    /// <param name="level_index">The index of the level that will be focused in. (Should match Unity level index.)</param>
    public override void focus_level(int level_index)
    {
        planetaria_map[current_level_index].unload_room();
        planetaria_map[level_index].load_room();
        current_level_index = level_index;
    }

    private void Awake()
    {
        current_level_index = 0;
        planetaria_map = new Dictionary<int, Planetarium>();

        for (int level_index = 0; level_index < SceneManager.sceneCount; ++level_index) // FIXME: This should NOT load Menu Screen and other GUI levels
        {
            SceneManager.LoadScene(level_index, LoadSceneMode.Additive);
            planetaria_map[level_index] = Planetarium.planetarium(level_index);
        }
    }

    private int current_level_index;
    private Dictionary<int, Planetarium> planetaria_map;
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