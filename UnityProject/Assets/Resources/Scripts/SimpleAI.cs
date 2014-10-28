using UnityEngine;
using System.Collections;

public class SimpleAI : Worker 
{

    public override void SetPoolName(string value)
    {
        base.SetPoolName(value);
    }

    public Entity TargetEntity = null;

    public float MinSpeed = 0.5f; //Backwards
    public float MaxSpeed = 5.0f; //Straight

    public float ClimbSpeed = 4.0f;
    public float FlyHeight = 2.0f;

    public Job CurrentJob = null;

	// Use this for initialization
	void Start () 
    {
        NextNavigationPosition = transform.position;
        WantedLookDirection = Vector3.forward;

        CurrentJob = new Job(Owner, this, "StandingAround", transform.position);
        CurrentJob.NextJob = null;
	}

    public void SetTarget(Entity newTarget)
    {
        TargetEntity = newTarget;
    }

    public float RotationDamping = 5.0f;
    private Vector3 WantedLookDirection = Vector3.zero;

    public float CurrentSpeed = 0f;

    public override void Reset()
    {
        base.Reset();
        NextNavigationPosition = transform.position;
    }

    #region PathFinding
    SearchingPath currentFindingPath = null;
    bool finished = false;

    public Path path = null;

    public void UpdatePathFinding()
    {
        if (currentFindingPath != null && !finished)
        {
            for (int i = 0; i < 10; i++)
            {
                currentFindingPath.NextStepPath(out finished);
                if (finished)
                {
                    PathFinished(currentFindingPath.GeneratePath());
                    break;
                }
            }
        }
    }

    public override void SetTargetPositionCell(Cell end)
    {
        Cell start = LevelGenerator.Level.GetCell(Position.x, Position.z);

        if (!start)
            return;

        if (!end || !end.Walkable)
        {
            end = LevelGenerator.Level.FindNeighborWalkableCell(end);
        }

        if (!end || !end.Walkable)
            return;

        path = null;
        finished = false;

        currentFindingPath = new SearchingPath(start, end);
    }

    public override void SetTargetCell(Cell destination)
    {
        if (destination.Entity != null)
        {
            TargetEntity = destination.Entity.SpawnedEntity;
        }
        else
        {
            TargetEntity = null;
            SetTargetPositionCell(destination);
        }
    }

    public override void SetJob(Job value)
    {
        base.SetJob(value);

    }

    public Vector3 NextNavigationPosition = Vector3.zero;

    public void NextWaypoint()
    {
        NextNavigationPosition = path.GetNext().Position;
    }

    public void PathFinished(Path newPath)
    {
        path = newPath;
        if (path == null || path.IsEmpty)
        {
            NextNavigationPosition = transform.position;
            return;
        }
        NextWaypoint();
        if (!path.IsLast)
            NextWaypoint();

        CurrentJob = new Job(Owner, this, "WalkTo", path.Destination.Position + Vector3.up * FlyHeight);
    }
    #endregion

    // Update is called once per frame
	void Update ()
    {
        UpdatePathFinding();

        if (Vector3.Distance(transform.position, NextNavigationPosition + Vector3.up * FlyHeight) < 0.2f)
        {
            if (path != null && !path.IsLast)
            {
                NextWaypoint();
            }
        }

        UpdateTarget();

        WantedLookDirection = NextNavigationPosition - transform.position;
        if (TargetEntity != null)
        {
            WantedLookDirection = TargetEntity.Position - transform.position;
        }
        WantedLookDirection.y = 0;
        
        if (WantedLookDirection.magnitude > 0.2f)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(WantedLookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping);            
        }

        Vector3 wantedPosition = NextNavigationPosition + Vector3.up * FlyHeight;

        float WantedSpeed = 0f;

        float dot = Mathf.Max(Vector3.Dot(transform.forward, (wantedPosition - transform.position).normalized) - 0.5f, 0.0f) * 2f;

        WantedSpeed = dot * MaxSpeed;

        if (TargetEntity != null)
            WantedSpeed = Mathf.Max(WantedSpeed, MinSpeed);

        if(path != null && path.IsLast)
            WantedSpeed *= Mathf.Min((wantedPosition - transform.position).magnitude, 1.0f);

        CurrentSpeed = Mathf.Lerp(WantedSpeed, WantedSpeed, Time.deltaTime * 1.0f);


        transform.position += (wantedPosition - transform.position).normalized * CurrentSpeed * Time.deltaTime;

        transform.position += Vector3.up * (wantedPosition - transform.position).y * Time.deltaTime * ClimbSpeed;

        //Shooting
        ShootTimer += Time.deltaTime;
        if (ShootTimer >= ShootCD)
        {
            ShootTimer = 0f;
            Shoot();
        }

        
        //Job Update
        if (CurrentJob != null)
        {
            if (CurrentJob.Update())
            {
                CurrentJob = CurrentJob.NextJob;
            }
        }
        
	}

    void OnGUI()
    {
        if (CurrentJob != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(Position);
            GUI.Label(new Rect(pos.x, Screen.height - pos.y, 200, 25), CurrentJob.Info);
        }
    }

    public float MaxDistance = 10f;
    public LayerMask FindMask;
    public LayerMask AttackMask;

    public float ShootCD = 1.0f;
    private float ShootTimer = 0f;

    public void Shoot()
    {
        if (TargetEntity == null)
            return;

        Vector3 direction = (TargetEntity.Position - transform.position).normalized;

        float dot = Vector3.Dot(transform.forward, TargetEntity.Position - transform.position);
        if (dot < 0.5f)
            return;

        GameObject go = GameObjectPool.Instance.Spawn("SimpleBullet", transform.position + transform.forward, Quaternion.LookRotation(direction));
        SimpleBullet bullet = go.GetComponent<SimpleBullet>();
        bullet.mask = AttackMask;
        bullet.Owner = this;
    }

    public void UpdateTarget()
    {
        if (TargetEntity != null)
        {
            if (!TargetEntity.IsAlive)
                TargetEntity = null;
        }
        if (TargetEntity != null)
        {
            if (Vector3.Distance(TargetEntity.Position, transform.position) < MaxDistance)
            {
                //Target still in range
                return; 
            }
            else
            {
                //Target out of range
                TargetEntity = null;
            }
        }

        //Find new Target
        Collider[] collider = Physics.OverlapSphere(transform.position, MaxDistance, FindMask);
        if (collider.Length > 0)
            SetTarget(collider[0].GetComponent<Entity>());
    }
}
