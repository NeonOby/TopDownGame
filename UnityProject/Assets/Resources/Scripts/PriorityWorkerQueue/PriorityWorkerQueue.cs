using UnityEngine;
using System.Collections;

public class PriorityWorker
{
    public bool Canceled = false;
    public void Cancel()
    {
        Canceled = true;
    }
    public virtual void Work()
    {

    }
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
		using (var tryLock = new TryLock(fastLock))
		{
			if (tryLock.HasLock)
			{
				queue.Enqueue(priority, worker);
			}
		}
	}

    private static System.Object fastLock = new System.Object();

	public static PriorityQueue<int, PriorityWorker> queue = new PriorityQueue<int, PriorityWorker>();
	
	public float StartTime = 0f;
	public float CurrentUsedTime = 0f;
	public float MaxUsedDeltaTimePerFrame = 0.02f;

	private bool HasMoreTime
	{
		get
		{
			return TimeDiff(StartTime) < MaxUsedDeltaTimePerFrame;
		}
	}
	private float TimeDiff(float last)
	{
		return Time.realtimeSinceStartup - last;
	}

	// Update is called once per frame
	void Update ()
	{
		StartTime = Time.realtimeSinceStartup;
        using (var tryLock = new TryLock(fastLock))
        {
            if (tryLock.HasLock)
            {
                if (!queue.IsEmpty)
                {
                    PriorityWorker worker;
                    while (HasMoreTime)
					{
						worker = queue.Dequeue();
						worker.Work();

						if (queue.IsEmpty)
							break;
					}
                }
            }
        }
	}
}
