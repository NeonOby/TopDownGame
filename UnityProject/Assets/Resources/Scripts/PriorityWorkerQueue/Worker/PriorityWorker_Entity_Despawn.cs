using UnityEngine;

public class PriorityWorker_Entity_Despawn : PriorityWorker
{
    public static int PRIORITY = 3;

    public GameObject GameObject;

    public delegate void AfterDespawnCallback();
    public event AfterDespawnCallback CallBack;

    public static PriorityWorker_Entity_Despawn Create(GameObject gameObject, AfterDespawnCallback callBack)
    {
        PriorityWorker_Entity_Despawn worker = new PriorityWorker_Entity_Despawn();
        worker.GameObject = gameObject;
        worker.CallBack += callBack;
        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
        return worker;
    }

    public override void Work()
    {
        if (Canceled) return;
        SimpleLibrary.SimplePool.Despawn(GameObject);
        if (CallBack != null) CallBack();
    }
}
