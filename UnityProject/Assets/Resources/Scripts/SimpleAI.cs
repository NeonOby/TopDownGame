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

    private NavMeshAgent agent;

    public Transform currentTarget;
    public Vector3 targetPos;

    public float MinSpeed = 0.5f; //Backwards
    public float MaxSpeed = 5.0f; //Straight

	// Use this for initialization
	void Start () 
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent ==null)
        {
            enabled = false;
            return;
        }
        agent.updateRotation = false;
        agent.speed = 0f;
	}

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public void SetTargetPos(Vector3 newTargetPos, bool removeTarget = false)
    {
        targetPos = newTargetPos;
        agent.SetDestination(targetPos);
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

    public Transform target = null;

	// Update is called once per frame
	void Update ()
    {
        UpdateTarget();
        UpdateTargetPos();

        if (target != null)
        {
            WantedLookDirection = target.position - transform.position;
        }
        else
        {
            WantedLookDirection = agent.destination - transform.position;
        }
        
        if (WantedLookDirection.magnitude > 0.2f)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(WantedLookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping);
        }

        agent.speed = Vector3.Dot(transform.forward, (agent.destination - transform.position).normalized);
        agent.speed = Mathf.Max(agent.speed * MaxSpeed, MinSpeed);

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
}
