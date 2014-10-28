
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

    public virtual void SetTargetCell(Cell value)
    {

    }
    public virtual void SetTargetPositionCell(Cell value)
    {

    }

    private Job currentJob;
    public Job CurrentJob
    {
        get
        {
            return currentJob;
        }
    }

    public override void Reset()
    {
        base.Reset();
        currentJob = null;
    }

    public virtual void SetJob(Job value)
    {
        currentJob = value;
    }
    public virtual void Update()
    {
        if (currentJob != null && currentJob.Update())
        {
            currentJob = currentJob.NextJob;
        }
    }
    public virtual void PathFinished(EntityController controller, Path newPath)
    {

    }
}

