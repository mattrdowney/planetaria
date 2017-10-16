using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class VectorGraphicsWriter // FIXME: TODO: clean this up! // CONSIDER: make into an abstract interface? // Parameters for "edge creation" get weird if an interface is used
{
    public static void begin_shape()
    {
        scene_index = SceneManager.GetActiveScene().buildIndex;
        if (!Directory.Exists(svg_folder_path + "/" + scene_index))
        {
            Directory.CreateDirectory(svg_folder_path + "/" + scene_index);
        }
        svg_path = svg_relative_folder_path + scene_index + "/" + scene_index + ".txt";
        write_uv_header();
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

            StereoscopicProjectionCoordinates[,] pixel_grid_points = new StereoscopicProjectionCoordinates[rows+1,columns+1];
            for (int row = 0; row <= rows; ++row) // <= because of rows+1 size
            {
                float elevation = Mathf.Lerp(0.5f, -0.5f, row/(float)rows) * height + Mathf.PI/2;
                for (int column = 0; column <= columns; ++column)
                {
                    float angle = Mathf.Lerp(-0.5f, 0.5f, column/(float)columns) * width;
                    pixel_grid_points[row,column] = new NormalizedSphericalCoordinates(elevation, angle);
                }
            }
            for (int row = 0; row < rows; ++row)
            {
                for (int column = 0; column < columns; ++column)
                {
                    set_pixel(pixel_grid_points[row,column].data, pixel_grid_points[row,column+1].data,
                            pixel_grid_points[row+1,column].data, pixel_grid_points[row+1,column+1].data,
                            pixel_buffer[row*columns + column]);
                }
            }
        }
        write_footer();
    }

    private static void set_pixel(Vector2 top_left, Vector2 top_right, Vector2 bottom_right, Vector2 bottom_left, Color32 color)
    {
        top_left += Vector2.one/2;
        top_right += Vector2.one/2;
        bottom_right += Vector2.one/2;
        bottom_left += Vector2.one/2;

        top_left = new Vector2(-top_left.x, top_left.y);
        top_right = new Vector2(-top_right.x, top_right.y);
        bottom_right = new Vector2(-bottom_right.x, bottom_right.y);
        bottom_left = new Vector2(-bottom_left.x, bottom_left.y);

        writer.Write("\t<path d=\"");
        writer.Write("M" + top_left.x * scale + "," + top_left.y * scale);
        writer.Write(" L " + top_right.x * scale + "," + top_right.y * scale);
        writer.Write(" L " + bottom_right.x * scale + "," + bottom_right.y * scale);
        writer.Write(" L " + bottom_left.x * scale + "," + bottom_left.y * scale);
        writer.Write(" Z\" fill=\"rgb(" + color.r + "," + color.g + "," + color.b + ")\"/>\n");
    }

    public static void end_shape()
    {
        writer.Write(" Z\" fill=\"black\"/>\n");
        write_footer();
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