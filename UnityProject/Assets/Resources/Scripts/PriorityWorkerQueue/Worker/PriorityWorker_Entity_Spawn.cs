using SimpleLibrary;
using UnityEngine;

public class PriorityWorker_Entity_Spawn : PriorityWorker
{
    public static int PRIORITY = 3;

    public PoolInfo PoolName;
    public Vector3 Position;
    public Quaternion Rotation;

    public delegate void AfterSpawnCallback(GameObject go);
    public event AfterSpawnCallback CallBack;

	public static PriorityWorker_Entity_Spawn Create(PoolInfo poolName, Vector3 position, Quaternion rotation, AfterSpawnCallback callBack)
    {
        PriorityWorker_Entity_Spawn worker = new PriorityWorker_Entity_Spawn();
        worker.PoolName = poolName;
        worker.Position = position;
        worker.Rotation = rotation;
        worker.CallBack += callBack;
        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
        return worker;
    }

    public override void Work()
    {
        if (Canceled) return;
        GameObject go = SimplePool.Spawn(PoolName, Position, Rotation);
        if (CallBack != null) CallBack(go);
    }
}
