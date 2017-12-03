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
            internal_collider = internal_transformation.GetComponent<PlanetariaCollider>();
            internal_renderer = internal_transformation.GetComponent<PlanetariaRenderer>();
        }

        private void FixedUpdate() // FIXME: move into PlanetariaGameLoop?
        {
            move();
        }

        public NormalizedCartesianCoordinates position
        {
            get
            {
                return position_variable.data;
            }
            set
            {
                previous_position = position_variable.data;
                position_variable = value;
            }
        }

        public NormalizedCartesianCoordinates previous_position { get; private set; }

        public float rotation
        {
            get
            {
                return rotation_variable.data;
            }
            set
            {
                rotation_variable = value;
            }
        }

        public float scale
        {
            get
            {
                return scale_variable.data;
            }
            set
            {
                scale_variable = value;
            }
        }

        public Planetarium planetarium
        {
            get
            {
                return planetarium_variable.data;
            }
            set
            {
                planetarium_variable = value;
            }
        }

        public void move()
        {
            if (planetarium_variable.dirty)
            {
                cartesian_transform.position = planetarium.position;
                planetarium_variable.clear();
            }

            if (position_variable.dirty)
            {
                internal_position = Quaternion.LookRotation(position.data);
            }
            if (rotation_variable.dirty)
            {
                internal_rotation = Quaternion.Euler(0, 0, rotation*Mathf.Rad2Deg);
            }
            if (position_variable.dirty || rotation_variable.dirty)
            {
                cartesian_transform.rotation = internal_position * internal_rotation;
                position_variable.clear();
                rotation_variable.clear();
            }

            if (scale_variable.dirty)
            {
                if (internal_collider.exists)
                {
                    internal_collider.data.scale = Mathf.Sin(scale); // TODO: check
                }
                if (internal_renderer.exists)
                {
                    internal_renderer.data.scale = scale; // TODO: check
                }
                scale_variable.clear();
            }
        }

        private Transform cartesian_transform;
        private optional<PlanetariaCollider> internal_collider; // Observer pattern would be more elegant but slower
        private optional<PlanetariaRenderer> internal_renderer;

        private Quaternion internal_position;
        private Quaternion internal_rotation;

        private dirtyable<Planetarium> planetarium_variable;
        private dirtyable<NormalizedCartesianCoordinates> position_variable;
        private dirtyable<float> rotation_variable;
        private dirtyable<float> scale_variable;
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