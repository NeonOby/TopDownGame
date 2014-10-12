using UnityEngine;
using System.Collections;

public class SimpleAI : MonoBehaviour 
{

    public delegate void SimpleAIEvent(SimpleAI sender);
    public static event SimpleAIEvent SimpleAIDied;

    public void TriggerSimpleAIDied(SimpleAI sender)
    {
        if(SimpleAIDied != null)
        {
            SimpleAIDied(sender);
        }
    }

    public bool Enabled = false;

    public Transform currentTarget;
    public Vector3 targetPos;

    public float MinSpeed = 0.5f; //Backwards
    public float MaxSpeed = 5.0f; //Straight

    public void DespawnAllPerPool(string pool)
    {
        if (pool != poolName)
        {
            return;
        }
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }

	// Use this for initialization
	void Start () 
    {
        NextNavigationPosition = transform.position;
        WantedLookDirection = Vector3.forward;
        GameObjectPool.DespawnAllPerPool += DespawnAllPerPool;
	}

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public void SetTargetPos(Vector3 newTargetPos, bool removeTarget = false)
    {
        if (!Enabled)
            return;
        targetPos = newTargetPos;
        NextNavigationPosition = newTargetPos;
        if (removeTarget) currentTarget = null;
    }

    private void UpdateTargetPos()
    {
        if (currentTarget)
        {
            SetTargetPos(currentTarget.transform.position);
        }
    }

    public float RotationDamping = 5.0f;
    private Vector3 WantedLookDirection = Vector3.zero;

    public float CurrentSpeed = 0f;

    public Transform target = null;

    public Vector3 NextNavigationPosition = Vector3.zero;

    public void NextWaypoint()
    {
        NextNavigationPosition = path.GetNext().Position();
    }

	// Update is called once per frame
	void Update ()
    {
        if (!Enabled)
            return;

        if (Vector3.Distance(transform.position, NextNavigationPosition) < 0.2f)
        {
            if (path != null && !path.IsLast)
            {
                NextWaypoint();
            }
        }

        UpdateTarget();
        UpdateTargetPos();

        WantedLookDirection = NextNavigationPosition - transform.position;
        if (target != null)
        {
            WantedLookDirection = target.position - transform.position;
        }
        
        if (WantedLookDirection.magnitude > 0.3f)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(WantedLookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping);            
        }

        float WantedSpeed = 0f;

        float dot = Mathf.Max(Vector3.Dot(transform.forward, (NextNavigationPosition - transform.position).normalized)-0.5f, 0.0f)*2f;

        WantedSpeed = dot * MaxSpeed;

        if(target != null)
            WantedSpeed = Mathf.Max(WantedSpeed, MinSpeed);

        if(path != null && path.IsLast)
            WantedSpeed *= Mathf.Min((NextNavigationPosition - transform.position).magnitude, 1.0f);

        CurrentSpeed = Mathf.Lerp(WantedSpeed, WantedSpeed, Time.deltaTime * 1.0f);

        transform.position += (NextNavigationPosition - transform.position).normalized * CurrentSpeed * Time.deltaTime;

        ShootTimer += Time.deltaTime;
        if (ShootTimer >= ShootCD)
        {
            ShootTimer = 0f;
            Shoot();
        }
	}

    public float MaxDistance = 10f;
    public LayerMask mask;

    public float ShootCD = 1.0f;
    private float ShootTimer = 0f;

    public void Shoot()
    {
        if (target == null)
            return;

        GameObject go = GameObjectPool.Instance.Spawn("SimpleBullet", transform.position + transform.forward, Quaternion.LookRotation(WantedLookDirection));
        SimpleBullet bullet = go.GetComponent<SimpleBullet>();
        bullet.mask = mask;
    }

    public void UpdateTarget()
    {
        if (target != null)
        {
            SimpleAI ai = target.GetComponent<SimpleAI>();
            if (!ai.IsAlive)
                target = null;
        }
        if (target != null)
        {
            if (Vector3.Distance(target.position, transform.position) < MaxDistance)
            {
                //Target still in range
                return; 
            }
            else
            {
                //Target out of range
                target = null;
            }
        }

        //Find new Target
        Collider[] collider = Physics.OverlapSphere(transform.position, MaxDistance, mask);
        if(collider.Length > 0)
            target = collider[0].transform;
    }

    public int MaxHits = 10;
    public int CurrentHits = 0;
    private string poolName = "";

    public bool IsAlive
    {
        get
        {
            return CurrentHits < MaxHits;
        }
    }

    public void SetPoolName(string newPoolName)
    {
        poolName = newPoolName;
    }
    public void Reset()
    {
        CurrentHits = 0;
        Enabled = true;
        collider.enabled = true;
        NextNavigationPosition = transform.position;
    }

    public void Hit()
    {
        CurrentHits++;
        if (CurrentHits >= MaxHits)
        {
            TriggerSimpleAIDied(this);
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
    }

    public void Disable()
    {
        Enabled = false;
        collider.enabled = false;
    }

    public Path path = null;

    public void PathFinished(Path newPath)
    {
        path = newPath;
        if (path.IsEmpty)
        {
            return;
        }
        NextNavigationPosition = path.GetNext().Position();
    }
}
