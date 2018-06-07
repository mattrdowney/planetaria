using System;
using UnityEngine;
using UnityEngine.XR;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public class PlanetariaCamera : MonoBehaviour
    {
        private void Awake()
        {
            initialize();
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
        }

        private void Reset()
        {
            initialize();
        }

        private void initialize()
        {
            if (transform == null)
            {
                transform = this.GetOrAddComponent<PlanetariaTransform>();
                GameObject dolly = this.GetOrAddChild("CameraDolly");
                dolly_transform = dolly.GetComponent<Transform>();
                dolly.transform.GetOrAddComponent<AudioListener>();
                GameObject left_camera = dolly.transform.GetOrAddChild("LeftCamera");
                internal_left_camera = left_camera.transform.GetOrAddComponent<Camera>();
            }
            dolly_transform.position = Vector3.forward * zoom;
            dolly_transform.localScale = Vector3.one; // CONSIDER: setting this to zero mirrors `XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);`
            if (two_cameras)
            {
                GameObject right_camera = dolly_transform.GetOrAddChild("RightCamera");
                internal_right_camera = right_camera.transform.GetOrAddComponent<Camera>();
                initialize_camera(internal_left_camera, new Rect(0.0f, 0, 0.5f, 1), 1);
                initialize_camera(internal_right_camera, new Rect(0.5f, 0, 0.5f, 1), 0); // depth order hides bugs, but is generally good for end-users
            }
            else
            {
                DestroyImmediate(internal_right_camera.data.gameObject); // FIXME: Find gameobject?
                internal_right_camera = new optional<Camera>();
                initialize_camera(internal_left_camera, new Rect(0, 0, 1, 1), 1);
            }
        }

        private void initialize_camera(optional<Camera> internal_camera, Rect screen, int draw_order)
        {
            if (internal_camera.exists)
            {
                Camera camera = internal_camera.data;
                camera.rect = screen;
                camera.depth = draw_order;
                camera.useOcclusionCulling = false;
                camera.nearClipPlane = near_clip_plane;
                camera.farClipPlane = far_clip_plane;
            }
        }

        public const float near_clip_plane = 0.0078125f;
        public const float far_clip_plane = 2.0f;

        [SerializeField] public bool two_cameras = false;
        [SerializeField] public float zoom = 0;
        [SerializeField] [HideInInspector] protected new PlanetariaTransform transform;
        [SerializeField] [HideInInspector] protected Transform dolly_transform;
        [SerializeField] [HideInInspector] protected Camera internal_left_camera;
        [SerializeField] [HideInInspector] protected optional<Camera> internal_right_camera;
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