using UnityEngine;
using UnityEditor;

public class Unsave : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        MeshRenderer[] mesh_renderers = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach (MeshRenderer mesh_renderer in mesh_renderers)
        {
            GameObject.DestroyImmediate(mesh_renderer.gameObject);
        }
        SpriteRenderer[] sprite_renderers = GameObject.FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer sprite_renderer in sprite_renderers)
        {
            GameObject.DestroyImmediate(sprite_renderer.gameObject);
        }
        return paths;
    }

    //static void destroy_all_of_type(System.Type type) // TODO: figure out how to do this // C# extensions?
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