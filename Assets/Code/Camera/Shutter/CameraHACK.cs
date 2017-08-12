using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHACK : MonoBehaviour
{
    public GameObject prefabricated_object;

    GameObject[] shutter_edges;

    public int edges = 2;

    public float angle_to_center = 60f;
    public float angular_speed = -1f;

    void Start()
    {
        Camera camera = GameObject.Find("/MainCamera").GetComponent<Camera>();

        shutter_edges = new GameObject[edges];

        for (int edge_index = 0; edge_index < edges; ++edge_index)
        {
            shutter_edges[edge_index] = Instantiate(prefabricated_object, new Vector3(0, 0, 0.5f), Quaternion.Euler(0, 0, edge_index*360f/edges));
            shutter_edges[edge_index].transform.localScale = Vector3.one * 4 * PlanetariaMath.cone_radius(0.5f, camera.fieldOfView*Mathf.Deg2Rad) * Mathf.Sqrt(1 + (camera.aspect * camera.aspect)); // FIXME: VR FOV
        }
    }

    void Update()
    {
        for (int edge_index = 0; edge_index < edges; ++edge_index)
        {
            shutter_edges[edge_index].transform.localRotation = Quaternion.Euler(0, 0, edge_index*360f/edges + Mathf.PingPong(angle_to_center*Time.time*5, angle_to_center)*angular_speed/2);
            shutter_edges[edge_index].transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, Mathf.PingPong(angle_to_center*Time.time*5, angle_to_center));
        }
    }
}