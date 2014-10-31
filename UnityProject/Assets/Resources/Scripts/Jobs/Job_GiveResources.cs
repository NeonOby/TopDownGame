using UnityEngine;


[System.Serializable]
public class Job_GiveResources : Job
{

    public Job_GiveResources(EntityController owner, Worker worker, string name, Worker target, float x, float z)
        : base(owner, worker, name)
    {
        cellX = x;
        cellZ = z;
        this.Target = target;
    }

    public float cellX, cellZ;
    public Worker Target;

    public void SetTargetPos(Vector3 target)
    {
        WantedPosition = target;
    }

    public Vector3 WantedPosition;
    private float StartDistance = 0f;

    public float Range = 5f;

    public float GivingCoolDown = 0.15f;
    private float givingTimer = 0f;

    public override bool IsFinished
    {
        get
        {
            return Target.InventoryFull || Worker.InventoryEmpty;
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
        StartDistance = Vector3.Distance(Worker.Position, WantedPosition);
    }

    public override void UpdateProgression()
    {
        if (Worker.HasResources && Target.HasSpace)
        {
            givingTimer -= Time.deltaTime;
            givingTimer = Mathf.Max(givingTimer, 0);
            if (givingTimer <= 0)
            {
                if (Vector3.Distance(Worker.Position, Target.Position) < Range)
                    Worker.DropResources(Target, 1);
                givingTimer = GivingCoolDown;
            }
        }

        float prog1 = (Target.AvailableSpace / (float)Target.MaxResources);
        float prog2 = (Worker.CurResources / (float)Worker.MaxResources);

        Progression = Mathf.Max(prog1, prog2);
    }
}

