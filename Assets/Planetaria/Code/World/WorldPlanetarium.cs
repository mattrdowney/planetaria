using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public abstract class WorldPlanetarium
    {
        // TODO: name and filetype?
        public abstract Color32 sample_pixel(Vector3 planetarium_position);

        public Color32 sample_pixel(Vector3[] planetarium_positions)
        {
            if (planetarium_positions.Length == 1) // NOTE: optimization for common case (speeds up quick tests)
            {
                return sample_pixel(planetarium_positions[0]);
            }
            Color32Blender pixel = new Color32Blender();
            foreach (Vector3 planetarium_position in planetarium_positions)
            {
                pixel.blend(sample_pixel(planetarium_position));
            }
            return pixel;
        }

        public void render_texture(Texture2D texture, int sample_rate, PointConverter uv_converter)
        {
            Color[] pixels = new Color[texture.width*texture.height];
            float pixel_width = 1f / texture.width;
            float pixel_height = 1f / texture.height;
            for (int y = 0; y < texture.height; ++y) // set pixels from first row
            {
                float v_min = y * pixel_height;
                for (int x = 0; x < texture.width; ++x)
                {
                    float u_min = x * pixel_width;
                    Rect pixel_boundaries = new Rect(u_min, v_min, pixel_width, pixel_height);
                    Vector2[] uv_points = Miscellaneous.uv_samples(pixel_boundaries, sample_rate);
                    Vector3[] xyz_points = uv_points.Select(uv => uv_converter(uv)).ToArray();
                    Color32 color = sample_pixel(xyz_points);
                    pixels[x + y*texture.width] = color;
                }
            }
            texture.SetPixels(pixels);
        }

        protected Texture2D LoadOrCreateTexture2D(Material material, string name, string texture_identifier, int resolution) // FIXME: HACK: support more filetypes than PNG!
        {
            Texture2D texture = new Texture2D(resolution, resolution);
            if (File.Exists(Application.dataPath + "/Planetaria/Art/Textures/" + name + ".png"))
            {
                byte[] png_data = File.ReadAllBytes(Application.dataPath + "/Planetaria/Art/Textures/" + name + ".png");
                texture.LoadImage(png_data);
                if (texture.width != resolution || texture.height != resolution)
                {
                    texture.Resize(resolution, resolution);
                }
                material.SetTexture(texture_identifier, texture);
            }
            else
            {
                SaveTexture2D(texture, name);
            }
            return texture;
        }

        protected void SaveTexture2D(Texture2D texture, string name) // FIXME: HACK: support more filetypes than PNG!
        {
            byte[] png_data = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Planetaria/Art/Textures/" + name + ".png", png_data);
        }

        protected Material LoadOrCreateMaterial(string name, string shader_name)
        {
            Material material = (Material)AssetDatabase.LoadAssetAtPath("Assets/Planetaria/Art/Materials/" + name + ".mat", typeof(Material));
            if (material == null)
            {
                material = new Material(Shader.Find(shader_name));
                AssetDatabase.CreateAsset(material, "Assets/Planetaria/Art/Materials/" + name + ".mat");
            }
            return material;
        }

        public string identifier;

        public delegate Vector3 PointConverter(Vector2 uv);
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