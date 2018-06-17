using UnityEngine;
using Planetaria;

public class RaycastUnitTests : MonoBehaviour
{
    private void Start()
    {
        GameObject camera_object = GameObject.Find("MainCamera/__CameraDolly/__LeftCamera");
        main_camera = camera_object.GetComponent<Camera>();
        player = GameObject.Find("Character").transform;
    }

    private void Update()
    {
        Vector3 screen_point = Input.mousePosition;
        Vector3 mouse_position = main_camera.ScreenPointToRay(screen_point).direction;

        Vector3 character_position = player.forward;
        PlanetariaRaycastHit[] collision_info = PlanetariaPhysics.raycast_all(Arc.line(character_position, mouse_position));
        Vector3 last_position = character_position;
        bool blue = true;
        Color color;
        foreach (PlanetariaRaycastHit hit in collision_info)
        {
            color = blue ? Color.blue : Color.red;
            Debug.DrawLine(last_position, hit.point, color);
            last_position = hit.point;
            blue = !blue;
        }
        color = blue ? Color.blue : Color.red;
        Debug.DrawLine(last_position, mouse_position, color);
    }

    private Camera main_camera;
    private Transform player;
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