using UnityEngine;

namespace Planetaria
{
    public class CubePlanetarium : WorldPlanetarium
    {
        public CubePlanetarium(string name)
        {
            initialize(name);
        }

        public CubePlanetarium(string name, int resolution, WorldPlanetarium reference_planetarium, int sample_rate)
        {
            initialize(name, resolution);

            for (int index = 0; index < textures.Length; ++index)
            {
                Texture2D texture = textures[index];
                reference_planetarium.render_texture(texture, sample_rate, // isolate the behavior that varies: use a function that takes in a Texture2D and delegate function
                        delegate (Vector2 uv) { return ((NormalizedCartesianCoordinates)new CubeUVCoordinates(uv.x, uv.y, index)).data; });
                SaveTexture2D(texture, name + directions[index]);
            }
        }

        public override Color32 sample_pixel(Vector3 planetarium_position)
        {
            NormalizedCartesianCoordinates position = new NormalizedCartesianCoordinates(planetarium_position);
            CubeUVCoordinates uv = position;
            Texture2D texture = textures[uv.texture_index];
            Debug.Log(uv.texture_index + " " + uv.x(100) + " " + uv.y(100));
            Debug.Log(uv.texture_index + " " + uv.x(texture.width) + " " + uv.y(texture.height));
            Debug.Log(texture.GetPixel(uv.x(texture.width), uv.y(texture.height)));
            return texture.GetPixel(uv.x(texture.width), uv.y(texture.height));
        }

        private void initialize(string name, optional<int> resolution = new optional<int>())
        {
            identifier = name;
            material = LoadOrCreateMaterial(name, "RenderFX/Skybox");
            textures = new Texture2D[directions.Length];
            for (int index = 0; index < directions.Length; ++index)
            {
                textures[index] = LoadOrCreateTexture2D(material, name + directions[index], directions[index], resolution);
            }
        }
        
        private Texture2D[] textures;

        private static readonly string[] directions = { "_LeftTex", "_RightTex", "_DownTex", "_UpTex", "_BackTex", "_FrontTex" };
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