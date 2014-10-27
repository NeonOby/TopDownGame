using UnityEngine;

public class PriorityWorker_ResourceCube_UpdateMesh  : PriorityWorker
{
    public static int PRIORITY = 3;

    public ResourceCube cube;

    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector3[] normals;
    public Vector4[] tangents;

    public static void Create(ResourceCube cube,
        Vector3[] vertices,
        int[] triangles,
        Vector2[] uvs,
        Vector3[] normals,
        Vector4[] tangents)
    {
        PriorityWorker_ResourceCube_UpdateMesh worker = new PriorityWorker_ResourceCube_UpdateMesh();
        worker.cube = cube;
        worker.vertices = vertices;
        worker.triangles = triangles;
        worker.uvs = uvs;
        worker.normals = normals;
        worker.tangents = tangents;

        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
    }

    public override void Work()
    {
        Mesh mesh = cube.meshFilter.mesh;

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.tangents = tangents;


        mesh.RecalculateBounds();
        mesh.Optimize();

        cube.MeshUpdate();
    }
}

