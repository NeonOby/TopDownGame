using UnityEngine;

public class PriorityWorker_Entity_Spawn : PriorityWorker
{
    public static int PRIORITY = 3;

    public string PoolName;
    public Vector3 Position;
    public Quaternion Rotation;

    public delegate void AfterSpawnCallback(GameObject go);
    public event AfterSpawnCallback CallBack;

    public static void Create(string poolName, Vector3 position, Quaternion rotation, AfterSpawnCallback callBack)
    {
        PriorityWorker_Entity_Spawn worker = new PriorityWorker_Entity_Spawn();
        worker.PoolName = poolName;
        worker.Position = position;
        worker.Rotation = rotation;
        worker.CallBack += callBack;
        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
    }

    public override void Work()
    {
        GameObject go = GameObjectPool.Instance.Spawn(PoolName, Position, Rotation);
        if (CallBack != null) CallBack(go);
    }
}
