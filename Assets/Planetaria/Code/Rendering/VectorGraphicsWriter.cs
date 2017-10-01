using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class VectorGraphicsWriter
{
    public static void begin_shape()
    {
        write_header();
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

    public static void end_shape()
    {
        writer.Write(" Z\" fill=\"black\"/>\n");
        write_footer();
    }

    public static TextAsset get_svg()
    {
        string location = "0";
        TextAsset result = Resources.Load<TextAsset>(location);
        return result;
    }

    private static void write_header()
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
    }

    private static StreamWriter writer;
    private static string svg_path = Application.dataPath + "/Planetaria/Art/VectorGraphics/Resources/" + SceneManager.GetActiveScene().buildIndex + ".txt"; // .svg, oh Unity
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