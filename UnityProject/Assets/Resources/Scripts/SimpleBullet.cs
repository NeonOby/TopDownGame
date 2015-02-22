using UnityEngine;
using System.Collections;
using SimpleLibrary;

public class SimpleBullet : MonoBehaviour 
{
    public Worker Owner;

    public static bool IsInLayerMask(GameObject obj, LayerMask mask){
        return ((mask.value & (1 << obj.layer)) > 0);
    }

    public int Damage = 1;

    public float Impulse = 10f;

    public float LifeTime = 4f;
    private float LifeTimer = 0f;

    public LayerMask mask;

    public void OnSpawn()
    {
        LifeTimer = 0f;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.forward * Impulse, ForceMode.Impulse);
    }

    void Update()
    {
        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTime)
        {
            Explode();
            return;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (IsInLayerMask(coll.gameObject, mask))
        {
            Entity entity = coll.gameObject.GetComponent<Entity>();
            if (entity == null)
                return;

            entity.Hit(Damage, Owner);
        }
        Explode();
    }

    void Explode()
    {
        SimplePool.Despawn(gameObject);
    }
}
