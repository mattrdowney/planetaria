﻿using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public abstract class PlanetariaRenderer : MonoBehaviour
    {
        //private void Reset() //AddComponent might be smarter than the Reset() editor strat' even though it incurs some runtime penalty.

        protected abstract void set_renderer();

        protected void set_layer()
        {
            if (!drawing_order.exists)
            {
                drawing_order = next_available_order(sorting_layer.id);
            }
            internal_renderer.sortingLayerID = sorting_layer.id;
        }

        protected void set_renderer(optional<Renderer> renderer, System.Type type)
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

        protected void set_renderer_values(string shader)
        {
            internal_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            internal_renderer.receiveShadows = false;
            internal_renderer.material.shader = Shader.Find(shader);
            internal_renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            internal_renderer.sharedMaterial = material;
        }

        protected void set_transformation(string name)
        {
            optional<Transform> transformation = this.gameObject.transform.Find(name);
            if (!transformation.exists)
            {
                GameObject child = new GameObject(name);
                child.transform.parent = this.transform;
                transformation = child.GetComponent<Transform>();
            }
            internal_transformation = transformation.data;
        }

        [SerializeField] public SortingLayer sorting_layer;

        /// <summary>
        /// Property - A number [-32768, 32767] that determines drawing order (-32768 = background, 32767 = foreground).
        /// Up to 65536 objects can be created on the same layer without z-fighting (but z-fighting might happen with fewer if objects are destroyed at runtime).
        /// If undefined, it will be arbitrarily chosen.
        /// </summary>
        public optional<short> drawing_order
        {
            get
            {
                return sorting_order;
            }
            set
            {
                sorting_order = value;
                if (sorting_order.exists)
                {
                    internal_renderer.sortingOrder = sorting_order.data;
                }
            }
        }

        /// <summary>
        /// Property - the radius as an angle (in radians) along the surface of the sphere.
        /// </summary>
        public float scale { get; set; } // FIXME: 

        [SerializeField] public Material material;

        [SerializeField] protected optional<short> sorting_order;
        protected Transform internal_transformation;
        protected Renderer internal_renderer;

        protected static short next_available_order(int layer_identifier)
        {
            short order = order_identifier;
            if (order == short.MaxValue)
            {
                order_identifier = short.MinValue;
            }
            else
            {
                ++order_identifier;
            }
            return order;
        }

        private static short order_identifier = short.MinValue; // CONSIDER: use layer map again if convenient
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