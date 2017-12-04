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

        /// <summary>
        /// The Sphere collider that envelops a axial "point" with a given planetaria_radius.
        /// </summary>
        /// <param name="transformation">An optional Transform that will be used to shift the center (if moved).</param>
        /// <param name="axis">The center of the circle (both a point and an axis).</param>
        /// <param name="planetaria_radius">The angle along the surface from circle center to circle boundary.</param>
        /// <returns>A Sphere that can be used as a collider.</returns>
        public static Sphere collider(optional<Transform> transformation, Vector3 axis, float planetaria_radius)
        {
            // Background:
            // imagine two spheres:
            // "left" sphere is of radius 1 and placed at the origin (0,0,0).
            // "right" sphere is of radius 2 and is placed at (3,0,0).
            // These spheres intersect at a point (1,0,0).
            // If slid closer together they will intersect at a circle of radius (0,1].
            // The goal is to find the distance between spheres to create a collider along a given circle.

            // Derivation:
            // I created four variables:
            // left_height = sin(left_angle)
            // left_distance = 1 - cos(left_angle)
            // right_height = 2sin(right_angle)
            // right_distance = 2 - 2cos(right_angle)

            // While we are trying to find 3 - sum(distances), we know that left_height must equal right_height for the intersection to be valid
            // Therefore sin(left_angle) = left_height = right_height = 2sin(right_angle)
            // Since left_angle is the planetaria_radius (because we are dealing with a unit sphere) we can solve for right_angle:
            // right_angle = arcsin(sin(left_angle)/2)
            
            // Now that we know right_angle and left_angle we can solve all formulas:
            // left_distance = 1 - cos(left_angle)
            // right_distance = 2 - 2cos(arcsin(sin(left_angle)/2))
            // Next we just subtract these differences from 3 (the radii sum):
            // axial_distance = 3 - [1 - cos(left_angle)] - [2 - 2cos(arcsin(sin(left_angle)/2))]
            // axial_distance = 3 - 1 + cos(left_angle) - 2 + 2cos(arcsin(sin(left_angle)/2))
            // axial_distance = cos(left_angle) + 2cos(arcsin(sin(left_angle)/2))
            // which, according to Wolfram Alpha is: cos(left_angle) + sqrt(sin2(left_angle) + 3)

            planetaria_radius += Precision.collider_extrusion; // Collisions can theoretically (and practically) be missed due to rounding errors.

            if (planetaria_radius > Mathf.PI/2) // Flip to other hemisphere when circle is on the other hemisphere
            {
                planetaria_radius = Mathf.PI - planetaria_radius;
                axis *= -1;
            }

            float x = Mathf.Cos(planetaria_radius);
            float axial_distance = x + Mathf.Sqrt(x*x + 3);
            return Sphere.sphere(transformation, axis*axial_distance, 2);
        }
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