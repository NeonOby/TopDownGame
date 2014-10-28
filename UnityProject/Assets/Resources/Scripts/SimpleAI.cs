using UnityEngine;
using System.Collections;

public class SimpleAI : Worker 
{
	public override void SetPoolName(string value)
	{
		base.SetPoolName(value);
	}

	public Entity TargetEntity = null;
	public Entity TargetJobEntity = null;


	public float ClimbSpeed = 4.0f;
	public float FlyHeight = 2.0f;

	// Use this for initialization
	void Start()
	{
		NextNavigationPosition = transform.position;
		WantedLookDirection = Vector3.forward;

		AddJob(new Job(Owner, this, "StandingAround"));
	}

	public void SetTarget(Entity newTarget)
	{
		TargetEntity = newTarget;
	}

	public float RotationDamping = 5.0f;
	private Vector3 WantedLookDirection = Vector3.zero;

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

	protected override Cell SetTargetPositionCell(float x, float z)
	{
		Cell start = LevelGenerator.Level.GetCell(Position.x, Position.z);

		if (!start)
			return null;

		Cell end = LevelGenerator.Level.GetCell(x, z);

		if (!end || !end.Walkable)
		{
			end = LevelGenerator.Level.FindNeighborWalkableCell(end);
		}

		if (!end || !end.Walkable)
			return null;

		path = null;
		finished = false;

		currentFindingPath = new SearchingPath(start, end);
		return end;
	}

	public override void OnJobChanged()
	{
		base.OnJobChanged();
		TargetJobEntity = null;

		if (CurrentJob.GetType() == typeof(Job))
		{

		}
		else if (CurrentJob.GetType() == typeof(Job_Walk))
		{
			Job_Walk tmp = ((Job_Walk)CurrentJob);
			Cell end = SetTargetPositionCell(tmp.cellX, tmp.cellZ);
			tmp.SetTargetPos(end.Position);
		}
		else if (CurrentJob.GetType() == typeof(Job_Mining))
		{
			Job_Mining tmp = ((Job_Mining)CurrentJob);
			TargetJobEntity = tmp.block;
			SetTargetPositionCell(tmp.cellX, tmp.cellZ);
		}
	}

	//Right Click
	public override void SetTargetCell(Cell destination, bool shift)
	{
		if (!shift)
		{
			ClearJobs();
			TargetEntity = null;
			TargetJobEntity = null;
		}
			

		if (destination.Entity == null)
		{
			AddJob(new Job_Walk(Owner, this, "Walk To", destination.X, destination.Z));
		}
		else
		{
			Entity entity = destination.Entity.SpawnedEntity;
			if (entity is Entity_ResourceBlock)
			{
				AddJob(new Job_Mining(Owner, this, "Mining", ((Entity_ResourceBlock)entity), destination.X, destination.Z));
			}
			else
			{
				AddJob(new Job(Owner, this, "Standing"));
			}
		}
	}

	public Vector3 NextNavigationPosition = Vector3.zero;

	public void NextWaypoint()
	{
		NextNavigationPosition = path.GetNext().Position;
	}

	public override void PathFinished(Path newPath)
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
	}
	#endregion

	public float DotDiffAngle = 0.75f;
	public bool WalkInTargetDir = false;
	public float DistanceToNavPoint = 0.5f;

	public float MinSpeed = 0.5f; //Backwards
	public float MaxSpeed = 5.0f; //Straight

	public float Speed = 0f;
	public float Acceleration = 2.0f;
	public float Break = 5f;

	// Update is called once per frame
	public override void Update()
	{
		base.Update();

		UpdatePathFinding();

		if (path != null && !path.IsLast)
		{
			if (Vector3.Distance(transform.position, NextNavigationPosition + Vector3.up * FlyHeight) < DistanceToNavPoint)
			{
				NextWaypoint();
			}
		}

		UpdateTarget();

		WantedLookDirection = NextNavigationPosition - transform.position;
		if (TargetEntity != null && CanSeeTarget)
		{
			WantedLookDirection = TargetEntity.Position - transform.position;
		}
		WantedLookDirection.y = 0;
		
		if (WantedLookDirection.magnitude > 0.1f)
		{
			float dotRot = Mathf.Max(Vector3.Dot(transform.forward, WantedLookDirection.normalized) - 0.5f, 0.0f) * (1f / 0.5f);
			Quaternion wantedRotation = Quaternion.LookRotation(WantedLookDirection);

			transform.rotation = Quaternion.RotateTowards(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping * (1f - dotRot));        
		}

		Vector3 wantedPosition = NextNavigationPosition + Vector3.up * FlyHeight;

		Vector3 targetDiff = (wantedPosition - transform.position);
		float targetDistance = targetDiff.magnitude;


		float WantedSpeed = MaxSpeed;
		if (CanSeeTarget)
			WantedSpeed = Mathf.Max(WantedSpeed, MinSpeed);


		float dot = 0f;
		if (DotDiffAngle > 0)
			dot = Mathf.Max(Vector3.Dot(transform.forward, targetDiff.normalized) - DotDiffAngle, 0.0f) * (1f / DotDiffAngle);

		WantedSpeed *= dot;


		if(path != null && path.IsLast)
			WantedSpeed *= Mathf.Min(targetDistance, 1.0f);

		float speedChange = WantedSpeed > Speed ? Acceleration : Break;
		Speed = Mathf.Lerp(Speed, WantedSpeed, Time.deltaTime * speedChange);

		Vector3 walkDirection = transform.forward;
		if (WalkInTargetDir)
			walkDirection = targetDiff.normalized;

		transform.position += walkDirection * Speed * Time.deltaTime;

		transform.position += Vector3.up * targetDiff.y * Time.deltaTime * ClimbSpeed;

		//Shooting
		ShootTimer += Time.deltaTime;
		if (ShootTimer >= ShootCD)
		{
			ShootTimer = 0f;
			Shoot();
		}
	}

	void OnGUI()
	{
		if (CurrentJob != null)
		{
			Vector3 pos = Camera.main.WorldToScreenPoint(Position);
			GUI.Label(new Rect(pos.x, Screen.height - pos.y, 200, 25), CurrentJob.Info);
		}
		if (TargetEntity != null)
		{
			Vector3 pos = Camera.main.WorldToScreenPoint(TargetEntity.Position);
			GUI.Label(new Rect(pos.x, Screen.height - pos.y, 200, 25), "Target");
		}
	}

	public float AttackRange = 10f;
	public LayerMask FindMask;
	public LayerMask AttackMask;

	public float ShootCD = 1.0f;
	private float ShootTimer = 0f;

	public bool CanSeeTarget = false;

	public Collider otherCollider = null;

	public void Shoot()
	{
		CanSeeTarget = false;
		if (TargetEntity == null)
			return;

		Vector3 direction = (TargetEntity.Position - transform.position).normalized;
		float dot = Vector3.Dot(transform.forward, direction);
		if (dot < 0.5f)
			return;

		RaycastHit hit;
		bool CanSeeSomething = Physics.Raycast(new Ray(transform.position, direction), out hit, AttackRange, AttackMask);
		if(CanSeeSomething)
			otherCollider = hit.collider;

		if (!CanSeeSomething || hit.collider != TargetEntity.colliderForRaycasts)
		{
			return;
		}

		CanSeeTarget = true;

		if (!Physics.Raycast(new Ray(transform.position, transform.forward), out hit, AttackRange, AttackMask))
			return;

		GameObject go = GameObjectPool.Instance.Spawn("SimpleBullet", transform.position + transform.forward, Quaternion.LookRotation(direction));
		SimpleBullet bullet = go.GetComponent<SimpleBullet>();
		bullet.mask = AttackMask;
		bullet.Owner = this;
	}

	public void UpdateTarget()
	{
		if (TargetEntity != null && !TargetEntity.IsAlive)
		{
			TargetEntity = null;
		}
		if (TargetJobEntity != null && Vector3.Distance(TargetJobEntity.Position, transform.position) < AttackRange)
		{
			TargetEntity = TargetJobEntity;
		}
		if (TargetEntity != null)
		{
			if (Vector3.Distance(TargetEntity.Position, transform.position) < AttackRange)
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
		Collider[] collider = Physics.OverlapSphere(transform.position, AttackRange, FindMask);
		if (collider.Length > 0)
			SetTarget(collider[0].GetComponent<Entity>());
	}
}
