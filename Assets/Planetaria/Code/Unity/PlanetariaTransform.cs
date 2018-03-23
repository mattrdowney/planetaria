using UnityEngine;

namespace Planetaria
{
    // FIXME: find a way to serialize
    public class PlanetariaTransform : MonoBehaviour
    {
        private void Awake()
        {
            Transform internal_transformation = this.GetComponent<Transform>();
            cartesian_transform = internal_transformation;
            internal_collider = internal_transformation.GetComponent<PlanetariaCollider>(); // FIXME: undefined behavior
            internal_renderer = internal_transformation.GetComponent<PlanetariaRenderer>(); // FIXME:

            position = new NormalizedCartesianCoordinates(cartesian_transform.forward);
            direction = new NormalizedCartesianCoordinates(Vector3.up);
            scale = cartesian_transform.localScale.x;
        }

        public NormalizedCartesianCoordinates position
        {
            get
            {
                return position_variable;
            }
            set
            {
                position_variable = value;
                cartesian_transform.rotation = Quaternion.LookRotation(position.data, direction.data);
            }
        }

        /// <summary>
        /// The direction the object faces. The object will rotate towards "direction".
        /// </summary>
        /// <example>arc.normal() [dynamic] or Vector3.up [static]</example>
        public NormalizedCartesianCoordinates direction
        {
            get
            {
                return direction_variable;
            }
            set
            {
                direction_variable = value;
                cartesian_transform.rotation = Quaternion.LookRotation(position.data, direction.data);
            }
        }

        /// <summary>
        /// The diameter of the player - divide by two when you need the radius/extrusion.
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
                if (internal_collider.exists)
                {
                    internal_collider.data.scale = scale; // TODO: check
                }
                if (internal_renderer.exists)
                {
                    internal_renderer.data.scale = scale; // TODO: check
                }
            }
        }

        private Transform cartesian_transform;
        private optional<PlanetariaCollider> internal_collider; // Observer pattern would be more elegant but slower
        private optional<PlanetariaRenderer> internal_renderer;

        //private Planetarium planetarium_variable; // cartesian_transform's position
        private NormalizedCartesianCoordinates position_variable = new NormalizedCartesianCoordinates(Vector3.forward);
        private NormalizedCartesianCoordinates direction_variable = new NormalizedCartesianCoordinates(Vector3.forward);
        private float scale_variable;
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