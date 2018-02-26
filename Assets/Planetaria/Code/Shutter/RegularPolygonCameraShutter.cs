using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public class RegularPolygonCameraShutter : PlanetariaCameraShutter
    {
        protected override void initialize()
        {
            Camera camera = this.GetComponentInChildren<Camera>() as Camera;

            shutter_edges = new GameObject[edges];

            for (int edge_index = 0; edge_index < edges; ++edge_index)
            {
                shutter_edges[edge_index] = (GameObject) Instantiate(Resources.Load("PrimaryEdge"),
                        new Vector3(0, 0, PlanetariaCamera.near_clip_plane),
                        Quaternion.Euler(0, 0, edge_index*360f/edges), camera.transform);

                float x = PlanetariaMath.cone_radius(PlanetariaCamera.near_clip_plane, camera.fieldOfView*Mathf.Deg2Rad);
                float y = x * camera.aspect;
                float z = PlanetariaCamera.near_clip_plane;
            
                StereoscopicProjectionCoordinates stereoscopic_projection = new NormalizedCartesianCoordinates(new Vector3(x, y, z));

                shutter_edges[edge_index].transform.localScale = Vector3.one * stereoscopic_projection.data.magnitude; // FIXME: VR FOV
            
                //shutter_edges[edge_index].transform.localScale = Vector3.one * 4 * PlanetariaMath.cone_radius(0.5f, camera.fieldOfView*Mathf.Deg2Rad) * Mathf.Sqrt(1 + (camera.aspect * camera.aspect)); // FIXME: VR FOV
            }
        }

        protected override void set(float interpolation_factor)
        {
            interpolation_factor = Mathf.Clamp(interpolation_factor, 0, 1.5f);

            for (int edge_index = 0; edge_index < edges; ++edge_index)
            {
                shutter_edges[edge_index].SetActive(interpolation_factor != 0);
                shutter_edges[edge_index].transform.localRotation = Quaternion.Euler(0, 0, edge_index*360f/edges + interpolation_factor*angle_to_center*rotation_adjustor/2);
                shutter_edges[edge_index].transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, interpolation_factor*angle_to_center);
            }
        }

        public int edges;
        public float rotation_adjustor;

        /// <summary>Reference to transparent cutout-textured quadrilateral planes that create camera shutter.</summary>
        private GameObject[] shutter_edges;

        /// <summary>The angle two semicircles must each turn to intersect at their old center.</summary>
        private const float angle_to_center = 60f;
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