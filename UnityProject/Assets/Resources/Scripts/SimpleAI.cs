using UnityEngine;
using System.Collections;
using SimpleLibrary;

public class SimpleAI : Worker 
{

	public Entity TargetEntity = null;
	public Entity TargetJobEntity = null;

	public int Damage = 3;

	public float ClimbSpeed = 4.0f;
	public float FlyHeight = 2.0f;

	public float RotationDamping = 5.0f;
	private Vector3 WantedLookDirection = Vector3.zero;

	public override void OnSpawn()
	{
		base.OnSpawn();
		NextNavigationPosition = transform.position;
		WantedLookDirection = Vector3.forward;
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
					PathFinished(currentFindingPath.path);
					break;
				}
			}
		}

		if (path != null && !path.IsEmpty)
		{
			Cell Last = null;
			foreach (var item in path.path)
			{
				if (Last != null)
					Debug.DrawLine(new Vector3(Last.X, 0, Last.Z), new Vector3(item.X, 0, item.Z));
				Last = item;
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
			end = LevelGenerator.Level.FindNeighborWalkableCellRandom(end);
		}

		if (!end || !end.Walkable)
			return null;

		path = null;
		finished = false;

		currentFindingPath = new SearchingPath(start, end);
		return end;
	}

	public float WaitTimer = 0f;

	public override void OnJobChanged()
	{
		base.OnJobChanged();
		TargetJobEntity = null;

		Job curLastJob = LastJob;
		Job curJob = CurrentJob;

		if (curJob == null)
		{
			if (curLastJob == null)
				return;

			if (curLastJob.GetType() == typeof(Job_Mining) && !InventoryEmpty)
			{
				AddJob(new Job_GiveResources(Owner, this, "Giving Resources", Owner.Base, Owner.Base.Position.x, Owner.Base.Position.z));

				if (((Job_Mining)curLastJob).FinishedMining == false)
					AddJob(curLastJob);
			}
			return;
		}

		if (curJob.GetType() == typeof(Job))
		{

		}
		else if (curJob.GetType() == typeof(Job_Walk))
		{
			Job_Walk tmp = ((Job_Walk)curJob);
			Cell end = SetTargetPositionCell(tmp.cellX, tmp.cellZ);
			if (end == null) return;
			tmp.SetTargetPos(end.Position);
		}
		else if (curJob.GetType() == typeof(Job_Mining))
		{
			Job_Mining tmp = ((Job_Mining)curJob);
			TargetJobEntity = tmp.block;
			SetTargetPositionCell(tmp.cellX, tmp.cellZ);
		}
		else if (curJob.GetType() == typeof(Job_GiveResources))
		{
			if (curLastJob != null && curLastJob.GetType() == typeof(Job_Mining))
			{
				WaitTimer += 1.0f;
			}
			Job_GiveResources tmp = ((Job_GiveResources)curJob);
			TargetJobEntity = tmp.Target;
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

		if (destination.LevelEntity == null)
		{
			AddJob(new Job_Walk(Owner, this, "Walk To", destination.X, destination.Z));
		}
		else
		{
			Entity entity = destination.LevelEntity.SpawnedEntity;
			bool friend = Entity.Friends(this, entity);

			if (entity.GetType() == typeof(Entity_ResourceBlock))
			{
				AddJob(new Job_Mining(Owner, this, "Mining", ((Entity_ResourceBlock)entity), destination.X, destination.Z));
			}
			else if (entity.GetType() == typeof(Entity_PlayerBase))
			{
				if(friend)
					AddJob(new Job_GiveResources(Owner, this, "Giving Resources", ((Worker)entity), destination.X, destination.Z));
			}
			else
			{
				//AddJob(new Job(Owner, this, "Standing"));
			}
		}
	}

	public Vector3 NextNavigationPosition = Vector3.zero;

	public void NextWaypoint()
	{
		if (path.IsEmpty || path.IsLast)
			return;
		NextNavigationPosition = path.GetNext().Position;
	}

	public void CheckNextPosition()
	{
		if (LevelGenerator.Level.GetCell(NextNavigationPosition.x, NextNavigationPosition.z).Walkable == false)
		{
			Repath();
		}
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

		CheckNextPosition();
	}
	#endregion

	public float DotDiffAngleRot = 0.8f;
	public float DotDiffAngle = 0.75f;
	
	public bool WalkInTargetDir = false;
	public float DistanceToNavPoint = 0.5f;

	public float MinSpeed = 0.5f; //Backwards
	public float MaxSpeed = 5.0f; //Straight

	public float Speed = 0f;
	public float Acceleration = 2.0f;
	public float Break = 5f;

	public Vector3 RaycastPos
	{
		get
		{
			return transform.position + Vector3.up * 0.5f;
		}
	}

	public bool HasTarget
	{
		get
		{
			return TargetEntity != null;
		}
	}

	

	// Update is called once per frame
	public override void Update()
	{
		Walkable = !HasJob;
		Walkable = false;
		base.Update();

		UpdatePathFinding();

		if (path != null && !path.IsLast)
		{
			if (Vector3.Distance(transform.position, NextNavigationPosition + Vector3.up * FlyHeight) < DistanceToNavPoint)
			{
				NextWaypoint();
				CheckNextPosition();
			}
		}

		UpdateTarget();

		WantedLookDirection = NextNavigationPosition - transform.position;
		if (HasTarget && CanSeeTarget)
		{
			WantedLookDirection = TargetEntity.Position - transform.position;
		}
		WantedLookDirection.y = 0;
		
		if (WantedLookDirection.magnitude > 0.1f)
		{
			float dotRot = Mathf.Max(Vector3.Dot(transform.forward, WantedLookDirection.normalized) - DotDiffAngleRot, 0.0f) * (1f / DotDiffAngleRot);
			Quaternion wantedRotation = Quaternion.LookRotation(WantedLookDirection);

			transform.rotation = Quaternion.RotateTowards(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping * (1f - dotRot));        
		}

		Vector3 wantedPosition = NextNavigationPosition + Vector3.up * FlyHeight;

		Vector3 targetDiff = (wantedPosition - transform.position);
		float targetDistance = targetDiff.magnitude;


		float WantedSpeed = MaxSpeed;
		if (CanSeeTarget)
			WantedSpeed = Mathf.Max(WantedSpeed, MinSpeed);

		float dot = 1f;
		if (DotDiffAngle > 0)
			dot = Mathf.Max(Vector3.Dot(transform.forward, targetDiff.normalized) - DotDiffAngle, 0.0f) * (1f / DotDiffAngle);

		WantedSpeed *= dot;

		bool isNearEnd = ((path != null && path.IsLast) && targetDistance < 1f);

		if (isNearEnd)
			WantedSpeed *= Mathf.Min(targetDistance * targetDistance, 1.0f);

		WaitTimer -= Time.deltaTime;
		WaitTimer = Mathf.Max(WaitTimer, 0f);
		if (WaitTimer > 0)
			WantedSpeed *= 0;

		float speedChange = WantedSpeed > Speed ? Acceleration : Break;
		Speed = Mathf.Lerp(Speed, WantedSpeed, Time.deltaTime * speedChange);

		Vector3 walkDirection = transform.forward;
		if (WalkInTargetDir)
			walkDirection = targetDiff.normalized;

		speedChange = 1f;

		if (!isNearEnd)
		{
			RaycastHit hit;
			if (Physics.Raycast(new Ray(RaycastPos, transform.forward), out hit, InMyWayDistance, NavigationMask))
			{
				speedChange = 0.0f;
				SimpleAI ai = hit.collider.GetComponent<SimpleAI>();
				if (ai != null)
				{
					if (!ai.CanGoOutOfTheWay())
					{
						Repath();
					}
				}
			}
		}        

		rigidbody.AddForce(walkDirection * Speed * Time.deltaTime * speedChange * 100f);

		//transform.position += walkDirection * Speed * Time.deltaTime * speedChange;

		transform.position += Vector3.up * targetDiff.y * Time.deltaTime * ClimbSpeed;

		//Shooting
		ShootTimer -= Time.deltaTime;
		if (ShootTimer <= 0)
		{
			ShootTimer = ShootCD;
			Shoot();
		}

		PathUpdateTimer -= Time.deltaTime;
		if (HasJob && PathUpdateTimer <= 0)
		{
			PathUpdateTimer = PathUpdateTime;

			float Distance = 1000f;

			if (CurrentJob.GetType() == typeof(Job_Walk))
			{
				Job_Walk job = ((Job_Walk)CurrentJob);
				Distance = Vector3.Distance(Position, new Vector3(job.cellX, 0, job.cellZ));
				if (Distance > 1f)
				{
					SetTargetPositionCell(job.cellX, job.cellZ);
				}
			}
			else if (CurrentJob.GetType() == typeof(Job_Mining))
			{
				Job_Mining job = ((Job_Mining)CurrentJob);
				Distance = Vector3.Distance(Position, new Vector3(job.cellX, 0, job.cellZ));
				if (Distance > 1f)
				{
					SetTargetPositionCell(job.cellX, job.cellZ);
				}
			}
			else if (CurrentJob.GetType() == typeof(Job_GiveResources))
			{
				Job_GiveResources job = ((Job_GiveResources)CurrentJob);
				Distance = Vector3.Distance(Position, new Vector3(job.cellX, 0, job.cellZ));
				if (Distance > 1f)
				{
					SetTargetPositionCell(job.cellX, job.cellZ);
				}
			}
		}
		
	}

	public float InMyWayDistance = 1.0f;

	public float OutOfWayTime = 1.0f;
	public float OutOfWayTimer = 0f;

	public bool CanGoOutOfTheWay()
	{

		OutOfWayTimer -= Time.deltaTime;
		if (OutOfWayTimer > 0)
			return true;

		Cell dest = LevelGenerator.Level.GetCell(Position.x, Position.z);
		if(dest != null)
			dest = LevelGenerator.Level.FindNeighborWalkableCellRandom(dest);
		if(dest != null)
			dest = SetTargetPositionCell(dest.X, dest.Z);

		if (dest == null)
			return false;

		OutOfWayTimer = OutOfWayTime;
		return true;
	}

	public float PathUpdateTime = 1.0f;
	public float PathUpdateTimer = 0f;

	public void Repath()
	{
		if (path == null)
			return;

		Cell dest = SetTargetPositionCell(path.Destination.X, path.Destination.Z);
		if(dest == null)
		{
			return;
		}

		return;
		if (HasJob)
		{
			if (CurrentJob.GetType() == typeof(Job_Walk))
			{
				//((Job_Walk)CurrentJob).SetTargetPos(dest.Position);
				CurrentJob.Start();
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
		if (HasTarget)
		{
			Vector3 pos = Camera.main.WorldToScreenPoint(TargetEntity.Position);
			GUI.Label(new Rect(pos.x, Screen.height - pos.y, 200, 25), "Target");
		}
	}

	public float AttackRange = 10f;
	public LayerMask FindMask;
	public LayerMask AttackMask;

	public LayerMask NavigationMask;

	public float ShootCD = 1.0f;
	private float ShootTimer = 0f;

	public bool CanSeeSomething = false;
	public bool CanSeeTarget = false;

	public Collider otherCollider = null;

	public bool NeedsTargetInAttackThing = false;

	public PoolInfo Bullet;

	public void Shoot()
	{
		if (!HasTarget || !CanSeeTarget)
			return;

		Vector3 direction = (TargetEntity.Position - transform.position).normalized;
		float dot = Vector3.Dot(transform.forward, direction);
		if (dot < 0.5f)
			return;

		if(TargetEntity.GetType() == typeof(Entity_ResourceBlock) && AvailableSpace == 0)
		{
			TargetEntity = null;
			return;
		}

		if (NeedsTargetInAttackThing)
		{
			RaycastHit hit;
			if (!Physics.Raycast(new Ray(RaycastPos, transform.forward), out hit, AttackRange, AttackMask))
				return;
		}


		GameObject go = SimplePool.Spawn(Bullet, transform.position + transform.forward, Quaternion.LookRotation(direction));
		SimpleBullet bullet = go.GetComponent<SimpleBullet>();
		bullet.mask = AttackMask;
		bullet.Owner = this;
		bullet.Damage = Damage;
	}

	public bool IsInRange(Entity other)
	{
		if (other == null) return false;
		if (Vector3.Distance(other.Position, Position) < AttackRange)
			return true;
		return false;
		
	}

	public void UpdateTarget()
	{
		CanSeeTarget = false;
		CanSeeSomething = false;

		if (HasTarget && !TargetEntity.IsAlive)
		{
			TargetEntity = null;
		}
		if (IsInRange(TargetJobEntity))
		{
			if (Entity.Enemies(this, TargetJobEntity))
				TargetEntity = TargetJobEntity;
		}
		if (HasTarget && !IsInRange(TargetEntity))
		{
			TargetEntity = null;
		}

		if (!HasTarget)
		{
			//Find new Target
			Collider[] collider = Physics.OverlapSphere(transform.position, AttackRange, FindMask);
			for (int i = 0; i < collider.Length; i++)
			{
				Entity entity = collider[i].GetComponent<Entity>();
				if (entity != null && Entity.Enemies(this, entity))
				{
					TargetEntity = entity;
					break;
				}
			}
		}

		if (!HasTarget)
			return;

		Vector3 direction = (TargetEntity.Position - transform.position).normalized;
		RaycastHit hit;
		CanSeeSomething = Physics.Raycast(new Ray(RaycastPos, direction), out hit, AttackRange, AttackMask);
		if (CanSeeSomething)
			otherCollider = hit.collider;

		if (CanSeeSomething || hit.collider == TargetEntity.colliderForRaycasts)
		{
			CanSeeTarget = true;
		}

	}
}
