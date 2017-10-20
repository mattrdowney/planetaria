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

    public static void write_pixel_art_header(string path)
    {
        writer = new StreamWriter(path); //Replace-able with: write_uv_header();
        writer.Write("<svg width=\"" + scale + "\" height=\"" + scale + "\">\n");
        //writer.Write(" viewBox =\"" + (-scale/2) + " " + (-scale/2) + " " + scale + " " + scale + "\">\n");
    }

    public static void write_uv_header()
    {
        writer = new StreamWriter(svg_path);
        writer.Write("<svg width=\"" + scale + "\" height=\"" + scale + "\">\n");
    }

    public static void write_footer()
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