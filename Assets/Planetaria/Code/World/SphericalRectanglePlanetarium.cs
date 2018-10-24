using UnityEngine;

namespace Planetaria
{
    public class SphericalRectanglePlanetarium : WorldPlanetarium
    {
        public override void set_pixels(Color[] colors)
        {
            texture.SetPixels(colors);
        }

        public override Color[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color[] colors = new Color[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                SphericalRectangleUVCoordinates spherical_rectangle = positions[index].to_spherical_rectangle(angular_width, angular_height);
                if (!spherical_rectangle.valid())
                {
                    colors[index] = Color.clear;
                }
                else
                {
                    colors[index] = texture.GetPixel(spherical_rectangle.uv.x.scale(texture.width), spherical_rectangle.uv.y.scale(texture.height));
                }
            }
            return colors;
        }

#if UNITY_EDITOR
        public override void save(string file_name)
        {
            WorldPlanetarium.save_material(material, file_name); // TODO: save subasset
            WorldPlanetarium.save_texture(texture, file_name, "_MainTex");
        }
        
        public static optional<SphericalRectanglePlanetarium> load(string file_name, float width, float height)
        {
            optional<Material> material = WorldPlanetarium.load_material(file_name);
            if (!material.exists)
            {
                return new optional<SphericalRectanglePlanetarium>();
            }
            SphericalRectanglePlanetarium result = new SphericalRectanglePlanetarium();
            result.material = material.data;
            result.texture = (Texture2D) WorldPlanetarium.load_texture(file_name);
            result.material.SetTexture("_MainTex", result.texture);
            result.angular_width = width;
            result.angular_height = height;
            return result;
        }
#endif

        private void initialize(float width, float height, int resolution = 0)
        {
            angular_width = width;
            angular_height = height;
            material = new Material(Shader.Find("Planetaria/Transparent Always"));
            texture = new Texture2D(resolution, resolution);
            material.SetTexture("_MainTex", texture);
        }

        private Texture2D texture;
        private float angular_width;
        private float angular_height;
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