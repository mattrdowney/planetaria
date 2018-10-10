using System.Collections.Generic;
using UnityEngine;

public class TessellatedTriangle
{
    public static Mesh generate(Mesh mesh, int triangle, int level_of_detail, bool triangle_strip = false)
    {
        triangle *= 3;
        uv_a = mesh.uv[mesh.triangles[triangle+0]].normalized;
        uv_b = mesh.uv[mesh.triangles[triangle+1]].normalized;
        uv_c = mesh.uv[mesh.triangles[triangle+2]].normalized;
        vertex_a = mesh.vertices[mesh.triangles[triangle+0]].normalized;
        vertex_b = mesh.vertices[mesh.triangles[triangle+1]].normalized;
        vertex_c = mesh.vertices[mesh.triangles[triangle+2]].normalized;
        Vector3 ba_edge = vertex_b - vertex_a;
        Vector3 cb_edge = vertex_c - vertex_b;
        triangle_plane = Vector3.Cross(ba_edge, cb_edge);
        area_doubled = triangle_plane.magnitude;

        if (level_of_detail < 0)
        {
            Debug.LogError("TessellatedTriangle::generate() must have level_of_detail 0 or greater.");
        }
        List<Vector2> interpolators = new List<Vector2>();
        int rows = level_of_detail + 1;
        for (int row = 0; row < rows; row += 1)
        {
            float top_row_interpolator = (row+0)/(float)rows;
            float bottom_row_interpolator = (row+1)/(float)rows;
            //zig-zag through the triangles (back and forth as a single strip)
            bool zig = true;
            TriangleStripDirection next_direction = TriangleStripDirection.TopDown;
            int top_columns = (row+0);
            int bottom_columns = (row+1);
            int top_column = 0;
            int bottom_column = 0;
            while (bottom_column < bottom_columns || next_direction == TriangleStripDirection.BottomUp)
            {
                if (next_direction == TriangleStripDirection.TopDown ||
                        next_direction == TriangleStripDirection.TopRight)
                {
                    float top_column_interpolator = (zig ? top_column : top_columns - top_column);
                    top_column_interpolator /= top_columns;
                    interpolators.Add(new Vector2(top_row_interpolator, top_column_interpolator));
                    
                    if (next_direction == TriangleStripDirection.TopDown &&
                            top_column != 0) // same point has to be added for up to three triangles
                    {
                        interpolators.Add(new Vector2(top_row_interpolator, top_column_interpolator));
                    }
                }
                else // bottom
                {
                    float bottom_column_interpolator = (zig ? bottom_column : bottom_columns - bottom_column);
                    bottom_column_interpolator /= bottom_columns;
                    interpolators.Add(new Vector2(bottom_row_interpolator, bottom_column_interpolator));
                    
                    if (next_direction == TriangleStripDirection.BottomUp &&
                            bottom_column != bottom_columns) // same point has to be added for up to three triangles
                    {
                        interpolators.Add(new Vector2(bottom_row_interpolator, bottom_column_interpolator));
                    }
                }
                
                if (next_direction == TriangleStripDirection.TopRight)
                {
                    top_column += 1;
                }
                else if (next_direction == TriangleStripDirection.BottomRight)
                {
                    bottom_column += 1;
                }
                next_direction = (TriangleStripDirection)(((int)next_direction + 1) % 4); // cycle through directions
            }
        }
        List<Vector2> flipped_interpolators = new List<Vector2>();
        if (triangle_strip == false)
        {
            for (int triangle_index = 0; triangle_index < interpolators.Count; triangle_index += 3)
            {
                // flip triangle vertices
                Vector2 interpolator_1 = interpolators[triangle_index+0];
                Vector2 interpolator_2 = interpolators[triangle_index+1];
                Vector2 interpolator_3 = interpolators[triangle_index+2];
                Vector3 ba_vector = interpolator_2 - interpolator_1;
                Vector3 cb_vector = interpolator_3 - interpolator_2;
                Vector3 triangle_direction = Vector3.Cross(ba_vector, cb_vector);
                bool cull_triangle = Vector3.Dot(triangle_direction, interpolator_1) < 0;
                if (cull_triangle)
                {
                    flipped_interpolators.AddRange(new List<Vector2>{ interpolator_3, interpolator_2, interpolator_1 }); // flip 1 and 3
                }
                else
                {
                    flipped_interpolators.AddRange(new List<Vector2>{ interpolator_1, interpolator_2, interpolator_3 });
                }
            }
        }
        List<int> triangle_set = new List<int>();
        List<Vector2> uv_set = new List<Vector2>();
        List<Vector3> vertex_set = new List<Vector3>();
        for (int index = 0; index < flipped_interpolators.Count; index += 1)
        {
            triangle_set.Add(index);
            uv_set.Add(interpolated_uv(flipped_interpolators[index]));
            vertex_set.Add(interpolated_vertex(flipped_interpolators[index]));
        }
        Mesh result = new Mesh();
        result.uv = uv_set.ToArray();
        result.vertices = vertex_set.ToArray();
        result.normals = vertex_set.ToArray(); // TODO: verify
        result.triangles = triangle_set.ToArray();
        result.RecalculateBounds();
        return result;
    }

    private static Vector3 interpolated_vertex(Vector2 interpolator)
    {
        Vector3 left_rail = Vector3.Slerp(uv_a, vertex_b, interpolator.x);
        Vector3 right_rail = Vector3.Slerp(uv_a, vertex_c, interpolator.x);
        return Vector3.Slerp(left_rail, right_rail, interpolator.y);
    }

    private static Vector2 interpolated_uv(Vector2 interpolator)
    {
        Vector3 sphere_position = interpolated_vertex(interpolator);
        Vector3 triangle_position = Vector3.ProjectOnPlane(sphere_position, triangle_plane);
        
        // find Barycentric vectors
        var vector_a = vertex_a - triangle_position;
        var vector_b = vertex_b - triangle_position;
        var vector_c = vertex_c - triangle_position;

        // find Barycentric weights
        float weight_a = Vector3.Cross(vector_b, vector_c).magnitude / area_doubled;
        float weight_b = Vector3.Cross(vector_c, vector_a).magnitude / area_doubled;
        float weight_c = Vector3.Cross(vector_a, vector_b).magnitude / area_doubled;

        // find uv coordinate
        return uv_a * weight_a + uv_b * weight_b + uv_c * weight_c;
    }
    
    private enum TriangleStripDirection { TopDown = 0, BottomRight = 1, BottomUp = 2, TopRight = 3 }
    private static Vector2 uv_a;
    private static Vector2 uv_b;
    private static Vector2 uv_c;
    private static Vector3 vertex_a;
    private static Vector3 vertex_b;
    private static Vector3 vertex_c;
    private static float area_doubled;
    private static Vector3 triangle_plane;
}

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