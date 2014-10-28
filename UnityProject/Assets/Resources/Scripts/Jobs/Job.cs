using UnityEngine;


[System.Serializable]
public class Job
{

    public Job(EntityController owner, Worker worker, string name)
    {
        Owner = owner;
        Worker = worker;
        Name = name;
    }

    public EntityController Owner { get; set; }
    public Worker Worker { get; set; }
    public string Name { get; set; }
    public float Progression = 0f;

    public virtual bool IsFinished
    {
        get
        {
            return Progression >= 1;
        }
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
        Progression = 1f;
    }

    public virtual void UpdateProgression()
    {
        
    }

    public bool Update()
    {
        UpdateProgression();
        return IsFinished;
    }

}

