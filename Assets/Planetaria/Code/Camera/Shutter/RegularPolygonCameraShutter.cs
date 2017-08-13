using UnityEngine;

public class RegularPolygonCameraShutter : PlanetariaCameraShutter
{
    GameObject[] shutter_edges;

    int edges;
    float rotation_adjustor;

    const float angle_to_center = 60f;

    public void initialize(int number_of_edges, float rotation_adjustment = -1f)
    {
        edges = number_of_edges;
        rotation_adjustor = rotation_adjustment;

        Camera camera = GameObject.Find("/MainCamera").GetComponent<Camera>();

        shutter_edges = new GameObject[edges];

        for (int edge_index = 0; edge_index < edges; ++edge_index)
        {
            shutter_edges[edge_index] = (GameObject) Instantiate(Resources.Load("PrimaryEdge"), new Vector3(0, 0, 0.5f), Quaternion.Euler(0, 0, edge_index*360f/edges));
            shutter_edges[edge_index].transform.parent = camera.transform;

            float x = PlanetariaMath.cone_radius(0.5f, camera.fieldOfView*Mathf.Deg2Rad);
            float y = x * camera.aspect;
            float z = 0.5f;
            
            StereoscopicProjectionCoordinates stereoscopic_projection = new NormalizedCartesianCoordinates(new Vector3(x, y, z));

            Debug.Log(stereoscopic_projection.data.x + " " + stereoscopic_projection.data.y);

            shutter_edges[edge_index].transform.localScale = Vector3.one * stereoscopic_projection.data.magnitude; // FIXME: VR FOV
            
            //shutter_edges[edge_index].transform.localScale = Vector3.one * 4 * PlanetariaMath.cone_radius(0.5f, camera.fieldOfView*Mathf.Deg2Rad) * Mathf.Sqrt(1 + (camera.aspect * camera.aspect)); // FIXME: VR FOV
        }
    }

    public override void set(float interpolation_factor)
    {
        interpolation_factor = Mathf.Clamp01(interpolation_factor);

        for (int edge_index = 0; edge_index < edges; ++edge_index)
        {
            shutter_edges[edge_index].transform.localRotation = Quaternion.Euler(0, 0, edge_index*360f/edges + interpolation_factor*angle_to_center*rotation_adjustor/2);
            shutter_edges[edge_index].transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, interpolation_factor*angle_to_center);
        }
    }
}
