using UnityEngine;
using System.Collections;

public class PriorityWorker_Gameobject_SetActive : PriorityWorker 
{
    public static int PRIORITY = 3;

    public GameObject gameObject;
    public bool value;

    public static void Create(GameObject gameObject, bool value)
    {
        PriorityWorker_Gameobject_SetActive worker = new PriorityWorker_Gameobject_SetActive();
        worker.gameObject = gameObject;
        worker.value = value;
        PriorityWorkerQueue.AddWorker(PRIORITY, worker);
    }

    public override void Work()
    {
        gameObject.SetActive(value);
    }
}
