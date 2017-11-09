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
        string name = scene_index + "/" + scene_index;
        write_header(name);
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
        string name = scene_index + "/" + scene_index;
        write_footer(name);
    }

    public static void end_shape(Color32 color)
    {
        writer.Write(" Z\" fill=\"rgb(" + color.r + "," + color.g + "," + color.b + ")\"/>\n");
    }

    public static TextAsset get_svg()
    {
        TextAsset result = Resources.Load<TextAsset>(resource_location);
        return result;
    }

    public static void write_header(string identifier)
    {
        writer = new StreamWriter(svg_relative_folder_path + identifier + ".txt");
        writer.Write("<svg width=\"" + scale + "\" height=\"" + scale + "\">\n");
    }

    public static void write_footer(string identifier, bool add_guid) // TODO: simplify
    {
        string suffix = "";
        string path = svg_relative_folder_path + identifier;
        if (add_guid)
        {
            string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path + ".txt");
            suffix = "_" + guid;
        }
        writer.Write("</svg>");
        writer.Dispose();
        UnityEditor.AssetDatabase.Refresh();
        writer = new StreamWriter(path + suffix + ".svg");
        resource_location = identifier;
        writer.Write(get_svg().text);
        writer.Dispose();
        if (add_guid)
        {
            UnityEditor.AssetDatabase.RenameAsset(path + ".txt", identifier.Substring(identifier.LastIndexOf('/') + 1) + suffix);
        }
        resource_location = identifier + suffix;
        UnityEditor.AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// Mutator (Operating System level) - Writes (or overwrites!) the file at location "file" with the string "text_contents". 
    /// </summary>
    /// <param name="file">The absolute file path from the operating system's root directory (e.g. "C:/ProgramData").</param>
    /// <param name="text_contents">The contents that will be placed in the file</param>
    public static void write_file(string file, string text_contents, bool add_guid)
    {
        // FIXME: relative file path? :: private static string svg_folder_path = Application.dataPath + "/Planetaria/Art/VectorGraphics/Resources/";
        // file = Application.dataPath + "/Planetaria/Art/VectorGraphics/Resources/" + file; 
        using (StreamWriter writer = new StreamWriter(file, false))
        {
        string suffix = "";
        string path = svg_relative_folder_path + identifier;
        if (add_guid)
        {
            string guid = UnityEditor.AssetDatabase.AssetPathToGUID(path + ".txt");
            suffix = "_" + guid;
        }
        writer.Write("</svg>");
        writer.Dispose();
        UnityEditor.AssetDatabase.Refresh();
        writer = new StreamWriter(path + suffix + ".svg");
        resource_location = identifier;
        writer.Write(get_svg().text);
        writer.Dispose();
        if (add_guid)
        {
            UnityEditor.AssetDatabase.RenameAsset(path + ".txt", identifier.Substring(identifier.LastIndexOf('/') + 1) + suffix);
        }
        resource_location = identifier + suffix;
        UnityEditor.AssetDatabase.Refresh();
    }

    private static StreamWriter writer;
    private static int scene_index;
    private static string svg_folder_path = Application.dataPath + "/Planetaria/Art/VectorGraphics/Resources/";
    private static string svg_relative_folder_path = "Assets/Planetaria/Art/VectorGraphics/Resources/";
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