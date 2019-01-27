#if UNITY_EDITOR

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
            SphericalCircle,
            UnityCubemap,
        }

        [MenuItem("Planetaria/Level Converter")]
        public static void convert_level_window()
        {
            EditorWindow.GetWindow(typeof(SkyboxConversion));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Image(s) to convert"))
            {
                from_file_name = EditorUtility.OpenFilePanel("PNG to convert", "Assets/Planetaria/ExampleProjects/DébrisNoirs/Art/Textures", "mat"); // TODO: multiple types
                from_file_name = from_file_name.Substring(0, from_file_name.Length-4);
                // this is an editor tool, so the following is fine:
                int clip_index = from_file_name.IndexOf("Assets/");
                from_file_name = from_file_name.Substring(clip_index);
            }
            if (GUILayout.Button("Generated PNG(s) filename"))
            {
                to_file_name = EditorUtility.SaveFilePanel("Generated PNG filename", "Assets/Planetaria/ExampleProjects/DébrisNoirs/Art/Textures", "output_file", "png"); // TODO: multiple types and use output in conversion // FIXME: HACK: trying to make deadlines
                to_file_name = to_file_name.Substring(0, to_file_name.Length-4);
                // this is an editor tool, so the following is fine:
                int clip_index = to_file_name.IndexOf("Assets/");
                to_file_name = to_file_name.Substring(clip_index);
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
                    canvas = EditorGUILayout.RectField("Spherical Rectangle", canvas);
                    GUILayout.EndHorizontal();
                    break;
                case Shape.SphericalCircle:
                    GUILayout.BeginHorizontal();
                    radius = EditorGUILayout.FloatField("Spherical Circle radius", radius);
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
                        optional<CubePlanetarium> cubemap = CubePlanetarium.load(from_file_name);
                        Debug.Assert(cubemap.exists);
                        from = cubemap.data;
                        break;
                    case Shape.Octahedron:
                        optional<OctahedronPlanetarium> octahedron = OctahedronPlanetarium.load(from_file_name);
                        Debug.Assert(octahedron.exists);
                        from = octahedron.data;
                        break;
                    case Shape.SphericalRectangle:
                        optional<SphericalRectanglePlanetarium> rectangle = SphericalRectanglePlanetarium.load(from_file_name, canvas);
                        Debug.Assert(rectangle.exists);
                        from = rectangle.data;
                        break;
                    case Shape.SphericalCircle:
                    default: // FIXME:
                        optional<SphericalCirclePlanetarium> circle = SphericalCirclePlanetarium.load(from_file_name, radius);
                        Debug.Assert(circle.exists);
                        from = circle.data;
                        break;
                }
                switch (to_shape)
                {
                    case Shape.Cube:
                        to = new CubePlanetarium(resolution);
                        to.convert(from);
                        to.save(to_file_name);
                        break;
                    case Shape.Octahedron:
                        to = new OctahedronPlanetarium(resolution);
                        to.convert(from);
                        to.save(to_file_name);
                        break;
                }
            }
        }

        [SerializeField] [HideInInspector] private string from_file_name;
        [SerializeField] [HideInInspector] private string to_file_name = "Assets/Planetaria/ExampleProjects/DébrisNoirs/Art/Textures/output_file";
        [SerializeField] [HideInInspector] private Shape from_shape = Shape.SphericalRectangle;
        [SerializeField] [HideInInspector] private Shape to_shape = Shape.Octahedron;
        [SerializeField] [HideInInspector] private Rect canvas = new Rect(-Mathf.PI/4, -Mathf.PI/4, Mathf.PI/2, Mathf.PI/2);
        [SerializeField] [HideInInspector] private float radius = Mathf.PI/2;
        [SerializeField] [HideInInspector] private int resolution = 128;
        [SerializeField] [HideInInspector] private int sample_rate = 1;
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
