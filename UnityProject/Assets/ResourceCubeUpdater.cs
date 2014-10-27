using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshUpdater
{
    public ResourceCube cube;

    public ResizeAbleArray<Vector3> vertices;
    public ResizeAbleArray<int> triangles;
    public ResizeAbleArray<Vector2> uvs;
    public ResizeAbleArray<Vector3> normals;
    public Vector4[] tangents;

    public void UpdateMesh()
    {
        Mesh mesh = cube.meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices.Array;
        mesh.triangles = triangles.Array;
        mesh.uv = uvs.Array;

        //mesh.RecalculateNormals();
        mesh.normals = normals.Array;
        mesh.tangents = tangents;

        //ResourceCube.TangentSolver(mesh);


        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}

public class ResourceCubeUpdater : MonoBehaviour {

    private static ResourceCubeUpdater instance;
    public static ResourceCubeUpdater Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ResourceCubeUpdater>();
            return instance;
        }
    }

    public Queue<MeshUpdater> updater = new Queue<MeshUpdater>();

    public int UpdateMeshEveryFrames = 5;
    private int UpdateMeshTimer = 0;
	
	// Update is called once per frame
	void Update () 
    {
        UpdateMeshTimer = Mathf.Min(0, UpdateMeshTimer--);

        if (UpdateMeshTimer == 0)
        {
            UpdateNextMesh();
            UpdateMeshTimer = UpdateMeshEveryFrames;
        }
	}

    public void AddUpdater(
        ResourceCube cube, 
        ResizeAbleArray<Vector3> vertices, 
        ResizeAbleArray<int> triangles, 
        ResizeAbleArray<Vector2> uvs, 
        ResizeAbleArray<Vector3> normals, 
        Vector4[] tangents)
    {
        MeshUpdater meshUpdater = new MeshUpdater();
        meshUpdater.cube = cube;
        meshUpdater.vertices = vertices;
        meshUpdater.triangles = triangles;
        meshUpdater.uvs = uvs;
        meshUpdater.normals = normals;
        meshUpdater.tangents = tangents;
        updater.Enqueue(meshUpdater);
    }

    public void UpdateNextMesh()
    {
        if (updater.Count == 0)
            return;

        MeshUpdater meshUpdater = updater.Dequeue();
        meshUpdater.UpdateMesh();
        meshUpdater.cube.MeshUpdate();
    }
}
