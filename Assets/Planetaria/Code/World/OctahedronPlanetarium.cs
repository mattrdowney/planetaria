using UnityEngine;

namespace Planetaria
{
    public class OctahedronPlanetarium : WorldPlanetarium
    {
        public OctahedronPlanetarium(string name)
        {
            initialize(name);
        }

        public OctahedronPlanetarium(string name, int resolution, WorldPlanetarium reference_planetarium, int sample_rate)
        {
            initialize(name, resolution);

            // isolate the behavior that varies: use a function that takes in a Texture2D and delegate function
            reference_planetarium.render_texture(texture, sample_rate,
                    delegate (Vector2 uv) { return ((NormalizedCartesianCoordinates)new OctahedronUVCoordinates(uv.x, uv.y)).data; });
            SaveTexture2D(texture, name);
        }

        public override Color32 sample_pixel(Vector3 planetarium_position)
        {
            NormalizedCartesianCoordinates position = new NormalizedCartesianCoordinates(planetarium_position);
            OctahedronUVCoordinates uv = position;
            return texture.GetPixel(uv.x(texture.width), uv.y(texture.height));
        }

        private void initialize(string name, optional<int> resolution = new optional<int>())
        {
            identifier = name;
            material = LoadOrCreateMaterial(name, "Planetaria/Transparent Always");
            texture = LoadOrCreateTexture2D(material, name, "_MainTex", resolution);
        }
        
        private Texture2D texture;
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