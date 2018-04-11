using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoaderUtility
{
    public static void load(int level_index, Vector3 center)
    {
        AsyncOperation thread;
        if (!SceneManager.GetSceneByBuildIndex(level_index).isLoaded) // this is to avoid double loading first level
        {
            levels[level_index] = SceneManager.LoadSceneAsync(level_index, LoadSceneMode.Additive);
        }
                        
        int current_level = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(level_index + " " + current_level);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(level_index));
        GameObject[] spawned_objects = SceneManager.GetSceneByBuildIndex(level_index).GetRootGameObjects();
        foreach (GameObject game_object in spawned_objects)
        {
            game_object.transform.position = center;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(current_level));
    }

    public static void unload(int level_index)
    {
        if (levels.ContainsKey(level_index) && levels[level_index].isDone)
        {
            SceneManager.UnloadSceneAsync(level_index); // TODO: is this safe? - if not, add to a map<int, AsyncOperation>
        }
    }

    public static void enable(int level_index)
    {
        set_active(level_index, true);
    }

    public static void disable(int level_index)
    {
        set_active(level_index, false);
    }

    private static void set_active(int level_index, bool active)
    {
        if (SceneManager.GetSceneByBuildIndex(level_index).isLoaded)
        {
            GameObject[] spawned_objects = SceneManager.GetSceneByBuildIndex(level_index).GetRootGameObjects();
            foreach (GameObject game_object in spawned_objects)
            {
                game_object.SetActive(active);
            }
        }
    }

    private static Dictionary<int, AsyncOperation> levels = new Dictionary<int, AsyncOperation>();
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