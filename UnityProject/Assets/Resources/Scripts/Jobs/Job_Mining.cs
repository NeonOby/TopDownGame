using UnityEngine;


[System.Serializable]
public class Job_Mining : Job
{

    public Job_Mining(EntityController owner, Worker worker, string name, Entity_ResourceBlock block, float cellX, float cellZ)
        : base(owner, worker, name)
    {
        this.block = block;
        this.cellX = cellX;
        this.cellZ = cellZ;
    }

    public float cellX, cellZ;
    public Entity_ResourceBlock block;

    public int StartResources = 64;

    public override bool IsFinished
    {
        get
        {
            return FinishedMining && Worker.IncomingResource == 0;
        }
    }

    public bool FinishedMining
    {
        get
        {
            return (block == null || block.IsDead || block.CurResources == 0);
        }
    }

    public override bool Paused
    {
        get
        {
            return Worker.InventoryFull && Worker.IncomingResource == 0;
        }
    }

    public float Timer = 0f;
    public float Wait = 0.0f;

    public override void Start()
    {
        StartResources = 64;
        Timer = Wait;
        Entity.EntityDied += OnEntityDied;
    }

    public void OnEntityDied(Entity entity)
    {
        if (entity == block)
        {
            block = null;
        }
    }

    public override void UpdateProgression()
    {
        if (block == null || block.IsDead)
            return;

        Progression = (1f - (block.CurResources / (float)StartResources));

        
    }
}

