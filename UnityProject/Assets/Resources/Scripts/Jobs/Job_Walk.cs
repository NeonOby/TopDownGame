using UnityEngine;


[System.Serializable]
public class Job_Walk : Job
{

    public Job_Walk(EntityController owner, Worker worker, string name, float x, float z) : base(owner, worker, name)
    {
        cellX = x;
        cellZ = z;
    }

    public float cellX, cellZ;

    public void SetTargetPos(Vector3 target)
    {
        WantedPosition = target;
    }

    public Vector3 WantedPosition;
    private float StartDistance = 0f;

    public override bool IsFinished
    {
        get
        {
            return Mathf.RoundToInt((Progression * 10)) >= 10;
        }
    }

    public override void Start()
    {
        StartDistance = Vector3.Distance(Worker.Position, WantedPosition);
    }

    public override void UpdateProgression()
    {
        Progression = (1f - (Vector3.Distance(Worker.Position, WantedPosition) / StartDistance));
    }
}

