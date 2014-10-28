using UnityEngine;

public class PriorityWorker_ResourceCube_Spawn : PriorityWorker
{
    public static int PRIORITY = 3;

    public string PoolName;
    public Vector3 Position;
    public Quaternion Rotation;
    public Worker Target;

    public delegate void AfterSpawnCallback(GameObject go);
    public event AfterSpawnCallback CallBack;

    public static PriorityWorker_ResourceCube_Spawn Create(string poolName, Vector3 position, Quaternion rotation, AfterSpawnCallback callBack, Worker target)
    {
        PriorityWorker_ResourceCube_Spawn worker = new PriorityWorker_ResourceCube_Spawn();
        worker.PoolName = poolName;
        worker.Position = position;
        worker.Rotation = rotation;
        worker.CallBack += callBack;
        worker.Target = target;
        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
        return worker;
    }

    public override void Work()
    {
        if (Canceled) return;
        GameObject go = GameObjectPool.Instance.Spawn(PoolName, Position, Rotation);
        Entity_ResourceCube cube = go.GetComponent<Entity_ResourceCube>();
        if (cube) cube.target = Target;
        if (CallBack != null) CallBack(go);
    }
}
