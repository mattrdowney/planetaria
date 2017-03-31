using UnityEngine;

public class Octahedron : MonoBehaviour
{
    public void Start()
    {
        this.GetComponent<MeshFilter>().mesh = OctahedronMesh();
    }

    static optional<Mesh> octahedron;

	public static Mesh OctahedronMesh()
    {
        if (octahedron.exists)
        {
            return octahedron.data;
        }
        else
        {
            octahedron = new Mesh();

            Vector3[] verts = new Vector3[9];
            Vector2[] uvs   = new Vector2[9];
            int[]     tris  = new int[3 * 8];

            verts[0] = Vector3.up;
            verts[1] = Vector3.right;
            verts[2] = Vector3.forward;
            verts[3] = Vector3.left;
            verts[4] = Vector3.back;
            verts[5] = Vector3.down;
            verts[6] = Vector3.down;
            verts[7] = Vector3.down;
            verts[8] = Vector3.down;

            uvs[0] = new Vector2(0.5f, 0.5f); // up
            uvs[1] = new Vector2(1.0f, 0.5f); // right
            uvs[2] = new Vector2(0.5f, 0.0f); // forward
            uvs[3] = new Vector2(0.0f, 0.5f); // left
            uvs[4] = new Vector2(0.5f, 1.0f); // back
            uvs[5] = new Vector2(1.0f, 0.0f); // down
            uvs[6] = new Vector2(0.0f, 0.0f); // down
            uvs[7] = new Vector2(0.0f, 1.0f); // down
            uvs[8] = new Vector2(1.0f, 1.0f); // down

            tris[0]  = 0; tris[1]  = 1; tris[2]  = 2; //+x, +y, +z
            tris[3]  = 0; tris[4]  = 2; tris[5]  = 3; //-x, +y, +z
            tris[6]  = 0; tris[7]  = 3; tris[8]  = 4; //-x, +y, -z
            tris[9]  = 0; tris[10] = 4; tris[11] = 1; //+x, +y, -z
            tris[12] = 5; tris[13] = 2; tris[14] = 1; //+x, -y, +z
            tris[15] = 6; tris[16] = 3; tris[17] = 2; //-x, -y, +z
            tris[18] = 7; tris[19] = 4; tris[20] = 3; //-x, -y, -z
            tris[21] = 8; tris[22] = 1; tris[23] = 4; //+x, -y, -z

            octahedron.data.vertices = verts;
            octahedron.data.uv = uvs;
            octahedron.data.triangles = tris;

            octahedron.data.RecalculateBounds();
            octahedron.data.RecalculateNormals();
            ;

            return octahedron.data;
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