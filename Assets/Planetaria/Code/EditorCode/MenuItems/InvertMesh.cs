#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Planetaria
{
    public class InvertMesh : EditorWindow
    {
        [MenuItem("Planetaria/Invert Mesh")]
        public static void convert_level_window()
        {
            EditorWindow.GetWindow(typeof(InvertMesh));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Mesh to invert"))
            {
                from_file_name = EditorUtility.OpenFilePanel("Mesh to invert", "Assets/Planetaria", "FBX"); // FIXME: multiple extensions work
                //from_file_name = from_file_name.Substring(0, from_file_name.LastIndexOf('.'));
                Debug.Log(from_file_name);
                // this is an editor tool, so the following is fine:
                int clip_index = from_file_name.IndexOf("Assets/");
                from_file_name = from_file_name.Substring(clip_index);
            }

            if (GUILayout.Button("Save location of inverted mesh"))
            {
                to_file_name = EditorUtility.SaveFilePanel("Save location of inverted mesh", "Assets/Planetaria", "output", "asset"); // TODO: multiple types
                to_file_name = to_file_name.Substring(0, to_file_name.LastIndexOf('.'));
                // this is an editor tool, so the following is fine:
                int clip_index = to_file_name.IndexOf("Assets/");
                to_file_name = to_file_name.Substring(clip_index);
            }

            if (GUILayout.Button("Generate"))
            {
                Mesh original_mesh = (Mesh) AssetDatabase.LoadAssetAtPath(from_file_name, typeof(Mesh));
                Mesh inverted_mesh = OutsideInMesh.generate(original_mesh);
                AssetDatabase.CreateAsset(inverted_mesh, to_file_name + ".asset");
                AssetDatabase.SaveAssets();
            }
        }

        [SerializeField] [HideInInspector] private string from_file_name;
        [SerializeField] [HideInInspector] private string to_file_name;
    }
}
#endif

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