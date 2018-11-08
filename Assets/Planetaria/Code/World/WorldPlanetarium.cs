using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public abstract class WorldPlanetarium // TODO: rename
    {
        public abstract void set_pixels(Color32[] colors);
        public abstract Color32[] get_pixels(NormalizedCartesianCoordinates[] positions);

        public void convert(WorldPlanetarium other)
        {
            Color32[] colors = other.get_pixels(this.pixel_centroids);
            this.set_pixels(colors);
        }

#if UNITY_EDITOR
        public abstract void save(string file_name);

        /// <summary>
        /// Serializer - Saves a material to a ".mat" file.
        /// </summary>
        /// <param name="material">The material to be saved.</param>
        /// <param name="file_name">The path where the material will be saved (relative to project folder) without file extension.</param>
        protected static void save_material(Material material, string file_name)
        {
            Debug.Log(file_name);
            AssetDatabase.CreateAsset(material, file_name + ".mat");
        }

        /// <summary>
        /// Serializer - Saves a texture to a ".png" file.
        /// </summary>
        /// <param name="texture">The Texture2D to be saved.</param>
        /// <param name="file_name">The path where the texture will be saved (relative to project folder) without file extension.</param>
        /// <param name="texture_identifier">The file suffix (e.g. "_LeftTex").</param>
        protected static void save_texture(Texture2D texture, string file_name, string texture_identifier)
        {
            byte[] png_data = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + file_name.Substring("Assets/".Length) + texture_identifier + ".png", png_data);
        }

        /// <summary>
        /// Constructor - Loads a material from a ".mat" file.
        /// </summary>
        /// <param name="file_name">The name of the file without ".mat" file extension.</param>
        /// <returns>
        /// The material that was loaded (if it exists),
        /// Nonexistent Material otherwise.
        /// </returns>
        protected static optional<Material> load_material(string file_name)
        {
            string absolute_file_name = Application.dataPath + "/" + file_name.Substring("Assets/".Length) + ".mat";
            Debug.Log(absolute_file_name);
            if (!File.Exists(absolute_file_name))
            {
                return new optional<Material>();
            }
            return (Material)AssetDatabase.LoadAssetAtPath(file_name + ".mat", typeof(Material));
        }

        /// <summary>
        /// Constructor - Loads a texture from a ".png", ".jpg", or ".exr" file.
        /// </summary>
        /// <param name="file_name">The name of the file (without suffix (e.g. "_LeftTex") or file extension (e.g. ".png")).</param>
        /// <param name="texture_identifier">The file suffix (e.g. "_LeftTex").</param>
        /// <returns>The texture that was loaded.</returns>
        protected static Texture2D load_texture(string file_name)
        {
            string absolute_file_name = Application.dataPath + "/" + file_name.Substring("Assets/".Length);
            if (File.Exists(absolute_file_name + ".png")) // TODO: support other capitalizations and variants?
            {
                file_name += ".png";
            }
            else if (File.Exists(absolute_file_name + ".jpg"))
            {
                file_name += ".jpg";
            }
            else if (File.Exists(absolute_file_name + ".exr"))
            {
                file_name += ".exr";
            }
            else
            {
                Debug.LogError("Critical texture error - File not found");
            }
            byte[] png_data = File.ReadAllBytes(file_name);
            Texture2D texture = new Texture2D(0,0);
            if (!texture.LoadImage(png_data))
            {
                Debug.LogError("Critical texture error - Load");
            }
            return texture;
        }
#endif

        public static List<Vector2> get_texture_uvs(int resolution)
        {
            List<Vector2> uvs = new List<Vector2>();
            float pixel_width = 1f / resolution;
            float pixel_height = 1f / resolution;
            float pixel_y = pixel_height/2;
            for (int y = 0; y < resolution; ++y) // set pixels from first row
            {
                float pixel_x = pixel_width / 2;
                for (int x = 0; x < resolution; ++x)
                {
                    uvs.Add(new Vector2(pixel_x, pixel_y));
                    pixel_x += pixel_width;
                }
                pixel_y += pixel_height;
            }
            return uvs;
        }

        protected Material material;
        protected NormalizedCartesianCoordinates[] pixel_centroids;
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