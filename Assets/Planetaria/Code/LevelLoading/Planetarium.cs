using UnityEngine;

public class Planetarium
{
    int level_index;

    public Planetarium(int level_index_)
    {
        level_index = level_index_;
    }

    public void Load_room()
    {
        Toggle_room(level_index, true);
    }

    public void Unload_room()
    {
        Toggle_room(level_index, false);
    }

    private static void Toggle_room(int level_index, bool state)
    {
        GameObject geometry_root = GameObject.Find("/" + level_index.ToString()); // TODO: make this more elegant...
        GameObject graphics_root = GameObject.Find("/" + level_index.ToString() + "g");

        // geometry
        for (int child_id = 0; child_id < geometry_root.transform.childCount; child_id++)
        {
            geometry_root.transform.GetChild(child_id).gameObject.SetActive(state);
        }

        // graphics
        for (int child_id = 0; child_id < graphics_root.transform.childCount; child_id++)
        {
            graphics_root.transform.GetChild(child_id).gameObject.SetActive(state);
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