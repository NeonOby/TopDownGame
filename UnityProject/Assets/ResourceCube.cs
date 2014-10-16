using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ResourceCube : MonoBehaviour {

    //Max 4x4x4 (64)
    public int ResourceAmount = 64;

    int width = 4, length = 4;

	// Use this for initialization
	void Start () {
        if (ResourceAmount > 64)
            ResourceAmount = 64;
        if (ResourceAmount > 0)
            ResourceAmount = 0;

        Debug.Log(Time.realtimeSinceStartup);
        GenerateMesh();

        Debug.Log("After:" + Time.realtimeSinceStartup);
	}

    public bool GetIsAir(int x, int y, int z)
    {
        if ((x + y * width + z * (width * length)) > ResourceAmount)
        {
            return true;
        }
        return false;
    }

    void GenerateMesh()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int layerCount = (int)(ResourceAmount / (width * length))+1;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int layer = 0; layer < layerCount; layer++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (GetIsAir(x, layer + 1, z))
                    {
                        int vertexIndex = vertices.Count;
                        vertices.Add(new Vector3(x, layer + 1, z));
                        vertices.Add(new Vector3(x, layer + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, layer + 1, z + 1));
                        vertices.Add(new Vector3(x + 1, layer + 1, z));

                        // first triangle for the block top
                        triangles.Add(vertexIndex);
                        triangles.Add(vertexIndex + 1);
                        triangles.Add(vertexIndex + 2);

                        // second triangle for the block top
                        triangles.Add(vertexIndex + 2);
                        triangles.Add(vertexIndex + 3);
                        triangles.Add(vertexIndex);

                        uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(0.25f, 0));
                        uvs.Add(new Vector2(0.25f, 0.25f));
                        uvs.Add(new Vector2(0, 0.25f));
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Solve(mesh);

        mesh.Optimize();
    }
    public static void Solve(Mesh mesh)
    {
        int triangleCount = mesh.triangles.Length / 3;
        int vertexCount = mesh.vertices.Length;
 
        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];
 
        Vector4[] tangents = new Vector4[vertexCount];
 
        for(long a = 0; a < triangleCount; a+=3)
        {
            long i1 = mesh.triangles[a+0];
            long i2 = mesh.triangles[a+1];
            long i3 = mesh.triangles[a+2];
 
            Vector3 v1 = mesh.vertices[i1];
            Vector3 v2 = mesh.vertices[i2];
            Vector3 v3 = mesh.vertices[i3];
 
            Vector2 w1 = mesh.uv[i1];
            Vector2 w2 = mesh.uv[i2];
            Vector2 w3 = mesh.uv[i3];
 
            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;
 
            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;
 
            float r = 1.0f / (s1 * t2 - s2 * t1);
 
            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
 
            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;
 
            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }
 
 
        for (long a = 0; a < vertexCount; ++a)
        {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];
 
            Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
            tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
 
            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }
 
        mesh.tangents = tangents;
    }
}
