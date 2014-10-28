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

    private int StartResources = 0;

    public override bool IsFinished
    {
        get
        {
            return block.CurrentResources == 0;
        }
    }

    public override string Info
    {
        get
        {
            return System.String.Format("{0} :{1:### %}", Name, Progression);
        }
    }

    //Every job should implement this
    //It tells KI if this job is broken or not avaible anymore
    public override bool IsAvaible
    {
        get;
        protected set;
    }

    public override void Start()
    {
        StartResources = block.CurrentResources;
    }

    public override void UpdateProgression()
    {
        if (StartResources > 0)
            Progression = (1f - (block.CurrentResources / StartResources));
        else
            Progression = 1f;
    }
}

