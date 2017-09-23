using UnityEngine;

public static class Octahedron
{
    /// <summary>
    /// Singleton - Fetch the octahedron mesh.
    /// </summary>
    /// <returns>The Mesh for an octahedron.</returns>
    public static Mesh octahedron_mesh()
    {
        if (octahedron.exists)
        {
            return octahedron.data;
        }
        else
        {
            octahedron = new Mesh();

            octahedron.data.vertices = initialize_vertex_array();
            octahedron.data.uv = initialize_uv_array();
            octahedron.data.triangles = initialize_triangle_array();

            octahedron.data.RecalculateBounds();
            octahedron.data.RecalculateNormals();

            return octahedron.data;
        }
    }

    /// <summary>
    /// Inspector - Use Barycentric coordinates to convert from octahedral coordinates to octahedral UV coordinates.
    /// </summary>
    /// <param name="octahedral">The octahedral coordinates.</param>
    /// <returns>The UV coordinates of an octahedron.</returns>
    public static Vector2 cartesian_to_uv(Vector3 octahedral)
    {
        // get mapping index from 0-7
        int xyz_mask =
                (octahedral.x < 0 ? 1 : 0) + // x sign is 1s place
                (octahedral.y < 0 ? 2 : 0) + // y sign is 2s place
                (octahedral.z < 0 ? 4 : 0);  // z sign is 4s place

        return convert(octahedron_mesh().vertices, octahedron_mesh().uv, octahedral, xyz_mask);
    }

    /// <summary>
    /// Inspector - Use Barycentric coordinates to convert from octahedral UV coordinates to octahedral coordinates.
    /// </summary>
    /// <param name="uv">The UV coordinates of the octahedron.</param>
    /// <returns>The octahedral coordinates.</returns>
    public static Vector3 uv_to_cartesian(Vector2 uv)
    {
        // get mapping index from 0-7
        int xyz_mask =
                (uv.x < 0.5f ? 1 : 0) + // x sign is 1s place
                (Mathf.Abs(uv.x) + Mathf.Abs(uv.y) > 1 ? 2 : 0) + // y sign is 2s place
                (uv.y < 0.5f ? 4 : 0);  // z sign is 4s place

        return convert(octahedron_mesh().vertices, octahedron_mesh().uv, uv, xyz_mask);
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="From"></typeparam>
    /// <typeparam name="To"></typeparam>
    /// <param name="from_array"></param>
    /// <param name="to_array"></param>
    /// <param name="from"></param>
    /// <param name="xyz_mask">Number from [0,7] inclusive. 1's place => negative x value, 2's => -y, 4's => -z. </param>
    /// <returns></returns>
    private static To convert<From, To>(From[] from_array, To[] to_array, Vector3 from, int xyz_mask) // FIXME: less than ideal, prefer using From[] To[] (supplied as parameters) // but until C# gets on their shit a la template metaprogramming / https://stackoverflow.com/questions/8188784/how-can-i-subtract-two-generic-objects-t-t-in-c-sharp-example-datetime-d this is in limbo // YESH workaround
    {
        Vector3 to = Vector3.zero;

        int triangle_start_index = xyz_mask * 3; // there are 24 indices in the octahedron mesh (i.e. 3 triangle vertices times 8 faces)

        Mesh mesh = octahedron_mesh();

        for (int vertex = 0; vertex < 3; ++vertex)
        {
            int begin_edge = mesh.triangles[triangle_start_index + vertex];
            int end_edge = mesh.triangles[triangle_start_index + (vertex + 1) % 3];

            Vector3 from_begin = get_vector(from_array[begin_edge]); // Rather than From, From, From...
            Vector3 from_end = get_vector(from_array[end_edge]);
            Vector3 from_vector = from_end - from_begin;

            Vector3 to_begin = get_vector(to_array[begin_edge]); // ... and To, To, To...
            Vector3 to_end = get_vector(to_array[end_edge]);
            Vector3 to_vector = to_end - to_begin;

            Vector3 from_position = get_vector(from) - from_begin; // ... This is a little workaround-y
            float dot_product = Vector3.Dot(from_position, from_vector) / from_vector.sqrMagnitude;
            to += to_vector*dot_product;
        }

        return set_vector<To>(to);
    }

    private static Vector3 get_vector<Vector>(Vector vector) // HACK: (I still think this makes the code easier to read than the alternative)
    {
        //return typeof(Vector) == typeof(Vector3) ? (Vector3)(object) vector : (Vector2)(object) vector; // Note: FIXME: this seems to be a Unity/VS/C#/idk bug

        if (typeof(Vector) == typeof(Vector3))
        {
            return (Vector3)(object) vector;
        }
        else
        {
            return (Vector2)(object) vector;
        }
    }

    private static Vector set_vector<Vector>(Vector3 vector) // HACK: 
    {
        if (typeof(Vector) == typeof(Vector3))
        {
            return (Vector) (object) vector;
        }
        else
        {
            return (Vector) (object) new Vector2(vector.x, vector.y);
        }
    }

    /// <summary>
    /// Inspector - Set up the six mesh corners (vertices) of the octahedron.
    /// </summary>
    /// <returns>List of vertices for an octahedron.</returns>
    private static Vector3[] initialize_vertex_array()
    {
        Vector3[] vertex_array = new Vector3[9];

        vertex_array[0] = Vector3.up;
        vertex_array[1] = Vector3.right;
        vertex_array[2] = Vector3.forward;
        vertex_array[3] = Vector3.left;
        vertex_array[4] = Vector3.back;
        vertex_array[5] = Vector3.down;
        vertex_array[6] = Vector3.down;
        vertex_array[7] = Vector3.down;
        vertex_array[8] = Vector3.down;

        return vertex_array;
    }

    /// <summary>
    /// Inspector - Set up the UVs for the six mesh corners (vertices) of the octahedron.
    /// </summary>
    /// <returns>List of UVs for an octahedron.</returns>
    private static Vector2[] initialize_uv_array()
    {
        Vector2[] uv_array = new Vector2[9];

        uv_array[0] = new Vector2(0.5f, 0.5f); // up
        uv_array[1] = new Vector2(1.0f, 0.5f); // right
        uv_array[2] = new Vector2(0.5f, 0.0f); // forward
        uv_array[3] = new Vector2(0.0f, 0.5f); // left
        uv_array[4] = new Vector2(0.5f, 1.0f); // back
        uv_array[5] = new Vector2(1.0f, 0.0f); // down
        uv_array[6] = new Vector2(0.0f, 0.0f); // down
        uv_array[7] = new Vector2(0.0f, 1.0f); // down
        uv_array[8] = new Vector2(1.0f, 1.0f); // down

        return uv_array;
    }

    /// <summary>
    /// Inspector - Set up the triplet indices that define the triangles of the octahedron.
    /// </summary>
    /// <returns>A list of index triplets that define the triangles in the octahedron.</returns>
    private static int[] initialize_triangle_array()
    {
        int[] triangle_array = new int[3 * 8];

        triangle_array[0]  = 0; triangle_array[1]  = 1; triangle_array[2]  = 2; //+x, +y, +z
        triangle_array[3]  = 0; triangle_array[4]  = 2; triangle_array[5]  = 3; //-x, +y, +z
        triangle_array[6] = 5; triangle_array[7] = 2; triangle_array[8] = 1; //+x, -y, +z
        triangle_array[9] = 6; triangle_array[10] = 3; triangle_array[11] = 2; //-x, -y, +z
        triangle_array[12]  = 0; triangle_array[13] = 4; triangle_array[14] = 1; //+x, +y, -z
        triangle_array[15]  = 0; triangle_array[16]  = 3; triangle_array[17]  = 4; //-x, +y, -z
        triangle_array[18] = 8; triangle_array[19] = 1; triangle_array[20] = 4; //+x, -y, -z
        triangle_array[21] = 7; triangle_array[22] = 4; triangle_array[23] = 3; //-x, -y, -z

        return triangle_array;
    }

    private static optional<Mesh> octahedron;
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