using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public static class ToggleHiddenInspector
    {
        [MenuItem("Planetaria/Toggle Hidden Inspector")]
        private static void toggle_hidden_inspector()
        {
            EditorGlobal.self.show_inspector = !EditorGlobal.self.show_inspector; // toggle state
            foreach (GameObject game_object in GameObject.FindObjectsOfType<GameObject>())
            {
                // Hide planetaria internals (GitHub Issue #43 and #75).
                // Toggling the inspector shows these objects
                if (game_object.name.Substring(0, 2) == "__") // double underscore indicates hidden object
                {
                    if (!EditorGlobal.self.show_inspector)
                    {
                        game_object.hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector); // set
                    }
                    else
                    {
                        game_object.hideFlags = HideFlags.None; // unset
                    }
                }
                EditorUtility.SetDirty(game_object);
            }
        }
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