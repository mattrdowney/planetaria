/*
using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    public sealed class ArcRenderer : PlanetariaRenderer // TODO: ? number of vertices should be variable based on the curvature of the arc.
    {
        protected sealed override void set_renderer()
        {
            if (internal_renderer == null)
            {
                internal_renderer = Miscellaneous.GetOrAddComponent<LineRenderer>(internal_transform);
            }
            internal_renderer.sharedMaterial = material;
            LineRenderer line_renderer = internal_transform.GetComponent<LineRenderer>();
            line_renderer.alignment = LineAlignment.View; // both options suck, but this one renders on both sides (the right shader could fix it, but it's not gonna work well no matter what)
            line_renderer.startWidth = line_renderer.endWidth = angular_width;
            recalculate();
        }

        public void recalculate()
        {
            LineRenderer line_renderer = internal_transform.GetComponent<LineRenderer>();
            List<Vector3> vertices = new List<Vector3>();
            line_renderer.positionCount = 0;
            line_renderer.SetPositions(new Vector3[0]);
            foreach (Arc arc in shape.arcs)
            {
                int line_segment_count = Mathf.CeilToInt((360*arc.angle()) / (Mathf.PI*2));
                for (int vertex = 0; vertex <= line_segment_count; ++vertex)
                {
                    float fraction = vertex / (float)line_segment_count;
                    float local_angle = Mathf.Lerp(-arc.angle()/2, +arc.angle()/2, fraction);
                    Vector3 position = arc.position(local_angle);
                    vertices.Add(position);
                }
            }
            line_renderer.positionCount = vertices.Count;
            line_renderer.SetPositions(vertices.ToArray());
        }

        [SerializeField] public float angular_width = 0.01f;
        [SerializeField] public PlanetariaShape shape;
    }
}
*/

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