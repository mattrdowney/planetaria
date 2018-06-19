using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public class SkyboxConversion : EditorWindow
    {
        public enum Shape
        {
            Cube,
            Octahedron,
            SphericalRectangle,
        }

        [MenuItem("Planetaria/Level Converter")]
        public static void convert_level_window()
        {
            EditorWindow.GetWindow(typeof(SkyboxConversion));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("PNG to convert"))
            {
                from_file_name = EditorUtility.OpenFilePanel("PNG to convert", "Assets", "png"); // TODO: multiple types
            }
            if (GUILayout.Button("Generated PNG filename"))
            {
                to_file_name = EditorUtility.OpenFilePanel("Generated PNG filename", "Assets", "png"); // TODO: multiple types and use output in conversion
            }

            GUILayout.BeginHorizontal();
            from_shape = (Shape)EditorGUILayout.EnumPopup("Current shape format", from_shape);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            to_shape = (Shape) EditorGUILayout.EnumPopup("Target shape format", to_shape);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            resolution = EditorGUILayout.IntField("Pixel resolution", resolution);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            sample_rate = EditorGUILayout.IntField("Sample rate", sample_rate);
            GUILayout.EndHorizontal();

            switch (from_shape)
            {
                case Shape.SphericalRectangle:
                    GUILayout.BeginHorizontal();
                    width = EditorGUILayout.FloatField("Angular width", width);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    height = EditorGUILayout.FloatField("Angular height", height);
                    GUILayout.EndHorizontal();
                    break;
            }

            if (GUILayout.Button("Convert"))
            {
                WorldPlanetarium from;
                WorldPlanetarium to;
                switch (from_shape)
                {
                    case Shape.Cube:
                        from = new CubePlanetarium(from_file_name);
                        break;
                    case Shape.Octahedron:
                        from = new OctahedronPlanetarium(from_file_name);
                        break;
                    case Shape.SphericalRectangle:
                    default: // FIXME:
                        from = new SphericalRectanglePlanetarium(from_file_name, width, height);
                        break;
                }
                switch (to_shape)
                {
                    case Shape.Cube:
                        to = new CubePlanetarium(to_file_name, resolution, from, sample_rate);
                        break;
                    case Shape.Octahedron:
                        to = new OctahedronPlanetarium(to_file_name, resolution, from, sample_rate);
                        break;
                }
            }
        }

        [SerializeField] [HideInInspector] private string from_file_name;
        [SerializeField] [HideInInspector] private string to_file_name;
        [SerializeField] [HideInInspector] private Shape from_shape = Shape.Octahedron;
        [SerializeField] [HideInInspector] private Shape to_shape = Shape.Cube;
        [SerializeField] [HideInInspector] private float width = 1;
        [SerializeField] [HideInInspector] private float height = 1;
        [SerializeField] [HideInInspector] private int resolution = 256;
        [SerializeField] [HideInInspector] private int sample_rate = 1;
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