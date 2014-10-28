using UnityEngine;
using System.Collections;

public class Entity_ResourceCube : Entity 
{
    public Worker target;

    public float MaxSpeed = 10f;
    public float Acceleration = 1f;

    public float Speed = 0f;

    public override void Reset()
    {
        base.Reset();
        Speed = 0f;
        rigidbody.velocity = Vector3.zero;

        rigidbody.AddForce(Vector3.up + Random.insideUnitSphere * 4f, ForceMode.Impulse);
        rigidbody.AddTorque(Random.insideUnitSphere * 10);
    }

	void Update ()
    {
        if (target == null)
            return;

        Speed = Mathf.Min(Speed + Acceleration * Time.deltaTime, MaxSpeed);
	}

    void FixedUpdate()
    {
        if (target == null)
            return;

        if (Vector3.Distance(transform.position, target.Position) < 0.5f)
        {
            target.AddResources(1);
            Die();
            return;
        }

        rigidbody.AddForce((target.Position - transform.position) * Speed * Time.deltaTime);
        rigidbody.AddForce(Vector3.down * (Speed - MaxSpeed) / MaxSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (target != null)
            return;
        target = other.GetComponent<Worker>();
    }
}
