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
            resolution = EditorGUILayout.IntField("Pixel resolution", resolution);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Generate"))
            {
                switch (shape)
                {
                    case Shape.Cube:
                        break;
                    case Shape.Octahedron:
                        SubdividedOctahedronSphere tesselated_octahedron = SubdividedOctahedronSphere.generate(resolution);
                        AssetDatabase.CreateAsset(tesselated_octahedron.get_shared_mesh(),
                                "Assets/Planetaria/Procedural/Mesh/octahedron_" + resolution + ".asset");
                        break;
                }
                AssetDatabase.SaveAssets();
            }
        }

        [SerializeField] [HideInInspector] private Shape shape = Shape.Octahedron;
        [SerializeField] [HideInInspector] private int resolution = 1;
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