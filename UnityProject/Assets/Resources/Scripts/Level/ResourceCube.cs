using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class ResizeAbleArray<T>
{
    private T[] array = null;
    private List<T> list = new List<T>();

    private void IncreaseSize()
    {
        T[] tmpArray = new T[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            tmpArray[i] = array[i];
        }
        array = tmpArray;
    }

    private void AddElement(T element)
    {
        list.Add(element);
    }

    #region PublicThings
    public void Add(T element)
    {
        AddElement(element);
    }

    public void Clear()
    {
        list.Clear();
    }

    public int Count
    {
        get
        {
            return list.Count;
        }
    }

    public void GenerateArray()
    {
        array = list.ToArray();
    }

    public T[] Array
    {
        get
        {
            return array;
        }
    }
    #endregion
    
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ResourceCube : MonoBehaviour
{
    public int ResourceAmount = 64;

    public void SetResourceAmount(int newAmount)
    {
        ResourceAmount = newAmount;
        Reset();
        UpdateMesh();
    }

    public void Reset()
    {
        CurrentResources = ResourceAmount;
        renderer.enabled = false;
    }

    public int MineResource(int amount)
    {
        amount = Mathf.Min(CurrentResources, amount);
        CurrentResources -= amount;
        UpdateMesh();
        return amount;
    }

	// Use this for initialization
	void Start () 
    {
        InitMesh();
	}

    private void InitMesh()
    {
        mesh = new Mesh();
        mesh.name = "HEllo";
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        if (MeshNeedsUpdate && !generating)
        {
            ChangeMesh();
        }
    }

    public void MeshUpdate()
    {
        renderer.enabled = true;
    }

    public void ChangeMesh()
    {
        lock (lockArrays)
        {
            ResourceCubeUpdater.Instance.AddUpdater(this, vertices, triangles, uvs, normals, tangents);

            MeshNeedsUpdate = false;
        }
    }

    public MeshFilter meshFilter;

    private void UpdateMesh()
    {
        if (generatingThread != null && generatingThread.IsAlive)
        {
            generatingThread.Abort();
        }

        generating = true;
        MeshNeedsUpdate = true;

        if(mesh == null)
        {
            InitMesh();
        }
        generatingThread = new Thread(GenerateMesh);
        generatingThread.Start();
    }

    #region MeshGeneration
    private bool GetIsAir(int x, int y, int z)
    {
        if (x < 0 || x >= width)
            return true;
        if (z < 0 || z >= length)
            return true;

        if (y > Height)
            return true;
        if (y < 0)
            return false;

        if ((x + z * width + y * (width * length)) >= CurrentResources)
        {
            return true;
        }

        return false;
    }

    private void GenerateTriangles(ResizeAbleArray<int> triangles, int vertexIndex)
    {
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);

        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
    }

    private void GenerateUVs(ResizeAbleArray<Vector2> uvs, int x, int y, int z)
    {
        float uvX = (x % 4) * 0.25f + (y % 4) * 0.25f;
        float uvY = (z % 4) * 0.25f + (y % 4) * 0.25f;

        uvs.Add(new Vector2(uvX, uvY));
        uvs.Add(new Vector2(uvX + 0.25f, uvY));
        uvs.Add(new Vector2(uvX + 0.25f, uvY + 0.25f));
        uvs.Add(new Vector2(uvX, uvY + 0.25f));
    }

    private void GenerateNormals(ResizeAbleArray<Vector3> normals, Vector3 direction)
    {
        normals.Add(direction);
        normals.Add(direction);
        normals.Add(direction);
        normals.Add(direction);
    }

    public volatile bool generating = false;
    public bool MeshNeedsUpdate = false;

    public Thread generatingThread;

    //Generating Thread used variables
    private System.Object lockThis = new System.Object();
    private System.Object lockArrays = new System.Object();

    int Height
    {
        get
        {
            return (int)(CurrentResources / (width * length)) + 1;
        }
    }

    private volatile Mesh mesh;

    private volatile int CurrentResources = 0;
    private volatile int width = 4, length = 4;
    private volatile int lastAmount = 0;

    private volatile ResizeAbleArray<Vector3> vertices = new ResizeAbleArray<Vector3>();
    private volatile ResizeAbleArray<int> triangles = new ResizeAbleArray<int>();
    private volatile ResizeAbleArray<Vector2> uvs = new ResizeAbleArray<Vector2>();
    private volatile ResizeAbleArray<Vector3> normals = new ResizeAbleArray<Vector3>();
    private volatile Vector4[] tangents = null;

    private void GenerateMesh()
    {
        int layerCount = 0;

        if (lastAmount == CurrentResources)
            return;

        lastAmount = CurrentResources;

        if (CurrentResources > 64)
            CurrentResources = 64;
        if (CurrentResources < 0)
            CurrentResources = 0;

        generating = true;

        layerCount = Height;

        lock (lockArrays)
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
            normals.Clear();

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
                            GenerateNormals(normals, Vector3.up);
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
                            GenerateNormals(normals, Vector3.right);
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
                            GenerateNormals(normals, Vector3.left);
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
                            GenerateNormals(normals, Vector3.forward);
                        }

                        //Back
                        if (!GetIsAir(x, layer, z) && GetIsAir(x, layer, z + 1))
                        {
                            vertexIndex = vertices.Count;
                            vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight, diffZ + z * zDiff + zDiff));
                            vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight, diffZ + z * zDiff + zDiff));
                            vertices.Add(new Vector3(diffX + x * xDiff + xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));
                            vertices.Add(new Vector3(diffX + x * xDiff, layer * layerHeight + layerHeight, diffZ + z * zDiff + zDiff));

                            GenerateTriangles(triangles, vertexIndex);
                            GenerateUVs(uvs, x, layer, z);
                            GenerateNormals(normals, Vector3.back);
                        }
                    }
                }
            }

        }

        vertices.GenerateArray();
        triangles.GenerateArray();
        uvs.GenerateArray();
        normals.GenerateArray();

        tangents = GenerateTangents(vertices.Array, triangles.Array, uvs.Array, normals.Array);

        generating = false;
    }

    public Vector4[] GenerateTangents(Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3[] normals)
    {
        Vector3[] tan2 = new Vector3[vertices.Length];
        Vector3[] tan1 = new Vector3[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        //Vector3[] binormal = new Vector3[mesh.vertices.Length]; 
        for (int a = 0; a < (triangles.Length); a += 3)
        {
            long i1 = triangles[a + 0];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uvs[i1];
            Vector2 w2 = uvs[i2];
            Vector2 w3 = uvs[i3];

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

        for (int a = 0; a < vertices.Length; a++)
        {
            Vector3 n = normals[a];
            Vector3 t = tan1[a];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;

            // Calculate handedness
            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }
        return tangents;
    }

    public static void TangentSolver(Mesh mesh)
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
