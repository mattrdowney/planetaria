#if UNITY_EDITOR 

using UnityEngine;
using System.Collections.Generic;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class EquilateralBuilder : MonoBehaviour  // TODO: PlanetariaComponent
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
            GameObject shape = new GameObject("CustomGeometry");
            PlanetariaCollider collider = shape.AddComponent<PlanetariaCollider>();
            collider.shape = new PlanetariaShape(curves, true, true);
            DestroyImmediate(this.gameObject);

            /*optional<TextAsset> svg_file = BlockRenderer.render(block, edges == 2 ? 1 : 0); // lines can't have zero thickness
            if (svg_file.exists)
            {
                PlanetariaRenderer renderer = shape.AddComponent<PlanetRenderer>();
                renderer.material = RenderVectorGraphics.render(svg_file.data);
            }*/
        }

        private void create_equilateral()
        {
            if (edges > 0 && center != first_vertex)
            {
                if (edges >= 2)
                {
                    Vector3 forward = center.normalized;
                    Vector3 right = Vector3.ProjectOnPlane(first_vertex, forward).normalized;
                    Vector3 up = Vector3.Cross(forward, right).normalized;

                    float phi = Vector3.Angle(first_vertex, center)*Mathf.Deg2Rad;
                    
                    List<Vector3> vertices = new List<Vector3>();
                    for (float edge_index = 0; edge_index < edges; ++edge_index)
                    {
                        Vector3 equatorial_position = PlanetariaMath.spherical_linear_interpolation(right, up, -(edge_index/edges)*(Mathf.PI*2));
                        Vector3 final_position = PlanetariaMath.spherical_linear_interpolation(forward, equatorial_position, phi);
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
                    arcs.Clear();
                    curves.Clear();

                    // first_vertex is circle start
                    Vector3 right = Vector3.Cross(center, first_vertex).normalized;
                    Vector3 mirror = Vector3.Cross(center, right).normalized;
                    Vector3 hidden_vertex = Vector3.Reflect(first_vertex, mirror).normalized; // opposite end of circle start

                    Vector3 first_up = Vector3.Cross(first_vertex, right).normalized;
                    Vector3 first_tangent = -Vector3.Cross(first_up, first_vertex).normalized;
                    
                    Vector3 second_up = -Vector3.Cross(hidden_vertex, right).normalized;
                    Vector3 second_tangent = -Vector3.Cross(second_up, hidden_vertex).normalized;

                    arcs.Add(Arc.curve(first_vertex, first_tangent, hidden_vertex)); // draw first semi-circle
                    curves.Add(GeospatialCurve.curve(first_vertex, first_tangent));

                    arcs.Add(Arc.curve(hidden_vertex, second_tangent, first_vertex)); // draw second semi-circle
                    curves.Add(GeospatialCurve.curve(hidden_vertex, second_tangent));

                }
            }
        }
    
        public List<Arc> arcs = new List<Arc>(); // TODO: use Shape
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