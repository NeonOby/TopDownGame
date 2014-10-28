using UnityEngine;
using System.Collections;

public class SimpleBullet : MonoBehaviour 
{
    public Worker Owner;

    public static bool IsInLayerMask(GameObject obj, LayerMask mask){
        return ((mask.value & (1 << obj.layer)) > 0);
    }

    public Collider Collider;

    public float Speed = 5f;

    public float LifeTime = 4f;
    private float LifeTimer = 0f;

    public LayerMask mask;

    private string poolName = "";

    public void Reset()
    {
        LifeTimer = 0f;
    }

    public void SetPoolName(string newPoolName)
    {
        poolName = newPoolName;
    }

    void Update()
    {
        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTime)
        {
            Explode();
        }
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Resource")
            return;

        if (IsInLayerMask(coll.gameObject, mask))
        {
            Entity entity = coll.gameObject.GetComponent<Entity>();
            if (entity == null)
                return;

            entity.Hit(1f, Owner);
        }
        Explode();
    }

    void Explode()
    {
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
