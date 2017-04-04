using UnityEngine;

public static class Octahedron
{
    static optional<Mesh> octahedron;

    static Vector3[] vertices;
    static int[] triangles;
    static Vector2[] UVs;
    static Vector2[,,,] BarycentricUVs;
    static Vector3[,,,] BarycentricCartesian;
    
    static Vector3[] InitializeVertices()
    {
        vertices = new Vector3[9];

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

    static Vector2[] InitializeUVs()
    {
        UVs = new Vector2[9];

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

    static int[] InitializeTriangles()
    {
        triangles = new int[3 * 8];

        triangles[0]  = 0; triangles[1]  = 1; triangles[2]  = 2; //+x, +y, +z
        triangles[3]  = 0; triangles[4]  = 2; triangles[5]  = 3; //-x, +y, +z
        triangles[6]  = 0; triangles[7]  = 3; triangles[8]  = 4; //-x, +y, -z
        triangles[9]  = 0; triangles[10] = 4; triangles[11] = 1; //+x, +y, -z
        triangles[12] = 5; triangles[13] = 2; triangles[14] = 1; //+x, -y, +z
        triangles[15] = 6; triangles[16] = 3; triangles[17] = 2; //-x, -y, +z
        triangles[18] = 7; triangles[19] = 4; triangles[20] = 3; //-x, -y, -z
        triangles[21] = 8; triangles[22] = 1; triangles[23] = 4; //+x, -y, -z

        return triangles;
    }

    static Vector2[,,,] InitializeBarycentricUVs()
    {
        BarycentricUVs = new Vector2[2,2,2,3];

        // +x, +y, +z
        BarycentricUVs[0,0,0,0] = new Vector2(1.0f, 1.0f);
        BarycentricUVs[0,0,0,1] = new Vector2(0.5f, 1.0f);
        BarycentricUVs[0,0,0,2] = new Vector2(1.0f, 0.5f);

        // -x, +y, +z
        BarycentricUVs[1,0,0,0] = new Vector2(0.0f, 1.0f);
        BarycentricUVs[1,0,0,1] = new Vector2(0.0f, 0.5f);
        BarycentricUVs[1,0,0,2] = new Vector2(0.5f, 1.0f);

        // +x, -y, +z
        BarycentricUVs[0,1,0,0] = new Vector2(0.5f, 0.5f);
        BarycentricUVs[0,1,0,1] = new Vector2(1.0f, 0.5f);
        BarycentricUVs[0,1,0,2] = new Vector2(0.5f, 1.0f);

        // -x, -y, +z
        BarycentricUVs[1,1,0,0] = new Vector2(0.5f, 0.5f);
        BarycentricUVs[1,1,0,1] = new Vector2(0.5f, 1.0f);
        BarycentricUVs[1,1,0,2] = new Vector2(0.0f, 0.5f);

        // +x, +y, -z
        BarycentricUVs[0,0,1,0] = new Vector2(1.0f, 0.0f);
        BarycentricUVs[0,0,1,1] = new Vector2(1.0f, 0.5f);
        BarycentricUVs[0,0,1,2] = new Vector2(0.5f, 0.0f);

        // -x, +y, -z
        BarycentricUVs[1,0,1,0] = new Vector2(0.0f, 0.0f);
        BarycentricUVs[1,0,1,1] = new Vector2(0.5f, 0.0f);
        BarycentricUVs[1,0,1,2] = new Vector2(0.0f, 0.5f);

        // +x, -y, -z
        BarycentricUVs[0,1,1,0] = new Vector2(0.5f, 0.5f);
        BarycentricUVs[0,1,1,1] = new Vector2(0.5f, 0.0f);
        BarycentricUVs[0,1,1,2] = new Vector2(1.0f, 0.5f);

        // -x, -y, -z
        BarycentricUVs[1,1,1,0] = new Vector2(0.5f, 0.5f);
        BarycentricUVs[1,1,1,1] = new Vector2(0.0f, 0.5f);
        BarycentricUVs[1,1,1,2] = new Vector2(0.5f, 0.0f);

        return BarycentricUVs;
    }

    static Vector3[,,,] InitializeBarycentricCartesian()
    {
        BarycentricCartesian = new Vector3[2,2,2,3];

        // +x, +y, +z
        BarycentricCartesian[0,0,0,0] = new Vector3(+0.0f, +1.0f, +1.0f);
        BarycentricCartesian[0,0,0,1] = new Vector3(+1.0f, +0.0f, -1.0f);
        BarycentricCartesian[0,0,0,2] = new Vector3(-1.0f, -1.0f, +0.0f);

        // -x, +y, +z
        BarycentricCartesian[1,0,0,0] = new Vector3(-1.0f, +1.0f, +0.0f);
        BarycentricCartesian[1,0,0,1] = new Vector3(+1.0f, +0.0f, +1.0f);
        BarycentricCartesian[1,0,0,2] = new Vector3(+0.0f, -1.0f, -1.0f);

        // +x, -y, +z
        BarycentricCartesian[0,1,0,0] = new Vector3(+1.0f, -1.0f, +0.0f);
        BarycentricCartesian[0,1,0,1] = new Vector3(-1.0f, +0.0f, +1.0f);
        BarycentricCartesian[0,1,0,2] = new Vector3(+0.0f, +1.0f, -1.0f);
        
        // -x, -y, +z
        BarycentricCartesian[1,1,0,0] = new Vector3(+0.0f, -1.0f, +1.0f);
        BarycentricCartesian[1,1,0,1] = new Vector3(-1.0f, +0.0f, -1.0f);
        BarycentricCartesian[1,1,0,2] = new Vector3(+1.0f, +1.0f, +0.0f);
        
        // +x, +y, -z
        BarycentricCartesian[0,0,1,0] = new Vector3(+1.0f, +1.0f, +0.0f);
        BarycentricCartesian[0,0,1,1] = new Vector3(-1.0f, +0.0f, -1.0f);
        BarycentricCartesian[0,0,1,2] = new Vector3(+0.0f, -1.0f, +1.0f);
        
        // -x, +y, -z
        BarycentricCartesian[1,0,1,0] = new Vector3(+0.0f, +1.0f, -1.0f);
        BarycentricCartesian[1,0,1,1] = new Vector3(-1.0f, +0.0f, +1.0f);
        BarycentricCartesian[1,0,1,2] = new Vector3(+1.0f, -1.0f, +0.0f);
        
        // +x, -y, -z
        BarycentricCartesian[0,1,1,0] = new Vector3(+0.0f, -1.0f, -1.0f);
        BarycentricCartesian[0,1,1,1] = new Vector3(+1.0f, +0.0f, +1.0f);
        BarycentricCartesian[0,1,1,2] = new Vector3(-1.0f, +1.0f, +0.0f);
        
        // -x, -y, -z
        BarycentricCartesian[1,1,1,0] = new Vector3(-1.0f, -1.0f, +0.0f);
        BarycentricCartesian[1,1,1,1] = new Vector3(+1.0f, +0.0f, -1.0f);
        BarycentricCartesian[1,1,1,2] = new Vector3(+0.0f, +1.0f, +1.0f);
        
        return BarycentricCartesian;
    }

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
            InitializeBarycentricUVs();
            InitializeBarycentricCartesian();

            octahedron.data.RecalculateBounds();
            octahedron.data.RecalculateNormals();

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