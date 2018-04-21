﻿using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public class Field : MonoBehaviour
    {   
        /// <summary>
        /// Constructor - Generates a field using a .ssvg file.
        /// </summary>
        /// <param name="ssvg_file">
        /// The .ssvg (spherical scalable vector graphics) file that will generate the field.
        /// Special note: the .ssvg MUST be convex or all behavior is undefined.
        /// </param>
        /// <returns>The GameObject reference with an attached Field component.</returns>
        public static GameObject field(string ssvg_file) // TODO: add convex check asserts.
        {
            GameObject result = new GameObject();
            //Field field = result.AddComponent<Field>(); // FIXME: implement

            return result;
        }

        /// <summary>
        /// Checks if the center of mass (position) is below all of the planes.
        /// Special note: if a portion of the extruded volume is inside all planes, this function might still return false.
        /// </summary>
        /// <param name="position">A position on a unit-sphere.</param>
        /// <param name="radius">The radius [0,PI/2] to extrude.</param>
        /// <returns>True if position is below (inside) all of the planes; false otherwise.</returns>
        public bool contains(Vector3 position, float radius = 0f)
        {
            if (!active)
            {
                return false;
            }

            /*foreach (Plane plane in plane_list)
            {
                if (plane.GetSide(position))
                {
                    return false;
                }
            }*/

            return true;
        }

        private void Start()
        {
            transform = this.GetOrAddComponent<PlanetariaTransform>();
        }

        public bool active { get; set; }
        
        // TODO: implement - List<Plane> became PlanetariaCollider
        [SerializeField] public new PlanetariaTransform transform; // TODO: make arcs relative (for moving platforms)
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