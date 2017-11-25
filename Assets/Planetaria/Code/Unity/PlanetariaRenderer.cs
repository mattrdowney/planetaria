using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRenderer : MonoBehaviour
    {
        public enum RenderType { Sprite, Model }

        private void Reset()
        {
            set_transformation();
            set_renderer();
            set_layer();
            set_renderer_values();
        }

        /// <summary>
        /// Property - layer is a number [-128, 127] that determines drawing order (-128 = background, 127 = foreground).
        /// Up to 256 objects can be created on the same layer without z-fighting (but z-fighting might happen with fewer if objects are destroyed at runtime).
        /// </summary>
        [SerializeField] public sbyte layer = 0;

        public optional<MeshFilter> mesh_filter;
        public float scale { get; set; } // FIXME: 
        public RenderType render_type
        {
            get
            {
                return mesh_filter.exists ? RenderType.Model : RenderType.Sprite;
            }
        }

        private void set_layer()
        {
            float scale = Miscellaneous.layer_to_distance(layer) * Octahedron.octahedron_face_distance;
            scale = Mathf.Clamp(scale, PlanetariaCamera.near_clip_plane, PlanetariaCamera.far_clip_plane);
            internal_transformation.localScale = Vector3.one*scale;
        }

        private void set_renderer()
        {
            optional<Renderer> renderer = internal_transformation.GetComponent<Renderer>();
            mesh_filter = internal_transformation.GetComponent<MeshFilter>();
            if (mesh_filter.exists)
            {
                set_renderer(renderer, typeof(MeshRenderer));
            }
            else
            {
                set_renderer(renderer, typeof(SpriteRenderer));
            }
        }

        private void set_renderer(optional<Renderer> renderer, System.Type type)
        {
            if (!renderer.exists || renderer.data.GetType() != type)
            {
                if (renderer.exists)
                {
                    DestroyImmediate(renderer.data);
                }
                internal_renderer = this.gameObject.AddComponent(type) as Renderer;
            }
        }

        private void set_renderer_values()
        {
            internal_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            internal_renderer.receiveShadows = false;
            internal_renderer.material.shader = Shader.Find("Planetaria/Transparent Lit");
        }

        private void set_transformation()
        {
            optional<Transform> transformation = this.gameObject.transform.Find("Renderer");
            if (!transformation.exists)
            {
                GameObject child = new GameObject("Renderer");
                transformation = child.GetComponent<Transform>();
            }
            internal_transformation = transformation.data;
        }

        private Transform internal_transformation;
        private Renderer internal_renderer;
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