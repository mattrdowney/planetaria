#if UNITY_EDITOR 

using UnityEngine;
using System.Collections.Generic;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class EquilateralBuilder : MonoBehaviour
    {
        public static EquilateralBuilder equilateral_builder(Vector3 center, int sides)
        {
            GameObject game_object = new GameObject("Equilateral Builder");
            EquilateralBuilder result = game_object.AddComponent<EquilateralBuilder>();
            result.center = result.first_vertex = center;
            result.edges = sides;
            result.create_equilateral();
            return result;
        }

        public void set_edge(Vector3 first_corner)
        {
            this.first_vertex = first_corner;
            create_equilateral();
        }

        public void close_shape()
        {
            GameObject shape = Block.block(curves);
            Block block = shape.GetComponent<Block>();
            optional<TextAsset> svg_file = BlockRenderer.render(block);
            if (svg_file.exists)
            {
                //PlanetariaRenderer renderer = shape.AddComponent<PlanetRenderer>();
                //renderer.material = RenderVectorGraphics.render(svg_file.data);
            }
            DestroyImmediate(this.gameObject);
        }

        private void create_equilateral()
        {
            if (edges > 0)
            {
                if (edges >= 2 && center != first_vertex)
                {
                    Vector3 forward = center.normalized;
                    Vector3 right = Vector3.ProjectOnPlane(first_vertex, forward).normalized;
                    Vector3 up = Vector3.Cross(forward, right).normalized;

                    float phi = Vector3.Angle(first_vertex, center)*Mathf.Deg2Rad;
                    
                    List<Vector3> vertices = new List<Vector3>();
                    for (float edge_index = 0; edge_index < edges; ++edge_index)
                    {
                        Vector3 equatorial_position = PlanetariaMath.slerp(right, up, -(edge_index/edges)*(Mathf.PI*2));
                        Vector3 final_position = PlanetariaMath.slerp(forward, equatorial_position, phi);
                        vertices.Add(final_position);
                    }

                    arcs.Clear();
                    curves.Clear();

                    for (int edge_index = 0; edge_index < edges; ++edge_index)
                    {
                        Vector3 start_point = vertices[edge_index];
                        Vector3 end_point = vertices[(edge_index+1)%edges];

                        arcs.Add(Arc.line(start_point, end_point));
                        curves.Add(GeospatialCurve.curve(start_point, end_point));
                    }
                }
                else // create a circle with given radius
                {

                }
            }
        }
    
        public List<Arc> arcs = new List<Arc>();
        private List<GeospatialCurve> curves = new List<GeospatialCurve>();
        private Vector3 center { get; set; }
        private Vector3 first_vertex { get; set; }
        private int edges { get; set; }
    }
}

#endif

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
