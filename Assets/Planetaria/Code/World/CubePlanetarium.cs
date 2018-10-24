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
            material = new Material(Shader.Find("Skybox/Cubemap"));
            texture = new Cubemap(resolution, TextureFormat.RGBA32, false);
            material.SetTexture("_Tex", texture);
            List<NormalizedCartesianCoordinates> positions = new List<NormalizedCartesianCoordinates>();
            for (int index = 0; index < directions.Length; ++index)
            {
                List<Vector2> uvs = get_texture_uvs(texture.width);
                positions.AddRange(uvs.Select(uv => (NormalizedCartesianCoordinates) new CubeUVCoordinates(uv.x, uv.y, index)));
            }
            pixel_centroids = positions.ToArray();
        }

        public override void set_pixels(Color[] colors)
        {
            int pixels = colors.Length/directions.Length;
            int pixel_start = 0;
            Color[] subarray = new Color[pixels];
            for (int index = 0; index < directions.Length; ++index)
            {
                Array.Copy(colors, pixel_start, subarray, 0, pixels);
                texture.SetPixels(subarray, faces[index]);
                pixel_start += pixels;
            }
            texture.Apply();
        }

        public override Color[] get_pixels(NormalizedCartesianCoordinates[] positions)
        {
            Color[] colors = new Color[positions.Length];
            for (int index = 0; index < positions.Length; ++index)
            {
                CubeUVCoordinates uv = positions[index];
                colors[index] = texture.GetPixel(faces[index], uv.uv.x.scale(texture.width), uv.uv.y.scale(texture.height));
            }
            return colors;
        }

        public Cubemap get_cubemap()
        {
            return texture;
        }

#if UNITY_EDITOR
        public override void save(string file_name)
        {
            WorldPlanetarium.save_material(material, file_name); // TODO: save subasset
            WorldPlanetarium.save_cubemap(texture, file_name);
        }
        
        public static optional<CubePlanetarium> load(string file_name)
        {
            optional<Material> material = WorldPlanetarium.load_material(file_name);
            if (!material.exists)
            {
                return new optional<CubePlanetarium>();
            }
            Cubemap texture = (Cubemap) WorldPlanetarium.load_texture(file_name);
            CubePlanetarium result = new CubePlanetarium(texture.width);
            result.material = material.data;
            result.texture = texture;
            result.material.SetTexture("_Tex", texture); // TODO: this step (including other files) should not be necessary in load, right?
            return result;
        }
#endif

        private Cubemap texture;

        private static readonly string[] directions = { "_RightTex", "_LeftTex", "_UpTex", "_DownTex", "_FrontTex", "_BackTex" };
        private static readonly CubemapFace[] faces = { CubemapFace.PositiveX, CubemapFace.NegativeX,
                CubemapFace.PositiveY, CubemapFace.NegativeY, CubemapFace.PositiveZ, CubemapFace.NegativeZ };
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