using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class VectorGraphicsWriter // FIXME: TODO: clean this up! // CONSIDER: make into an abstract interface? // Parameters for "edge creation" get weird if an interface is used
{
    public static void begin_canvas()
    {
        scene_index = SceneManager.GetActiveScene().buildIndex;
        if (!Directory.Exists(svg_folder_path + "/" + scene_index))
        {
            Directory.CreateDirectory(svg_folder_path + "/" + scene_index);
        }
        svg_path = svg_relative_folder_path + scene_index + "/" + scene_index + ".txt";
        write_uv_header();
    }

    public static void begin_shape()
    {
        first = true;
        writer.Write("\t<path d=\"");
    }

    public static void set_edge(QuadraticBezierCurve curve)
    {
        if (first)
        {
            writer.Write("M" + (1 - curve.begin_uv.x) * scale + "," + curve.begin_uv.y * scale);
            first = false;
        }
        writer.Write(" Q" + (1 - curve.control_uv.x) * scale + "," + curve.control_uv.y * scale);
        writer.Write(" " + (1 - curve.end_uv.x) * scale + "," + curve.end_uv.y * scale);
    }

    public static void set_pixels(string file, float scale)
    {
        scale = Mathf.Clamp(Mathf.Abs(scale), 0, Mathf.PI);
        write_pixel_art_header("Assets/Planetaria/Art/VectorGraphics/checker_board.txt");
        optional<Texture2D> texture = Miscellaneous.fetch_image(file);
        if (texture.exists)
        {
            Color[] pixel_buffer = texture.data.GetPixels();
            int rows = texture.data.height;
            int columns = texture.data.width;
            int max_size = Mathf.Max(rows, columns);
            float width = ((columns/max_size)*scale);
            float height = ((rows/max_size)*scale);

            NormalizedCartesianCoordinates[] cardinal_boundaries = new NormalizedCartesianCoordinates[4];
            cardinal_boundaries[0] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - width, 0.0f*Mathf.PI);
            cardinal_boundaries[1] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - height, 0.5f*Mathf.PI);
            cardinal_boundaries[2] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - width, 1.0f*Mathf.PI);
            cardinal_boundaries[3] = new NormalizedSphericalCoordinates(1.5f*Mathf.PI - height, 1.5f*Mathf.PI);

            NormalizedCartesianCoordinates[] rectangle_corners = new NormalizedCartesianCoordinates[4];
            for (int side = 0; side < cardinal_boundaries.Length; ++side)
            {
                rectangle_corners[side] = new NormalizedCartesianCoordinates(Vector3.Cross(cardinal_boundaries[side].data, cardinal_boundaries[(side + 1) % 4].data));
            }

            Arc upper_rail = Arc.arc(rectangle_corners[1].data, rectangle_corners[0].data);
            Arc right_rail = Arc.arc(rectangle_corners[0].data, rectangle_corners[3].data);
            Arc lower_rail = Arc.arc(rectangle_corners[2].data, rectangle_corners[3].data);
            Arc left_rail = Arc.arc(rectangle_corners[1].data, rectangle_corners[2].data);

            Arc[] upper_arcs = get_arcs(left_rail, right_rail, rows);
            Arc[] lower_arcs = get_arcs(right_rail, left_rail, rows, true);
            Arc[] right_arcs = get_arcs(upper_rail, lower_rail, columns, true);
            Arc[] left_arcs = get_arcs(lower_rail, upper_rail, columns);
            
            begin_shape();
            BlockRenderer.draw_arc(upper_arcs[0], 0, 1);
            BlockRenderer.draw_arc(right_arcs[columns-1], 0, 1);
            BlockRenderer.draw_arc(lower_arcs[rows-1], 0, 1);
            BlockRenderer.draw_arc(left_arcs[0], 0, 1);
            end_shape(Color.black);

            for (int row = 0; row < rows; ++row)
            {
                //if (row > 0) continue;
                float prior_row = row/(float)rows;
                float later_row = (row+1)/(float)rows;
                for (int column = 0; column < columns; ++column)
                {
                    //if (column > 0) continue;
                    float prior_column = column/(float)columns;
                    float later_column = (column+1)/(float)columns;
                    begin_shape();
                    BlockRenderer.draw_arc(upper_arcs[row], prior_column, later_column); // upper row
                    BlockRenderer.draw_arc(right_arcs[column], prior_row, later_row); // right column
                    BlockRenderer.draw_arc(lower_arcs[row], (1-later_column), (1-prior_column)); // lower row
                    BlockRenderer.draw_arc(left_arcs[column], (1-later_row), (1-prior_row)); // left column
                    end_shape(pixel_buffer[columns*row + column]);
                }
            }
        }
        write_footer();
    }

    private static Arc[] get_arcs(Arc first_rail, Arc second_rail, int segments, bool offset = false)
    {
        Arc[] arcs = new Arc[segments];
        for (int segment = 0; segment < segments; ++segment)
        {
            float interpolation = (segment+(offset?1:0))/(float)segments;
            arcs[segment] = Arc.arc(first_rail.interpolate(interpolation), second_rail.interpolate(interpolation));
        }
        return arcs;
    }

    public static void end_canvas()
    {
        write_footer();
    }

    public static void end_shape(Color32 color)
    {
        writer.Write(" Z\" fill=\"rgb(" + color.r + "," + color.g + "," + color.b + ")\"/>\n");
    }

    public static TextAsset get_svg() // Unity is gr8; 10/10, would rate again
    {
        TextAsset result = Resources.Load<TextAsset>(resource_location);
        return result;
    }

    private static void write_pixel_art_header(string path)
    {
        writer = new StreamWriter(path); //Replace-able with: write_uv_header();
        writer.Write("<svg width=\"" + scale + "\" height=\"" + scale + "\">\n");
        //writer.Write(" viewBox =\"" + (-scale/2) + " " + (-scale/2) + " " + scale + " " + scale + "\">\n");
    }

    private static void write_uv_header()
    {
        writer = new StreamWriter(svg_path);
        writer.Write("<svg width=\"" + scale + "\" height=\"" + scale + "\">\n");
    }

    private static void write_footer()
    {
        writer.Write("</svg>");
        writer.Flush();
        writer.Close();
        writer.Dispose();
        UnityEditor.AssetDatabase.Refresh();
        string guid = UnityEditor.AssetDatabase.AssetPathToGUID(svg_path);
        writer = new StreamWriter(svg_path.Substring(0, svg_path.Length - 4) + "_" + guid + ".svg");
        resource_location = + scene_index + "/" + scene_index;
        writer.Write(get_svg().text);
        writer.Flush();
        writer.Close();
        writer.Dispose();
        UnityEditor.AssetDatabase.RenameAsset(svg_path, scene_index + "_" + guid);
        resource_location = + scene_index + "/" + scene_index + "_" + guid;
        UnityEditor.AssetDatabase.Refresh();
    }

    private static StreamWriter writer;
    private static int scene_index;
    private static string svg_folder_path = Application.dataPath + "/Planetaria/Art/VectorGraphics/Resources/";
    private static string svg_relative_folder_path = "Assets/Planetaria/Art/VectorGraphics/Resources/";
    private static string svg_path; // .svg, oh Unity
    private static string resource_location;
    private static int scale = 1024;
    private static bool first = true;
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