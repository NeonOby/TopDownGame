using UnityEngine;

[System.Serializable]
public class Job
{
    public Job(SimpleAI owner, string name, Cell target)
    {
        Owner = owner;
        Name = name;

        StartDistance = Vector3.Distance(Owner.CurrentPosition, WantedPosition);
    }

    public SimpleAI Owner { get; set; }
    public string Name { get; set; }
    public float Progression = 0f;
    public string Info
    {
        get
        {
            return System.String.Format("{0} :{1:### %}", Name, Progression);
        }
    }

    public Cell Target { get; set; }
    public Vector3 WantedPosition
    {
        get
        {
            if (!Target)
                return Owner.CurrentPosition;
            return Target.Position; 
        }
    }

    private float StartDistance = 0f;

    //Returns true when finished
    public virtual bool Update()
    {
        Progression = (1f - (Vector3.Distance(Owner.CurrentPosition, WantedPosition) / StartDistance));
        return Progression >= 1;
    }

    //The next job when this one is finished
    public Job NextJob { get; set; }

}

