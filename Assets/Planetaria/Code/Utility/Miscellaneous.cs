using System.IO;
using System.Linq;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
    /// A group of miscellaneous helper functions.
    /// </summary>
    public static class Miscellaneous
    {
        /// <summary>
        /// Counts the true booleans in a sequence. E.g. count_true_booleans(true, false, true) returns 2.
        /// </summary>
        /// <param name="boolean_list">A comma separated list of (or array of) booleans.</param>
        /// <returns>The number of true values in the sequence.</returns>
        public static int count_true_booleans(params bool[] boolean_list)
        {
           return boolean_list.Count(is_true => is_true);
        }

        public static optional<Texture2D> fetch_image(string image_file)
        {
            optional<Texture2D> texture = new optional<Texture2D>();
            if (File.Exists(image_file))
            {
                 byte[] raw_file_binary = File.ReadAllBytes(image_file);
                 texture = new Texture2D(0,0);
                 texture.data.LoadImage(raw_file_binary);
            }
            return texture;
        }

	    public static Subtype GetOrAddComponent<Subtype>(this Component self) where Subtype : Component
        {
		    optional<Subtype> result = self.GetComponent<Subtype>();
		    if (!result.exists)
            {
			    result = self.gameObject.AddComponent<Subtype>();
		    }
		    return result.data;
	    }

        /// <summary>
        /// Inspector - finds the last instance of inner_text between prefix and suffix, if it exists, and returns it.
        /// </summary>
        /// <param name="text">The text that will be searched.</param>
        /// <param name="prefix">The prefix before the desired text.</param>
        /// <param name="suffix">The suffix after the desired text.</param>
        /// <returns>The last instance of text between prefix and suffix (including neither prefix phrase nor suffix phrase).</returns>
        public static optional<string> inner_text(string text, string prefix, string suffix)
        {
            int end_index = text.LastIndexOf(suffix);
            if (end_index != -1)
            {
                int start_index = text.LastIndexOf(prefix, end_index-1);
                if (start_index != -1)
                {
                    start_index += prefix.Length;
                    int length = end_index - start_index; // off by one? start==0, end==1 (e.g. "A\0") has length (1-0) 
                    return text.Substring(start_index, length);
                }
            }
            return new optional<string>();
        }

        /// <summary>
        /// Mutator (Operating System level) - Writes (or overwrites!) the file at location "file" with the string "text_contents". 
        /// </summary>
        /// <param name="file">The relative file path starting in the Unity directory (e.g. "Assets/important_file.txt"). Note: will be overwritten.</param>
        /// <param name="text_contents">The contents that will be placed in the file (overwrites file).</param>
        /// <param name="add_global_unique_identifier">Adds "_[0-9a-f]{32}" before file extension (last dot) in "file".</param>
        public static optional<TextAsset> write_file(string file, string text_contents, bool add_global_unique_identifier)
        {
            using (StreamWriter writer = new StreamWriter(file, false))
            {
                string suffix = "";
                writer.Write(text_contents);
                if (add_global_unique_identifier)
                {
                    writer.Dispose();
                    UnityEditor.AssetDatabase.Refresh();
                    string global_unique_identifier = UnityEditor.AssetDatabase.AssetPathToGUID(file);
                    suffix = "_" + global_unique_identifier;
                    optional<string> name = Miscellaneous.inner_text(file, "/", ".");
                    if (name.exists)
                    {
                        UnityEditor.AssetDatabase.RenameAsset(file, name.data + suffix); // FIXME: INVESTIGATE: why does optional<string> + string work? (for safety reasons, it shouldn't)
                    }
                    else
                    {
                        Debug.Log("Critical Error");
                    }
                }
                UnityEditor.AssetDatabase.Refresh();
                optional<string> resource = Miscellaneous.inner_text(file, "/Resources/", ".");
                if (resource.exists)
                {
                    resource = resource.data + suffix;
                    return Resources.Load<TextAsset>(resource.data);
                }
                return new optional<TextAsset>();
            }
        }

        // according to: https://www.sjbaker.org/steve/omniv/love_your_z_buffer.html
        // z_buffer_value = z_buffer_digits * ( a + b / z )
        // therefore: distance = b / (z_buffer_value * z_buffer_digits - a);
        public static float z_buffer_to_distance(short z_buffer_value, bool centered = false)
        {
            float z_buffer = z_buffer_value; // z_buffer value is actually an int, but this allows for finding center position
            z_buffer = z_buffer - short.MinValue; // ensure z_buffer is non-negative
            z_buffer = (short.MaxValue - short.MinValue) - z_buffer; // z_buffer has reversed order
            z_buffer += (centered ? 0.5f : 0); // .5f will (typically?) be truncated by z_buffer calculations in certain cases
            return PlanetariaCamera.clip_b / (z_buffer * PlanetariaCamera.z_buffer_digits - PlanetariaCamera.clip_a);
            // TODO: check if results are accidentally inverted
        }

        public static float layer_to_distance(sbyte layer, bool centered = false)
        {
            int byte_layer = layer - (int) sbyte.MinValue;
            int extra_layer = layer_cache[byte_layer];
            if (extra_layer > byte.MaxValue)
            {
                layer_cache[byte_layer] = 0;
            }
            int z_buffer = layer * (byte.MaxValue+1) + extra_layer;
            return z_buffer_to_distance((short)z_buffer, centered);
        }

        static int[] layer_cache = new int[sbyte.MaxValue - sbyte.MinValue + 1];
    }
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