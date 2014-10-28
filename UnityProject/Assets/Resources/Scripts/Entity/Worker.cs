using System.Collections.Generic;
using UnityEngine;

public class Worker : Entity
{
    #region Health
    public float health = 0f;
    public override float Health
    {
        get
        {
            return health;
        }
    }
    public override bool IsAlive
    {
        get
        {
            return Health > 0;
        }
    }
    public override void GotHit(float value, Entity other)
    {
        health = Mathf.Max(health - value, 0);
    }
    public override void OnDeath()
    {
        
    }
    #endregion

    #region Inventory
    private int resources = 0;
    public int MaxResources = 20;

    public void AddResources(int amount)
    {
        resources = Mathf.Min(resources + amount, MaxResources);
    }

    public void DropResources(Worker target)
    {
        for (int i = 0; i < resources; i++)
        {
            PriorityWorker_ResourceCube_Spawn.Create("ResourceCube", transform.position + Vector3.up, Quaternion.identity, null, target);
        }
        resources = 0;
    }
    #endregion

    public virtual void SetTargetCell(Cell value, bool shift)
    {

    }
    protected virtual Cell SetTargetPositionCell(float x, float z)
    {
        return null;
    }

    protected Queue<Job> jobQueue = new Queue<Job>();
    public void ClearJobs()
    {
        jobQueue.Clear();
        CurrentJob = null;
    }
    public void AddJob(Job job)
    {
        jobQueue.Enqueue(job);
        if (CurrentJob == null)
            NextJob();
    }
    public virtual void OnJobChanged()
    {

    }
    public void NextJob()
    {
        if (jobQueue.Count == 0)
        {
            CurrentJob = null;
            return;
        }
        CurrentJob = jobQueue.Dequeue();
        OnJobChanged();
        CurrentJob.Start();
    }

    public Job CurrentJob
    {
        get;
        protected set;
    }

    public override void Reset()
    {
        base.Reset();
        CurrentJob = null;
    }

    public virtual void Update()
    {
        if (CurrentJob != null && CurrentJob.Update())
        {
            NextJob();
        }
    }
    public virtual void PathFinished(Path newPath)
    {

    }
}

