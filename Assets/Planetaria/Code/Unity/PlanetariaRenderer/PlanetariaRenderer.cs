using System;
using UnityEngine;

namespace Planetaria
{
    [DisallowMultipleComponent]
    [Serializable]
    public abstract class PlanetariaRenderer : PlanetariaComponent
    {
        protected override sealed void Awake()
        {
            base.Awake();
            initialize();
        }

        protected override void OnEnable()
        {
            this.transform.Find("__Renderer").gameObject.internal_game_object.SetActive(true);
        }

        protected override void OnDisable()
        {
            this.transform.Find("__Renderer").gameObject.internal_game_object.SetActive(false);
        }

        protected override sealed void Reset()
        {
            base.Reset();
            initialize();
        }

        private void initialize()
        {
            set_transformation();
            set_renderer();
            set_renderer_values();
            set_draw_order();
        }

        protected abstract void set_renderer();

        protected void set_draw_order()
        {
            if (!drawing_order.exists)
            {
                drawing_order = next_available_order(sorting_layer.id);
            }
            //internal_renderer.sortingLayerID = sorting_layer.id;
        }

        protected void set_renderer_values()
        {
            internal_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            internal_renderer.receiveShadows = false;
            internal_renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }

        protected void set_transformation()
        {
            if (internal_transform == null)
            {
                GameObject child = this.GetOrAddChild("Renderer");
                internal_transform = child.GetComponent<Transform>();
            }
            internal_transform.localPosition = offset * Vector3.forward;
        }

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
                    //internal_renderer.sortingOrder = sorting_order.data;
                }
            }
        }

        /// <summary>
        /// Property - the radius as an angle (in radians) along the surface of the sphere.
        /// </summary>
        public float scale
        {
            get
            {
                return scale_variable;
            }
            set
            {
                scale_variable = value;
                internal_transform.localScale = Vector3.one * scale_variable/2;
            }
        }

        public float angle
        {
            get
            {
                return angle_variable;
            }
            set
            {
                angle_variable = value;
                internal_transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            }
        }

        [SerializeField] private float angle_variable;
        [SerializeField] public Material material;
        [SerializeField] public SortingLayer sorting_layer;
        [SerializeField] protected optional<short> sorting_order;
        
        [SerializeField] private float scale_variable;
        
        [SerializeField] [HideInInspector] protected Transform internal_transform;
        [SerializeField] [HideInInspector] protected Renderer internal_renderer;

        [SerializeField] public float offset = 1;

        protected static short next_available_order(int layer_identifier) // TODO: remove this
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

        [SerializeField] [HideInInspector] private static short order_identifier = short.MinValue; // CONSIDER: use layer map again if convenient
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