using UnityEngine;

[System.Serializable]
public class Job
{
    public Job(EntityController owner, Worker worker, string name, Vector3 target)
    {
        Owner = owner;
        Worker = worker;
        Name = name;
        WantedPosition = target;

        Working = false;
    }

    public EntityController Owner { get; set; }
    public Worker Worker { get; set; }
    public string Name { get; set; }
    public float Progression = 0f;
    public Vector3 WantedPosition;
    private float StartDistance = 0f;

    public bool Working
    {
        get;
        protected set;
    }

    public virtual string Info
    {
        get
        {
            return System.String.Format("{0} :{1:### %}", Name, Progression);
        }
    }

    //Every job should implement this
    //It tells KI if this job is broken or not avaible anymore
    public virtual bool IsAvaible
    {
        get;
        protected set;
    }

    public virtual void Start()
    {
        StartDistance = Vector3.Distance(Worker.CurrentPosition, WantedPosition);

        Working = true;
    }

    public virtual bool Update()
    {
        Progression = (1f - (Vector3.Distance(Worker.CurrentPosition, WantedPosition) / StartDistance));
        return Progression >= 1;
    }

    public Job NextJob { get; set; }

}

