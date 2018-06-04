using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct NormalizedOctahedronCoordinates // https://knarkowicz.wordpress.com/2014/04/16/octahedron-normal-vector-encoding/
    {
        public Vector3 data
        {
            get { return data_variable; }
            set { data_variable = value; normalize(); }
        }

        /// <summary>
        /// Constructor - Stores octahedron coordinates in a wrapper class.
        /// </summary>
        /// <param name="octahedron">The octahedron coordinates. Note: matches Unity's default Vector3 definition.</param>
        public NormalizedOctahedronCoordinates(Vector3 octahedron)
        {
            data_variable = octahedron;
            normalize();
        }

        /// <summary>
        /// Inspector - Converts octahedron coordinates into Cartesian coordinates.
        /// </summary>
        /// <param name="octahedron">The octahedron that will be converted</param>
        /// <returns>The Cartesian coordinates.</returns> 
        public static implicit operator NormalizedCartesianCoordinates(NormalizedOctahedronCoordinates octahedron)
        {
            return new NormalizedCartesianCoordinates(octahedron.data);
        }
        
        /// <summary>
        /// Inspector - Converts octahedron coordinates into octahedron UV coordinates.
        /// </summary>
        /// <param name="octahedron">The octahedron that will be converted</param>
        /// <returns>The UV coordinates of an octahedron.</returns> 
        public static implicit operator OctahedronUVCoordinates(NormalizedOctahedronCoordinates octahedron)
        {
            Vector2 uv = new Vector2(octahedron.data.x, octahedron.data.z);
            if (octahedron.data.y < 0) // reflect across diamond
            {
                float immutable_u = uv.x; // the uv.y = ... would reference a changed variable otherwise
                float immutable_v = uv.y;
                uv.x = Mathf.Sign(immutable_u) * (1 - Mathf.Abs(immutable_v)); // invert (interchange x/y)
                uv.y = Mathf.Sign(immutable_v) * (1 - Mathf.Abs(immutable_u));
            }
            uv.x = 0.5f + uv.x/2;
            uv.y = 0.5f + uv.y/2;
            return new OctahedronUVCoordinates(uv.x, uv.y);
        }

        /// <summary>
        /// Mutator - Normalizes Cartesian vector using Manhattan distance
        /// </summary>
        private void normalize()
        {
            float length = PlanetariaMath.manhattan_distance(data_variable);
            float absolute_error = Mathf.Abs(length-1);
            if (absolute_error > Precision.tolerance)
            {
                data_variable /= length;
            }
        }

        [SerializeField] private Vector3 data_variable;
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