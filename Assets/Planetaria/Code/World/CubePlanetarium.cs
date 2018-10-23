using UnityEngine;

namespace Planetaria
{
    public class CubePlanetarium : WorldPlanetarium
    {
        public CubePlanetarium()
        {
            initialize();
        }

        public CubePlanetarium(int resolution, WorldPlanetarium reference_planetarium, int sample_rate)
        {
            initialize(resolution);

            for (int index = 0; index < textures.Length; ++index)
            {
                Texture2D texture = textures[index];
                reference_planetarium.render_texture(texture, sample_rate, // isolate the behavior that varies: use a function that takes in a Texture2D and delegate function
                        delegate (Vector2 uv) { return ((NormalizedCartesianCoordinates)new CubeUVCoordinates(uv.x, uv.y, index)).data; });
            }
        }

        public override Color sample_pixel(Vector3 planetarium_position)
        {
            NormalizedCartesianCoordinates position = new NormalizedCartesianCoordinates(planetarium_position);
            CubeUVCoordinates uv = position;
            Texture2D texture = textures[uv.texture_index];
            return texture.GetPixel(uv.uv.x.scale(texture.width), uv.uv.y.scale(texture.height));
        }

        public Cubemap to_cubemap()
        {
            Cubemap result = new Cubemap(textures[0].width, TextureFormat.RGBA32, false);
            for (int face = 0; face < faces.Length; ++face)
            {
                result.SetPixels(textures[face].GetPixels(), faces[face]);
            }
            result.Apply();
            return result;
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
            CubePlanetarium result = new CubePlanetarium();
            result.material = material.data;
            for (int index = 0; index < directions.Length; ++index)
            {
                result.textures[index] = WorldPlanetarium.load_texture(file_name, directions[index]);
                result.material.SetTexture(directions[index], result.textures[index]);
            }
            return result;
        }
#endif

        private void initialize(int resolution = 0)
        {
            material = new Material(Shader.Find("RenderFX/Skybox"));
            textures = new Texture2D[6];
            for (int index = 0; index < directions.Length; ++index)
            {
                textures[index] = new Texture2D(resolution, resolution);
                material.SetTexture(directions[index], textures[index]);
            }
        }
        
        private Texture2D[] textures;

        private static readonly string[] directions = { "_LeftTex", "_RightTex", "_DownTex", "_UpTex", "_BackTex", "_FrontTex" };
        private static readonly CubemapFace[] faces = { CubemapFace.NegativeX, CubemapFace.PositiveX,
                CubemapFace.NegativeY, CubemapFace.PositiveY, CubemapFace.NegativeZ, CubemapFace.PositiveZ };
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