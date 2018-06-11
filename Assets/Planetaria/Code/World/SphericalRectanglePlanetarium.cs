using UnityEngine;

namespace Planetaria
{
    public class SphericalRectanglePlanetarium : WorldPlanetarium
    {
        public SphericalRectanglePlanetarium(string name, float angular_width, float angular_height)
        {
            initialize(name);
            this.angular_width = angular_width;
            this.angular_height = angular_height;
        }

        public override Color sample_pixel(Vector3 planetarium_position)
        {
            NormalizedCartesianCoordinates cartesian = new NormalizedCartesianCoordinates(planetarium_position);
            SphericalRectangleUVCoordinates spherical_rectangle = cartesian.to_spherical_rectangle(angular_width, angular_height);
            if (!spherical_rectangle.valid())
            {
                return Color.clear;
            }
            return texture.GetPixel(spherical_rectangle.uv.x.scale(texture.width), spherical_rectangle.uv.y.scale(texture.height));
        }

        private void initialize(string name, optional<int> resolution = new optional<int>())
        {
            identifier = name;
            material = LoadOrCreateMaterial(name, "Planetaria/Transparent Always");
            texture = LoadOrCreateTexture2D(material, name, "_MainTex", resolution);
        }

        private Texture2D texture;
        private float angular_width;
        private float angular_height;
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
