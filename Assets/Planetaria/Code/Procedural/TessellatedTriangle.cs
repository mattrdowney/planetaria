using System.Collections.Generic;
using UnityEngine;

public class TessellatedTriangle
{
    public static Mesh generate(Mesh mesh, int triangle, int level_of_detail, bool triangle_strip = false)
    {
        triangle *= 3;
        Vector3 a = mesh.vertices[mesh.triangles[triangle+0]].normalized;
        Vector3 b = mesh.vertices[mesh.triangles[triangle+1]].normalized;
        Vector3 c = mesh.vertices[mesh.triangles[triangle+2]].normalized;

        if (level_of_detail < 0)
        {
            Debug.LogError("TessellatedTriangle::generate() must have level_of_detail 0 or greater.");
        }
        List<Vector3> vertices = new List<Vector3>();
        int rows = level_of_detail + 1;
        for (int row = 0; row < rows; row += 1)
        {
            float top_row_interpolator = (row+0)/(float)rows;
            float bottom_row_interpolator = (row+1)/(float)rows;
            //zig-zag through the triangles (back and forth as a single strip)
            Vector3 zig = (row % 2 == 0 ? b : c);
            Vector3 zag = (row % 2 == 0 ? c : b);
            Vector3 top_left = Vector3.Slerp(a, zig, top_row_interpolator);
            Vector3 bottom_left = Vector3.Slerp(a, zig, bottom_row_interpolator);
            Vector3 top_right = Vector3.Slerp(a, zag, top_row_interpolator);
            Vector3 bottom_right = Vector3.Slerp(a, zag, bottom_row_interpolator);
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
                    vertices.Add(Vector3.Slerp(top_left, top_right, top_column/(float)top_columns));
                    
                    if (next_direction == TriangleStripDirection.TopDown &&
                            top_column != 0 /*&& top_column != top_columns*/) // same point has to be added for up to three triangles
                    {
                        vertices.Add(Vector3.Slerp(top_left, top_right, top_column/(float)top_columns));
                    }
                }
                else // bottom
                {
                    vertices.Add(Vector3.Slerp(bottom_left, bottom_right, bottom_column/(float)bottom_columns));
                    
                    if (next_direction == TriangleStripDirection.BottomUp &&
                            bottom_column != 0 && bottom_column != bottom_columns) // same point has to be added for up to three triangles
                    {
                        vertices.Add(Vector3.Slerp(bottom_left, bottom_right, bottom_column/(float)bottom_columns));
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
        List<Vector3> flipped_vertices = new List<Vector3>();
        if (triangle_strip == false)
        {
            for (int triangle_index = 0; triangle_index < vertices.Count; triangle_index += 3)
            {
                // flip triangle vertices
                Vector3 subvertex_a = vertices[triangle_index+0];
                Vector3 subvertex_b = vertices[triangle_index+1];
                Vector3 subvertex_c = vertices[triangle_index+2];
                Vector3 ba_vector = subvertex_b - subvertex_a;
                Vector3 cb_vector = subvertex_c - subvertex_b;
                Vector3 triangle_direction = Vector3.Cross(ba_vector, cb_vector);
                bool cull_triangle = Vector3.Dot(triangle_direction, subvertex_a) < 0;
                if (cull_triangle)
                {
                    flipped_vertices.AddRange(new List<Vector3>{ subvertex_c, subvertex_b, subvertex_b }); // flip a and c
                }
                else
                {
                    flipped_vertices.AddRange(new List<Vector3>{ subvertex_a, subvertex_b, subvertex_c });
                }
            }
        }
    }

    private enum TriangleStripDirection { TopDown = 0, BottomRight = 1, BottomUp = 2, TopRight = 3 }
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