using UnityEngine;

namespace Planetaria
{
    // FIXME: find a way to serialize
    public class PlanetariaTransform
    {
        public PlanetariaTransform(Transform internal_transform)
        {
            cartesian_transform = internal_transform;
            internal_collider = internal_transform.GetComponent<PlanetariaCollider>();
            internal_renderer = internal_transform.GetComponent<PlanetariaRenderer>();
        }

        public NormalizedCartesianCoordinates position
        {
            get
            {
                return position_variable;
            }
            set
            {
                dirty_position = true;
                previous_position = position_variable;
                position_variable = value;
            }
        }

        public NormalizedCartesianCoordinates previous_position { get; private set; }

        public float rotation
        {
            get
            {
                return rotation_variable;
            }
            set
            {
                dirty_rotation = true;
                rotation_variable = value;
            }
        }

        public float scale
        {
            get
            {
                return scale_variable;
            }
            set
            {
                dirty_scale = true;
                scale_variable = value;
            }
        }

        public Planetarium planetarium
        {
            get
            {
                return planetarium_variable;
            }
            set
            {
                dirty_planetarium = true;
                planetarium_variable = value;
            }
        }

        public void move()
        {
            if (dirty_position)
            {
                internal_position = Quaternion.LookRotation(position.data);
            }

            if (dirty_rotation)
            {
                internal_rotation = Quaternion.Euler(0, 0, rotation*Mathf.Rad2Deg);
            }

            if (dirty_position || dirty_rotation)
            {
                cartesian_transform.rotation = internal_position * internal_rotation;
                dirty_position = dirty_rotation = false;
            }

            if (dirty_scale)
            {
                if (internal_collider.exists)
                {
                    internal_collider.data.scale = Mathf.Sin(scale)*Vector3.one;
                }
                if (internal_renderer.exists)
                {
                    internal_renderer.data.scale = scale;
                }
                dirty_scale = false;
            }
        }

        private Transform cartesian_transform;
        private optional<PlanetariaCollider> internal_collider; // Observer pattern would be more elegant but slower
        private optional<PlanetariaRenderer> internal_renderer;

        private Quaternion internal_position;
        private Quaternion internal_rotation;

        private Planetarium planetarium_variable;
        private NormalizedCartesianCoordinates position_variable;
        private float rotation_variable;
        private float scale_variable;

        private bool dirty_position;
        private bool dirty_rotation;
        private bool dirty_scale;
        private bool dirty_planetarium;
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