using UnityEngine;

public static class SkyboxConversion
{
    public static Texture2D skybox_to_octahedron_uv(Skybox skybox, int output_width, int output_height, int sample_rate = 1)
    {
        Texture left = skybox.material.GetTexture("_LeftTex");
        Texture right = skybox.material.GetTexture("_RightTex");
        Texture down = skybox.material.GetTexture("_DownTex");
        Texture up = skybox.material.GetTexture("_UpTex");
        Texture back = skybox.material.GetTexture("_BackTex");
        Texture front = skybox.material.GetTexture("_FrontTex");

        Texture2D result = new Texture2D()

        for (int x = 0; x < down.width; ++x)
        {
            for (int y = 0; x < down.height; ++x)
            {

            }
        }
    }

    public static Skybox octahedron_uv_to_skybox(Texture2D texture)
    {

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