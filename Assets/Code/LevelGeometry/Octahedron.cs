using UnityEngine;

public static class Octahedron
{
    /// <summary>
    /// Singleton - Fetch the octahedron mesh.
    /// </summary>
    /// <returns>The Mesh for an octahedron.</returns>
    public static Mesh OctahedronMesh()
    {
        if (octahedron.exists)
        {
            return octahedron.data;
        }
        else
        {
            octahedron = new Mesh();

            octahedron.data.vertices = InitializeVertices();
            octahedron.data.uv = InitializeUVs();
            octahedron.data.triangles = InitializeTriangles();

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
    public static Vector2 Cartesian_to_UV(Vector3 octahedral)
    {
        Vector2 result = Vector2.zero;

        bool[] bSign = new bool[3];

        bSign[0] = octahedral.x < 0;
        bSign[1] = octahedral.y < 0;
        bSign[2] = octahedral.z < 0;

        int triangle_start_index = (bSign[0] ? 1 : 0) + (bSign[1] ? 2 : 0)  + (bSign[2] ? 4 : 0); // get value from 0-7 (x sign is 1s place, y sign is 2s place, z sign is 4s place)
     
        Mesh mesh = OctahedronMesh();

        for (int vertex = 0; vertex < 3; ++vertex)
        {
            int begin_edge = mesh.triangles[triangle_start_index + vertex];
            int end_edge = mesh.triangles[triangle_start_index + (vertex + 1) % 3];

            Vector2 begin_UV = mesh.uv[begin_edge];
            Vector2 end_UV = mesh.uv[end_edge];

            Vector3 begin_Cartesian = mesh.vertices[begin_edge];
            Vector3 end_Cartesian = mesh.vertices[end_edge];

            float dot_product = Vector2.Dot((octahedral - begin_Cartesian).normalized, (end_Cartesian - begin_Cartesian).normalized);
            Vector2 UV_vector = end_UV - begin_UV;
            result += UV_vector*dot_product;
        }

        return result;
    }

    /// <summary>
    /// Inspector - Use Barycentric coordinates to convert from octahedral UV coordinates to octahedral coordinates.
    /// </summary>
    /// <param name="UV">The UV coordinates of the octahedron.</param>
    /// <returns>The octahedral coordinates.</returns>
    public static Vector3 UV_to_Cartesian(Vector2 UV)
    {
        Vector3 result = Vector3.zero;

        bool[] bSign = new bool[3];

        bSign[0] = UV.x < 0.5f; // x < 0?
        bSign[1] = Mathf.Abs(UV.x) + Mathf.Abs(UV.y) > 1; // y < 0?
        bSign[2] = UV.y < 0.5f; // z < 0?

        int triangle_start_index = (bSign[0] ? 1 : 0) + (bSign[1] ? 2 : 0)  + (bSign[2] ? 4 : 0); // get value from 0-7 (x sign is 1s place, y sign is 2s place, z sign is 4s place)
        
        Mesh mesh = OctahedronMesh();

        for (int vertex = 0; vertex < 3; ++vertex)
        {
            int begin_edge = mesh.triangles[triangle_start_index + vertex];
            int end_edge = mesh.triangles[triangle_start_index + (vertex + 1) % 3];

            Vector2 begin_UV = mesh.uv[begin_edge];
            Vector2 end_UV = mesh.uv[end_edge];

            Vector3 begin_Cartesian = mesh.vertices[begin_edge];
            Vector3 end_Cartesian = mesh.vertices[end_edge];

            float dot_product = Vector2.Dot((UV - begin_UV).normalized, (end_UV - begin_UV).normalized);
            Vector3 Cartesian_vector = end_Cartesian - begin_Cartesian;
            result += Cartesian_vector*dot_product;
        }

        return result;
    }

    static optional<Mesh> octahedron;

    /// <summary>
    /// Inspector - Set up the six mesh corners (vertices) of the octahedron.
    /// </summary>
    /// <returns>List of vertices for an octahedron.</returns>
    static Vector3[] InitializeVertices()
    {
        Vector3[] vertices = new Vector3[9];

        vertices[0] = Vector3.up;
        vertices[1] = Vector3.right;
        vertices[2] = Vector3.forward;
        vertices[3] = Vector3.left;
        vertices[4] = Vector3.back;
        vertices[5] = Vector3.down;
        vertices[6] = Vector3.down;
        vertices[7] = Vector3.down;
        vertices[8] = Vector3.down;

        return vertices;
    }

    /// <summary>
    /// Inspector - Set up the UVs for the six mesh corners (vertices) of the octahedron.
    /// </summary>
    /// <returns>List of UVs for an octahedron.</returns>
    static Vector2[] InitializeUVs()
    {
        Vector2[] UVs = new Vector2[9];

        UVs[0] = new Vector2(0.5f, 0.5f); // up
        UVs[1] = new Vector2(1.0f, 0.5f); // right
        UVs[2] = new Vector2(0.5f, 0.0f); // forward
        UVs[3] = new Vector2(0.0f, 0.5f); // left
        UVs[4] = new Vector2(0.5f, 1.0f); // back
        UVs[5] = new Vector2(1.0f, 0.0f); // down
        UVs[6] = new Vector2(0.0f, 0.0f); // down
        UVs[7] = new Vector2(0.0f, 1.0f); // down
        UVs[8] = new Vector2(1.0f, 1.0f); // down

        return UVs;
    }

    /// <summary>
    /// Inspector - Set up the triplet indices that define the triangles of the octahedron.
    /// </summary>
    /// <returns>A list of index triplets that define the triangles in the octahedron.</returns>
    static int[] InitializeTriangles()
    {
        int[] triangles = new int[3 * 8];

        triangles[0]  = 0; triangles[1]  = 1; triangles[2]  = 2; //+x, +y, +z
        triangles[3]  = 0; triangles[4]  = 2; triangles[5]  = 3; //-x, +y, +z
        triangles[6] = 5; triangles[7] = 2; triangles[8] = 1; //+x, -y, +z
        triangles[9] = 6; triangles[10] = 3; triangles[11] = 2; //-x, -y, +z
        triangles[12]  = 0; triangles[13] = 4; triangles[14] = 1; //+x, +y, -z
        triangles[15]  = 0; triangles[16]  = 3; triangles[17]  = 4; //-x, +y, -z
        triangles[18] = 8; triangles[19] = 1; triangles[20] = 4; //+x, -y, -z
        triangles[21] = 7; triangles[22] = 4; triangles[23] = 3; //-x, -y, -z

        return triangles;
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