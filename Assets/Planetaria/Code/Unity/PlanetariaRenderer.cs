using UnityEngine;

namespace Planetaria
{
    public class PlanetariaRenderer : MonoBehaviour
    {
        public enum RenderType { Sprite, Model }

        private void Reset()
        {
            optional<Transform> transformation = this.gameObject.transform.Find("Renderer");
            if (!transformation.exists)
            {
                GameObject child = new GameObject("Renderer");
                transformation = child.GetComponent<Transform>();
            }
            internal_transformation = transformation.data;
            
            optional<Renderer> renderer = internal_transformation.GetComponent<Renderer>();
            mesh_filter = internal_transformation.GetComponent<MeshFilter>();
            if (mesh_filter.exists)
            {
                if (!renderer.exists || renderer.data.GetType() != typeof(MeshRenderer))
                {
                    if (renderer.exists)
                    {
                        DestroyImmediate(renderer.data);
                    }
                    internal_renderer = this.GetOrAddComponent<MeshRenderer>();
                }
            }
            else
            {
                if (!renderer.exists || renderer.data.GetType() != typeof(SpriteRenderer))
                {
                    if (renderer.exists)
                    {
                        DestroyImmediate(renderer.data);
                    }
                    internal_renderer = this.GetOrAddComponent<SpriteRenderer>();
                }
            }
            internal_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            internal_renderer.receiveShadows = false;
            internal_renderer.material.shader = Shader.Find("Planetaria/Transparent Lit");
        }

        public short layer // there are two choice: leave layer alone (z-fighting will happen) or do a startup prepass to determine unique layers (but runtime will still have issues)
        {
            get
            {
                return layer_variable;
            }
            set
            {
                layer_variable = value;
                float scale = Miscellaneous.layer_to_distance(value);
                internal_transformation.localScale = Vector3.one*scale;
            }
        }

        public optional<MeshFilter> mesh_filter;
        public float scale { get; set; } // FIXME: 
        public RenderType render_type
        {
            get
            {
                return mesh_filter.exists ? RenderType.Model : RenderType.Sprite;
            }
        }

        private Transform internal_transformation;
        private Renderer internal_renderer;
        private short layer_variable;
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