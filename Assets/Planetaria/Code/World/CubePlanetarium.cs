using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Planetaria
{
    public class CubePlanetarium : WorldPlanetarium
    {
        public CubePlanetarium(string name)
        {
            initialize(name, default_resolution);
        }

        public CubePlanetarium(string name, int resolution, WorldPlanetarium reference_planetarium, int sample_rate)
        {
            initialize(name, resolution);

            for (int cube_face = 0; cube_face < cube_skybox_textures.Length; ++ cube_face)
            {
                Texture2D texture = cube_skybox_textures[cube_face]; // TODO: isolate the behavior that varies: use a function that takes in a Texture2D, reference WorldPlanetarium, and delegate function
                float pixel_width = 1f/texture.width;
                float pixel_height = 1f/texture.height;
                for (int y = 0; y < texture.height; ++y) // set pixels from first row
                {
                    float v_min = y*pixel_height;
                    for (int x = 0; x < texture.width; ++x)
                    {
                        float u_min = x*pixel_width;
                        Rect pixel_boundaries = new Rect(u_min, v_min, pixel_width, pixel_height);
                        Vector2[] uv_points = Miscellaneous.uv_samples(pixel_boundaries, sample_rate);
                        Vector3[] xyz_points = uv_points.Select(uv => ((NormalizedCartesianCoordinates)new CubeUVCoordinates(uv.x, uv.y, cube_face)).data).ToArray();
                        Color32 color = reference_planetarium.sample_pixel(xyz_points);
                        texture.SetPixel(x, y, color); // TODO: optimize? with SetPixels()
                    }
                }
            }
        }

        private void initialize(string name, int resolution)
        {
            identifier = name;
            skybox = (Material)AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/" + name + ".mat", typeof(Material));
            if (skybox == null)
            {
                skybox = new Material(Shader.Find("RenderFX/Skybox"));
                AssetDatabase.CreateAsset(skybox, "Assets/Art/Materials/" + name);
            }
            cube_skybox_textures = new Texture2D[6];
            cube_skybox_textures[0] = LoadOrCreateTexture2D(name, "_LeftTex", resolution);
            cube_skybox_textures[1] = LoadOrCreateTexture2D(name, "_RightTex", resolution);
            cube_skybox_textures[2] = LoadOrCreateTexture2D(name, "_DownTex", resolution);
            cube_skybox_textures[3] = LoadOrCreateTexture2D(name, "_UpTex", resolution);
            cube_skybox_textures[4] = LoadOrCreateTexture2D(name, "_BackTex", resolution);
            cube_skybox_textures[5] = LoadOrCreateTexture2D(name, "_FrontTex", resolution);
        }

        private Texture2D LoadOrCreateTexture2D(string name, string cube_face, int resolution) // FIXME: HACK: support more filetypes than PNG!
        {
            Texture2D texture = (Texture2D) AssetDatabase.LoadAssetAtPath("Assets/Art/Textures/" + name + ".png", typeof(Texture2D));
            if (texture == null)
            {
                texture = new Texture2D(resolution, resolution);
                AssetDatabase.CreateAsset(texture, "Assets/Art/Textures/" + name);
                skybox.SetTexture(cube_face, texture);
            }
            if (texture.width != resolution)
            {
                texture.Resize(resolution, resolution);
            }
            return texture;
        }

        string identifier;
        Material skybox;
        Texture2D[] cube_skybox_textures;

        private const int default_resolution = 1024;
    }
}

