#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Planetaria
{
    public class ProceduralMesh : EditorWindow
    {
        public enum Shape // FIXME: move to its own class
        {
            Cube,
            Octahedron,
            SphericalRectangle,
        }

        [MenuItem("Planetaria/Procedural Mesh")]
        public static void convert_level_window()
        {
            EditorWindow.GetWindow(typeof(ProceduralMesh));
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            shape = (Shape)EditorGUILayout.EnumPopup("Shape", shape);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            triangle_budget = EditorGUILayout.IntField("Triangle Budget", triangle_budget);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate"))
            {
                switch (shape)
                {
                    case Shape.Cube:
                        break;
                    case Shape.Octahedron:
                        Mesh tesselated_octahedron = OctahedronSphere.generate(triangle_budget);
                        int triangles = tesselated_octahedron.triangles.Length/3;
                        AssetDatabase.CreateAsset(tesselated_octahedron,
                                "Assets/Planetaria/Procedural/Mesh/octahedron_" + triangles + ".asset");
                        break;
                }
                AssetDatabase.SaveAssets();
            }
        }

        [SerializeField] [HideInInspector] private Shape shape = Shape.Octahedron;
        [SerializeField] [HideInInspector] private int triangle_budget = 1;
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