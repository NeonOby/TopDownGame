using UnityEngine;
using System.Collections;

public class PriorityWorker_SimplePool_Despawn : PriorityWorker
{
	public static int PRIORITY = 3;

	public GameObject GameObject;

	public delegate void AfterDespawnCallback();
	public event AfterDespawnCallback CallBack;

	public static PriorityWorker_SimplePool_Despawn Create(GameObject gameObject, AfterDespawnCallback callBack)
	{
		PriorityWorker_SimplePool_Despawn worker = new PriorityWorker_SimplePool_Despawn();
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
