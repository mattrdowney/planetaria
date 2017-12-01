﻿using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode] // FIXME: scene file stores a new Renderer reference each save due to implementation, ISerializationCallbackReceiver can't delete objects either
    public abstract class PlanetariaRenderer : MonoBehaviour
    {
        protected abstract void set_renderer();

        protected void set_layer()
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

        protected void set_transformation(string name)
        {
            optional<Transform> child = this.transform.Find(name);
            if (!child.exists)
            {
                child = new GameObject(name).transform;
                child.data.parent = this.transform;
                child.data.hideFlags = HideFlags.DontSave;
                //child.data.hideFlags |= HideFlags.HideInHierarchy; // FIXME: when Planetaria goes to 1.0, this should be added.
            }
            internal_transformation = child.data.GetComponent<Transform>();
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
                if (scalable)
                {
                    internal_transformation.localScale = Vector3.one * scale_variable;
                }
            }
        }

        [SerializeField] public Material material;
        [SerializeField] public SortingLayer sorting_layer;
        [SerializeField] protected optional<short> sorting_order;
        
        private float scale_variable;
        protected bool scalable;

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