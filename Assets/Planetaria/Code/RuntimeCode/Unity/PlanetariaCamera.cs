using System;
using UnityEngine;
using UnityEngine.XR;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public sealed class PlanetariaCamera : PlanetariaComponent // TODO: unseal because of proprietary plugins (e.g. Oculus, Steam, Google, etc)
    {
        protected override sealed void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override void OnDestroy() { }

        protected override sealed void Reset()
        {
            base.Reset();
            initialize();
        }

        private void initialize()
        {
            if (internal_camera == null)
            {
                GameObject dolly = this.GetOrAddChild("CameraDolly");
                dolly_transform = dolly.GetComponent<Transform>();
                Miscellaneous.GetOrAddComponent<AudioListener>(dolly);
                GameObject camera_object = dolly.transform.GetOrAddChild("Camera");
                internal_camera = Miscellaneous.GetOrAddComponent<Camera>(camera_object);
            }
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            dolly_transform.localPosition = Vector3.forward * zoom;
            dolly_transform.localScale = Vector3.zero; // sets interpupillary  distance (IPD) to zero so there is no eye distance and therefore no depth // TODO: I thought there was something wrong with this
            initialize_camera(internal_camera, new Rect(0, 0, 1, 1), 1);
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
                camera.stereoSeparation = 0;
                camera.stereoConvergence = 0.01f;
            }
        }

        public float zoom
        {
            get
            {
                return zoom_variable;
            }
            set
            {
                zoom_variable = value;
                dolly_transform.localPosition = Vector3.forward * value;
            }
        }

        public const float near_clip_plane = 0.0078125f;
        public const float far_clip_plane = 2.0f;
        
        [SerializeField] private float zoom_variable = 0;
        [SerializeField] [HideInInspector] private Transform dolly_transform;
        [SerializeField] [HideInInspector] private Camera internal_camera;
    }
}

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.