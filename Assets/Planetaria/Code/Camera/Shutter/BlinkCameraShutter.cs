using UnityEngine;

public class BlinkCameraShutter : PlanetariaCameraShutter
{
    public override void initialize()
    {
        Camera camera = GameObject.Find("/MainCamera").GetComponent<Camera>();

        shutter_edges = new GameObject[edges];

        screen_height = PlanetariaMath.cone_radius(0.5f, camera.fieldOfView*Mathf.Deg2Rad)*2;
        screen_width = screen_height*camera.aspect;

        for (int edge_index = 0; edge_index < edges; ++edge_index)
        {
            shutter_edges[edge_index] = (GameObject) Instantiate(Resources.Load("BlinkShutter"),
                    new Vector3(0, screen_height*Mathf.Cos(edge_index*Mathf.PI), 0.5f),
                    Quaternion.Euler(0, 0, edge_index*180f), camera.transform);
            shutter_edges[edge_index].transform.localScale = new Vector3(screen_width, screen_height, 1);
        }
    }

    public override void set(float interpolation_factor)
    {
        interpolation_factor = Mathf.Clamp01(interpolation_factor);

        for (int edge_index = 0; edge_index < edges; ++edge_index)
        {
            shutter_edges[edge_index].SetActive(interpolation_factor != 0);
            shutter_edges[edge_index].transform.localPosition = new Vector3(0, screen_height*(1-interpolation_factor)*Mathf.Cos(edge_index*Mathf.PI), 0.5f);
        }
    }

    private GameObject[] shutter_edges;

    private const int edges = 2;

    private float screen_height;
    private float screen_width;
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