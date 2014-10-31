using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_ResourceCube : Entity 
{
    public Worker target;

    public float MaxSpeed = 10f;
    public float Acceleration = 1f;

    public float Range = 10f;

    public float Speed = 0f;

    public float Gravity = 10f;

    public Queue<Worker> availableTargets = new Queue<Worker>();
    
    public LayerMask FindMask;

    public override void Reset()
    {
        base.Reset();
        Speed = 0f;
        rigidbody.velocity = Vector3.zero;
        target = null;

        availableTargets.Clear();

        rigidbody.AddForce(Vector3.up + Random.insideUnitSphere * 4f, ForceMode.Impulse);
        rigidbody.AddTorque(Random.insideUnitSphere * 10);
    }

    public void SetTarget(Worker worker)
    {
        if (target != null)
            target.RemoveIncomingResource();
        target = worker;
        if (target != null)
        {
            if (!target.AddIncomingResource())
            {
                
                //Doesn't want new Resource :D
                target = null;
            }
        }
    }

	void Update ()
    {
        if (!CheckTarget())
        {
            NextTarget();
            return;
        }

        Speed = Mathf.Min(Speed + Acceleration * Time.deltaTime, MaxSpeed);
	}

    public bool CheckTarget()
    {
        if (target == null)
            return false;

        if (target.IsDead)
        {
            RemoveTarget();
            return false;
        }

        if (!target.HasSpace)
        {
            RemoveTarget();
            return false;
        }

        if (Vector3.Distance(Position, target.Position) > Range)
        {
            RemoveTarget();
            return false;
        }

        return true;
    }

    public void NextTarget()
    {
        RemoveTarget();
        if (availableTargets.Count > 0)
            SetTarget(availableTargets.Dequeue());
    }

    public void RemoveTarget()
    {
        Speed = 0f;
        SetTarget(null);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        RemoveTarget();
    }

    void FixedUpdate()
    {
        rigidbody.AddForce((Vector3.down * (MaxSpeed - Speed) / MaxSpeed) * Time.fixedDeltaTime * Gravity);

        if (!CheckTarget())
            return;

        if (Vector3.Distance(transform.position, target.Position) < 0.5f)
        {
            int taken = target.AddResources(1);
            if (taken > 0)
            {
                RemoveTarget();
                Die();
            }
            else
                RemoveTarget();
            return;
        }

        rigidbody.AddForce((target.Position - transform.position).normalized * Speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Worker worker = other.GetComponent<Worker>();

        if (worker != null)
        {
            availableTargets.Enqueue(worker);
        }
    }
}
