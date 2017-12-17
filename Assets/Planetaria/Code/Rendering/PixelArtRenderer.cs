#if UNITY_EDITOR

using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public static class PixelArtRenderer
    {
        public static void draw(string file, float scale)
        {
            scale = Mathf.Clamp(Mathf.Abs(scale), 0, Mathf.PI);
            VectorGraphicsWriter.write_header(file);
            optional<Texture2D> texture = Miscellaneous.fetch_image("Assets/Planetaria/Art/Textures/" + file + ".png");
            if (texture.exists)
            {
                Color[] pixel_buffer = texture.data.GetPixels();
                int rows = texture.data.height;
                int columns = texture.data.width;
                int max_size = Mathf.Max(rows, columns);
                float width = ((columns/max_size)*scale);
                float height = ((rows/max_size)*scale);

                NormalizedCartesianCoordinates[] cardinal_boundary_normals = new NormalizedCartesianCoordinates[4];
                cardinal_boundary_normals[0] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - width, 0.0f*Mathf.PI);
                cardinal_boundary_normals[1] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - height, 0.5f*Mathf.PI);
                cardinal_boundary_normals[2] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - width, 1.0f*Mathf.PI);
                cardinal_boundary_normals[3] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - height, 1.5f*Mathf.PI);

                NormalizedCartesianCoordinates[] cardinal_boundaries = new NormalizedCartesianCoordinates[4];
                cardinal_boundaries[0] = new NormalizedSphericalCoordinates(1.0f*Mathf.PI - width, 0.0f*Mathf.PI);
                cardinal_boundaries[1] = new NormalizedSphericalCoordinates(1.0f*Mathf.PI - height, 0.5f*Mathf.PI);
                cardinal_boundaries[2] = new NormalizedSphericalCoordinates(1.0f*Mathf.PI - width, 1.0f*Mathf.PI);
                cardinal_boundaries[3] = new NormalizedSphericalCoordinates(1.0f*Mathf.PI - height, 1.5f*Mathf.PI);

                NormalizedCartesianCoordinates[] rectangle_corners = new NormalizedCartesianCoordinates[4];
                for (int side = 0; side < cardinal_boundary_normals.Length; ++side)
                {
                    rectangle_corners[side] = new NormalizedCartesianCoordinates(Vector3.Cross(cardinal_boundary_normals[side].data, cardinal_boundary_normals[(side + 1) % 4].data));

                    Debug.Log("Cardinal[" + side + "] = " + cardinal_boundaries[side].data.ToString("F4") + " ... " + "Rectangle[" + side + "] = " + rectangle_corners[side].data.ToString("F4"));
                }

                Arc upper_rail = Arc.line(rectangle_corners[0].data, rectangle_corners[1].data);
                Arc right_rail = Arc.line(rectangle_corners[1].data, rectangle_corners[2].data);
                Arc lower_rail = Arc.line(rectangle_corners[3].data, rectangle_corners[2].data);
                Arc left_rail = Arc.line(rectangle_corners[0].data, rectangle_corners[3].data);

                Arc horizontal_rail = Arc.line(cardinal_boundaries[0].data, cardinal_boundaries[2].data);
                Arc vertical_rail = Arc.line(cardinal_boundaries[1].data, cardinal_boundaries[3].data);

                Arc[] upper_arcs = get_arcs(left_rail, vertical_rail, right_rail, rows);
                Arc[] lower_arcs = get_arcs(right_rail, vertical_rail, left_rail, rows, true);
                Arc[] right_arcs = get_arcs(upper_rail, horizontal_rail, lower_rail, columns, true);
                Arc[] left_arcs = get_arcs(lower_rail, horizontal_rail, upper_rail, columns);
            
                VectorGraphicsWriter.begin_shape();
                ShapeRenderer.draw_arc(upper_arcs[0], 0, 1);
                ShapeRenderer.draw_arc(right_arcs[columns-1], 0, 1);
                ShapeRenderer.draw_arc(lower_arcs[rows-1], 0, 1);
                ShapeRenderer.draw_arc(left_arcs[0], 0, 1);
                VectorGraphicsWriter.end_shape(Color.black);

                for (int row = 0; row < rows; ++row)
                {
                    float prior_row = row/(float)rows;
                    float later_row = (row+1)/(float)rows;
                    for (int column = 0; column < columns; ++column)
                    {
                        float prior_column = column/(float)columns;
                        float later_column = (column+1)/(float)columns;
                        VectorGraphicsWriter.begin_shape();
                        ShapeRenderer.draw_arc(upper_arcs[row], prior_column, later_column); // upper row
                        ShapeRenderer.draw_arc(right_arcs[column], prior_row, later_row); // right column
                        ShapeRenderer.draw_arc(lower_arcs[row], (1-later_column), (1-prior_column)); // lower row
                        ShapeRenderer.draw_arc(left_arcs[column], (1-later_row), (1-prior_row)); // left column
                        VectorGraphicsWriter.end_shape(pixel_buffer[columns*row + column]);
                    }
                }
            }
            VectorGraphicsWriter.write_footer(file, false);
        }

        private static Arc[] get_arcs(Arc first_rail, Arc inner_rail, Arc last_rail, int segments, bool offset = false)
        {
            Arc[] arcs = new Arc[segments];
            for (int segment = 0; segment < segments; ++segment)
            {
                float interpolation = (segment+(offset?1:0))/(float)segments;
                arcs[segment] = Arc.curve(
                        first_rail.interpolate(interpolation),
                        inner_rail.interpolate(interpolation),
                        last_rail.interpolate(interpolation));
            }
            return arcs;
        }
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
