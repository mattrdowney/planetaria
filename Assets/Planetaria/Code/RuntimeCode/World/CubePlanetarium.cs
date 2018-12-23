using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    public class CubePlanetarium : WorldPlanetarium
    {
        public CubePlanetarium(int resolution)
        {
            material = new Material(Shader.Find("Mobile/Skybox"));
            textures = new Texture2D[6];
            List<NormalizedCartesianCoordinates> positions = new List<NormalizedCartesianCoordinates>();
            for (int index = 0; index < directions.Length; ++index)
            {
                List<Vector2> uvs = get_texture_uvs(resolution);
                positions.AddRange(uvs.Select(uv => (NormalizedCartesianCoordinates) new CubeUVCoordinates(uv.x, uv.y, index)));
                Texture2D texture = new Texture2D(resolution, resolution);
                material.SetTexture(directions[index], texture);
            }
            pixel_centroids = positions.ToArray();
        }

        public override void set_pixels(Color32[] colors)
        {
            int pixels = colors.Length/directions.Length;
            int pixel_start = 0;
            Color32[] subarray = new Color32[pixels];
            for (int index = 0; index < directions.Length; ++index)
            {
                Array.Copy(colors, pixel_start, subarray, 0, pixels);
                textures[index].SetPixels32(subarray);
                pixel_start += pixels;
            }
        }

        public override Color32[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color32[] colors = new Color32[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                CubeUVCoordinates uv = positions[index];
                int scale = textures[0].width; // assumes width/height are same and six textures are equal dimensions
                colors[index] = textures[uv.texture_index].GetPixel(uv.uv.x.scale(scale), uv.uv.y.scale(scale));
            }
            return colors;
        }

#if UNITY_EDITOR
        public override void save(string file_name)
        {
            WorldPlanetarium.save_material(material, file_name); // TODO: save subasset
            for (int face = 0; face < directions.Length; ++face)
            {
                WorldPlanetarium.save_texture(textures[face], file_name, directions[face]);
            }
        }
        
        public static optional<CubePlanetarium> load(string file_name)
        {
            optional<Material> material = WorldPlanetarium.load_material(file_name);
            if (!material.exists)
            {
                return new optional<CubePlanetarium>();
            }
            Texture2D texture = WorldPlanetarium.load_texture(file_name);
            CubePlanetarium result = new CubePlanetarium(texture.width);
            result.material = material.data;
            for (int face = 0; face < directions.Length; ++face)
            {
                result.textures[face] = texture;
                result.material.SetTexture(directions[face], texture); // TODO: this step (including other files) should not be necessary in load, right?
            }
            return result;
        }
#endif

        private Texture2D[] textures;

        private static readonly string[] directions = { "_RightTex", "_LeftTex", "_UpTex", "_DownTex", "_FrontTex", "_BackTex" };
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