using UnityEngine;
using System.Collections;

public abstract class PriorityWorker
{
	public abstract void Work();
}

public class PriorityWorkerQueue : MonoBehaviour 
{

	private static PriorityWorkerQueue instance;
	public static PriorityWorkerQueue Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<PriorityWorkerQueue>();
			return instance;
		}
	}

	public static void AddWorker(int priority, PriorityWorker worker)
	{
		Instance.AddWorkerIntern(priority, worker);
	}

	public PriorityQueue<int, PriorityWorker> queue = new PriorityQueue<int, PriorityWorker>();

	public int WorkEveryXFrames = 2;
	public int WorkerPerUpdate = 1;

	private int frameCounter = 0;


	private void AddWorkerIntern(int priority, PriorityWorker worker)
	{
		//DEBUG priority
		if(priority <= 0)
		{
			worker.Work();
			return;
		}

		queue.Enqueue(priority, worker);
	}
	
	// Update is called once per frame
	void Update () 
	{
		frameCounter = Mathf.Min(frameCounter--, 0);
		if(frameCounter == 0)
		{
			if (!queue.IsEmpty)
			{
				PriorityWorker worker;
				for (int i = 0; i < WorkerPerUpdate; i++)
				{
					worker = queue.Dequeue();
					worker.Work();

					if (queue.IsEmpty)
						break;
				}
				frameCounter = WorkEveryXFrames;
			}
		}
	}
}
