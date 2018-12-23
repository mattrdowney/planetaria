using UnityEngine;

namespace Planetaria
{
    public class SphericalCirclePlanetarium : WorldPlanetarium
    {
        public override void set_pixels(Color32[] colors)
        {
            texture.SetPixels32(colors);
        }

        public override Color32[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color32[] colors = new Color32[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                PolarCoordinates polar = positions[index];
                SphericalCircleQVCoordinates spherical_circle = polar.to_spherical_circle(radius_variable);
                if (!spherical_circle.valid())
                {
                    colors[index] = new Color32(0,0,0,0); // Color.clear
                }
                else
                {
                    Vector2 texture_indices = spherical_circle.to_texture_coordinate(texture.width);
                    colors[index] = texture.GetPixel((int)texture_indices.x, (int)texture_indices.y);
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
        
        public static optional<SphericalCirclePlanetarium> load(string file_name, float radius)
        {
            optional<Material> material = WorldPlanetarium.load_material(file_name);
            if (!material.exists)
            {
                return new optional<SphericalCirclePlanetarium>();
            }
            SphericalCirclePlanetarium result = new SphericalCirclePlanetarium();
            result.material = material.data;
            result.texture = (Texture2D) WorldPlanetarium.load_texture(file_name);
            result.material.SetTexture("_MainTex", result.texture);
            result.radius_variable = radius;
            return result;
        }
#endif

        private void initialize(float radius, int resolution = 0)
        {
            radius_variable = radius;
            material = new Material(Shader.Find("Planetaria/Transparent Always"));
            texture = new Texture2D(resolution, resolution);
            material.SetTexture("_MainTex", texture);
        }

        private Texture2D texture;
        private float radius_variable;
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