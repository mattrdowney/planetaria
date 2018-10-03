#if UNITY_EDITOR 

using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class EquilateralBuilder : MonoBehaviour  // TODO: PlanetariaComponent
    {
        public static EquilateralBuilder equilateral_builder(Vector3 center, int sides)
        {
            GameObject game_object = new GameObject("EquilateralBuilder");
            EquilateralBuilder result = game_object.AddComponent<EquilateralBuilder>();
            result.center = result.first_vertex = center;
            result.edges = sides;
            result.shape = create_equilateral(center, center, sides);
            return result;
        }

        public void set_edge(Vector3 first_corner)
        {
            this.first_vertex = first_corner;
            shape = create_equilateral(center, first_corner, edges);
        }

        public void close_shape()
        {
            GameObject geometry_object = new GameObject("CustomGeometry");
            PlanetariaCollider collider = geometry_object.AddComponent<PlanetariaCollider>();
            collider.shape = shape;
            DestroyImmediate(this.gameObject);

            /*optional<TextAsset> svg_file = BlockRenderer.render(block, edges == 2 ? 1 : 0); // lines can't have zero thickness
            if (svg_file.exists)
            {
                PlanetariaRenderer renderer = shape.AddComponent<PlanetRenderer>();
                renderer.material = RenderVectorGraphics.render(svg_file.data);
            }*/
        }

        // Preview rendering of this function is non-trivial, since ephemeral edges only work for the most recent edge.
        public static PlanetariaShape create_equilateral(Vector3 center, Vector3 vertex, int faces)
        {
            if (faces > 0 && center != vertex)
            {
                if (faces >= 2)
                {
                    Vector3 forward = center.normalized;
                    Vector3 right = Vector3.ProjectOnPlane(vertex, forward).normalized;
                    Vector3 up = Vector3.Cross(forward, right).normalized;

                    float phi = Vector3.Angle(vertex, center)*Mathf.Deg2Rad;
                    
                    List<Vector3> vertices = new List<Vector3>();
                    for (float face_index = 0; face_index < faces; ++face_index)
                    {
                        Vector3 equatorial_position = PlanetariaMath.spherical_linear_interpolation(right, up, -(face_index/faces)*(Mathf.PI*2));
                        Vector3 final_position = PlanetariaMath.spherical_linear_interpolation(forward, equatorial_position, phi);
                        vertices.Add(final_position);
                    }

                    List<SerializedArc> polygon = new List<SerializedArc>();
                    for (int face_index = 0; face_index < faces; ++face_index)
                    {
                        Vector3 start_point = vertices[face_index];
                        Vector3 end_point = vertices[(face_index+1)%faces];
                        polygon.Add(ArcFactory.line(start_point, end_point));
                    }
                    return PlanetariaShape.Create(polygon, true, true);
                }
                else // create a circle with given radius
                {
                    // TODO: this entire function can be replaced now (with the circle generator)

                    // first_vertex is circle start
                    Vector3 right = Vector3.Cross(center, vertex).normalized;
                    Vector3 mirror = Vector3.Cross(center, right).normalized;
                    Vector3 hidden_vertex = Vector3.Reflect(vertex, mirror).normalized; // opposite end of circle start

                    Vector3 first_up = Vector3.Cross(vertex, right).normalized;
                    Vector3 first_tangent = -Vector3.Cross(first_up, vertex).normalized;
                    
                    Vector3 second_up = -Vector3.Cross(hidden_vertex, right).normalized;
                    Vector3 second_tangent = -Vector3.Cross(second_up, hidden_vertex).normalized;

                    SerializedArc upper_circle = ArcFactory.curve(vertex, first_tangent, hidden_vertex);
                    SerializedArc lower_circle = ArcFactory.curve(hidden_vertex, second_tangent, vertex);

                    return PlanetariaShape.Create(new List<SerializedArc>(){ upper_circle, lower_circle }, true, true);
                }
            }
            return PlanetariaShape.Create(new List<SerializedArc>(){ }, true, true);
        }
    
        public PlanetariaShape shape { get; set; }
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