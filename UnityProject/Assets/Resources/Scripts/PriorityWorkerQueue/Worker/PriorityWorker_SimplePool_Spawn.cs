using UnityEngine;

public class PriorityWorker_SimplePool_Spawn : PriorityWorker
{
    public static int PRIORITY = 3;

    public SimpleLibrary.PoolInfo PoolName;
    public Vector3 Position;
    public Quaternion Rotation;

    public delegate void AfterSpawnCallback(GameObject go);
    public event AfterSpawnCallback CallBack;

	public static PriorityWorker_SimplePool_Spawn Create(SimpleLibrary.PoolInfo poolName, Vector3 position, Quaternion rotation, AfterSpawnCallback callBack)
    {
		PriorityWorker_SimplePool_Spawn worker = new PriorityWorker_SimplePool_Spawn();
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
		GameObject go = SimpleLibrary.SimplePool.Spawn(PoolName, Position, Rotation);
        if (CallBack != null) CallBack(go);
    }
}
