using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// Immutable structure - This is compact and should always represent a valid Arc even if the underlying data is random (caveat: Quaternion should be normalized)
    /// </summary>
    [Serializable]
    public struct SerializedArc
    {
        public SerializedArc(Quaternion compact_basis_vectors, float half_angle, float arc_latitude, GeometryType curvature)
        {
            this.compact_basis_vectors = compact_basis_vectors;
            this.half_angle = half_angle;
            this.arc_latitude = arc_latitude;
            this.curvature = curvature;
        }

        /// <summary>A compact representation of center_axis, forward_axis, and right_axis from Arc.</summary>
        [SerializeField] public readonly Quaternion compact_basis_vectors;
        /// <summary>The angle of the arc in radians divided by two (must be positive). Range: [-PI, +PI]</summary>
        [SerializeField] public readonly float half_angle;
        /// <summary>The angle of the arc from its parallel "equator". Range: [-PI/2, +PI/2]</summary>
        [SerializeField] public readonly float arc_latitude;
        /// <summary>The curvature of the arc (e.g. Corner/Edge, Straight/Convex/Concave).</summary>
        [SerializeField] public readonly GeometryType curvature;
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