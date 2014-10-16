using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ResourceCube : MonoBehaviour
{
    public int ResourceAmount = 64;

    private int CurrentResources = 0;

    private int lastAmount = 0;

    int width = 4, length = 4;

    int Height
    {
        get
        {
            return (int)(CurrentResources / (width * length)) + 1;
        }
    }

    public void SetResourceAmount(int newAmount)
    {
        ResourceAmount = newAmount;
        Reset();
        GenerateMesh();
    }

    public void Reset()
    {
        CurrentResources = ResourceAmount;
    }

    public int MineResource(int amount)
    {
        amount = Mathf.Min(CurrentResources, amount);
        CurrentResources -= amount;
        GenerateMesh();
        return amount;
    }

	// Use this for initialization
	void Start () 
    {
        Reset();
        GenerateMesh();
	}

    #region MeshGeneration
    public bool GetIsAir(int x, int y, int z)
    {
        if (x < 0 || x >= width)
            return true;
        if (z < 0 || z >= length)
            return true;

        if (y > Height)
            return true;
        if (y < 0)
            return false;

        if ((x + z * width + y * (width * length)) >= ResourceAmount)
        {
            return true;
        }

        return false;
    }

    private void GenerateTriangles(List<int> triangles, int vertexIndex)
    {
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);

        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
    }

    private void GenerateUVs(List<Vector2> uvs, int x, int y, int z)
    {
        float uvX = (x % 4) * 0.25f + (y % 4) * 0.25f;
        float uvY = (z % 4) * 0.25f + (y % 4) * 0.25f;

        uvs.Add(new Vector2(uvX, uvY));
        uvs.Add(new Vector2(uvX + 0.25f, uvY));
        uvs.Add(new Vector2(uvX + 0.25f, uvY + 0.25f));
        uvs.Add(new Vector2(uvX, uvY + 0.25f));
    }

    [ContextMenu("RecreateMesh")]
    void GenerateMesh()
    {
        if (lastAmount == ResourceAmount)
            return;

        lastAmount = ResourceAmount;

        if (ResourceAmount > 64)
            ResourceAmount = 64;
        if (ResourceAmount < 0)
            ResourceAmount = 0;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int layerCount = Height;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float diffX = -0.4f;
        float diffZ = -0.4f;

        float xDiff = 0.2f;
        float zDiff = 0.2f;
        float layerHeight = 0.2f;

        int vertexIndex = vertices.Count;

        for (int layer = 0; layer < layerCount; layer++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (!GetIsAir(x, layer, z) && GetIsAir(x, layer + 1, z)) //TOP
                    {
                        vertexIndex = vertices.Count;
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff));

                        GenerateTriangles(triangles, vertexIndex);
                        GenerateUVs(uvs, x, layer, z);
                    }

                    //Right
                    if (!GetIsAir(x, layer, z) && GetIsAir(x + 1, layer, z))
                    {
                        vertexIndex = vertices.Count;
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight, diffZ + z * zDiff));

                        GenerateTriangles(triangles, vertexIndex);
                        GenerateUVs(uvs, x, layer, z);
                    }

                    //Left
                    if (!GetIsAir(x, layer, z) && GetIsAir(x - 1, layer, z))
                    {
                        vertexIndex = vertices.Count;
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight, diffZ + z * zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff));

                        GenerateTriangles(triangles, vertexIndex);
                        GenerateUVs(uvs, x, layer, z);
                    }

                    //Front
                    if (!GetIsAir(x, layer, z) && GetIsAir(x, layer, z - 1))
                    {
                        vertexIndex = vertices.Count;
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight, diffZ + z * zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight, diffZ + z * zDiff));

                        GenerateTriangles(triangles, vertexIndex);
                        GenerateUVs(uvs, x, layer, z);
                    }

                    //Front
                    if (!GetIsAir(x, layer, z) && GetIsAir(x, layer, z + 1))
                    {
                        vertexIndex = vertices.Count;
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));
                        vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));

                        GenerateTriangles(triangles, vertexIndex);
                        GenerateUVs(uvs, x, layer, z);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        TangentSolver(mesh);

        mesh.Optimize();
    }

    public void TangentSolver(Mesh mesh)
    {
        Vector3[] tan2 = new Vector3[mesh.vertices.Length];
        Vector3[] tan1 = new Vector3[mesh.vertices.Length];
        Vector4[] tangents = new Vector4[mesh.vertices.Length];
        //Vector3[] binormal = new Vector3[mesh.vertices.Length]; 
        for (int a = 0; a < (mesh.triangles.Length); a += 3)
        {
            long i1 = mesh.triangles[a + 0];
            long i2 = mesh.triangles[a + 1];
            long i3 = mesh.triangles[a + 2];

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

            float r = 1.0F / (s1 * t2 - s2 * t1);
            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for (int a = 0; a < mesh.vertices.Length; a++)
        {
            Vector3 n = mesh.normals[a];
            Vector3 t = tan1[a];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            // Calculate handedness
            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;

            //To calculate binormals if required as vector3 try one of below:-
            //Vector3 binormal[a] = (Vector3.Cross(n, t) * tangents[a].w).normalized;
            //Vector3 binormal[a] = Vector3.Normalize(Vector3.Cross(n, t) * tangents[a].w)
        }
        mesh.tangents = tangents;
    }
    #endregion

}
