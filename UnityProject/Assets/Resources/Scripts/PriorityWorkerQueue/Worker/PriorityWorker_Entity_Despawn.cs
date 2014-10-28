using UnityEngine;

public class PriorityWorker_Entity_Despawn : PriorityWorker
{
    public static int PRIORITY = 3;

    public string PoolName;
    public GameObject GameObject;

    public delegate void AfterDespawnCallback();
    public event AfterDespawnCallback CallBack;

    public static PriorityWorker_Entity_Despawn Create(string poolName, GameObject gameObject, AfterDespawnCallback callBack)
    {
        PriorityWorker_Entity_Despawn worker = new PriorityWorker_Entity_Despawn();
        worker.PoolName = poolName;
        worker.GameObject = gameObject;
        worker.CallBack += callBack;
        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
        return worker;
    }

    public override void Work()
    {
        if (Canceled) return;
        GameObjectPool.Instance.Despawn(PoolName, GameObject);
        if (CallBack != null) CallBack();
    }
}
