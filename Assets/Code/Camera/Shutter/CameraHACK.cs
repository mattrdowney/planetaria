using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHACK : MonoBehaviour
{
    public GameObject prefabricated_object;

    GameObject[] shutter_edges;

    void Start()
    {
        Camera camera = GameObject.Find("/MainCamera").GetComponent<Camera>();

        shutter_edges = new GameObject[6];

        for (int edge_index = 0; edge_index < 6; ++edge_index)
        {
            shutter_edges[edge_index] = Instantiate(prefabricated_object, new Vector3(0, 0, 0.5f), Quaternion.Euler(0, 0, edge_index*60f));
            shutter_edges[edge_index].transform.localScale = Vector3.one * 4 * PlanetariaMath.cone_radius(0.5f, camera.fieldOfView*Mathf.Deg2Rad) * Mathf.Sqrt(1 + (camera.aspect * camera.aspect));
        }
    }

    void Update()
    {
        for (int edge_index = 0; edge_index < 6; ++edge_index)
        {
            shutter_edges[edge_index].transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, Mathf.PingPong(10*Time.time, 60f));
        }
    }
}