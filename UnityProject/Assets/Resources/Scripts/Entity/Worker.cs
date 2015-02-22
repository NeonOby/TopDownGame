using SimpleLibrary;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Entity
{
    public Healthbar healthBar = null;
	public PoolInfo ResourceCube;

    public virtual void UpdateHealthBar()
    {
        if (healthBar == null)
            return;
        healthBar.UpdateHealth(ProcentageHealth);
    }

    #region Health
    public int health = 10;
    public int StartHealth = 10;
    public int MaxHealthValue = 10;
    public override int Health
    {
        get
        {
            return health + resources;
        }
    }
    public virtual int MaxHealth
    {
        get
        {
            return MaxHealthValue + MaxResources;
        }
    }
    public virtual float ProcentageHealth
    {
        get
        {
            return health / MaxHealthValue;
        }
    }

    public override void GotHit(int value, Entity other)
    {
        value = Mathf.Min(Health, value);

        Worker worker = null;
        if (other.GetType() == typeof(Worker))
            worker = (Worker)other;

        for (int i = 0; i < value; i++)
        {
			PriorityWorker_ResourceCube_Spawn.Create(ResourceCube, transform.position + Vector3.up, Quaternion.identity, null, worker);
        }

        int lostResources = Mathf.Min(resources, value);
        int lostHealth = Mathf.Min(health, value - lostResources);

        resources -= lostResources;
        health -= lostHealth;
    }
    #endregion

    #region Inventory
    public int resources = 0;
    public int MaxResources = 20;
    public virtual int CurResources
    {
        get
        {
            return resources + IncomingResource;
        }
    }

    public int IncomingResource = 0;

    //Returns if it can take more resources
    public bool AddIncomingResource()
    {
        if (AvailableSpace <= 0)
        {
            return false;
        }
        IncomingResource++;
        return true;
    }
    public void RemoveIncomingResource()
    {
        IncomingResource--;
        IncomingResource = Mathf.Max(IncomingResource, 0);
    }

    public bool InventoryFull
    {
        get
        {
            return resources >= MaxResources;
        }
    }

    public bool InventoryEmpty
    {
        get
        {
            return resources == 0;
        }
    }

    public bool HasSpace
    {
        get
        {
            return Health < MaxHealth;
        }
    }

    public bool HasResources
    {
        get
        {
            return !InventoryEmpty;
        }
    }

    public int AvailableSpace
    {
        get
        {
            return MaxHealth - Health;
        }
    }

    public virtual void OnResourcesChanged(int amount)
    {

    }

    public virtual void OnHealed(int amount)
    {

    }

    public int Heal(int amount)
    {
        if (amount <= 0)
            return 0;

        amount = Mathf.Min(amount, MaxHealthValue - health);
        health += amount;

        if (amount > 0)
            OnHealed(amount);

        return amount;
    }

    public int AddResources(int amount)
    {
        if (amount <= 0)
            return 0;

        int healAmount = Heal(amount);

        int moreResourceAmount = Mathf.Min(amount - healAmount, MaxResources - resources);
        resources += moreResourceAmount;

        if (moreResourceAmount > 0)
            OnResourcesChanged(moreResourceAmount);

        return healAmount + moreResourceAmount;
    }

    public void DropResources(Worker target, int amount)
    {
        if (amount <= 0)
            return;

        amount = Mathf.Min(amount, resources);

        if (amount == 0)
            return;

        for (int i = 0; i < amount; i++)
        {
			PriorityWorker_ResourceCube_Spawn.Create(ResourceCube, transform.position + Vector3.up, Quaternion.identity, null, target);
        }
        resources -= amount;
        OnResourcesChanged(amount);
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
        if (job == null)
            return;

        jobQueue.Enqueue(job);
        if (CurrentJob == null)
            NextJob();
    }
    public virtual void OnJobChanged()
    {

    }

    public bool HasJob
    {
        get
        {
            return CurrentJob != null;
        }
    }

    public Job CurrentJob = null;
    public Job LastJob = null;

    public void NextJob()
    {
        LastJob = CurrentJob;
        if (jobQueue.Count == 0)
        {
            CurrentJob = null;
            OnJobChanged();
            return;
        }
        CurrentJob = jobQueue.Dequeue();
        OnJobChanged();
        CurrentJob.Start();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        ClearJobs();
        CurrentJob = null;
        LastJob = null;
        health = StartHealth;

        IncomingResource = 0;
    }

    public override void Update()
    {
        base.Update();
        if (HasJob)
        {
            if (CurrentJob.Name == "")
            {
                NextJob();
                return;
            }

            bool finished = CurrentJob.Update();

            if (CurrentJob.Paused)
                finished = true;

            if (finished)
                NextJob();
        }
        UpdateHealthBar();
    }

    public virtual void PathFinished(Path newPath)
    {

    }
}

