using UnityEngine;
using System.Collections;

public class SimpleBullet : MonoBehaviour 
{

    public static bool IsInLayerMask(GameObject obj, LayerMask mask){
        return ((mask.value & (1 << obj.layer)) > 0);
    }

    public Collider Collider;

    public float Speed = 5f;

    public float LifeTime = 4f;
    private float LifeTimer = 0f;

    public LayerMask mask;

    private string poolName = "";

    public bool Enabled = false;

    public void Reset()
    {
        LifeTimer = 0f;
        Enabled = true;
        Collider.enabled = true;
    }

    public void SetPoolName(string newPoolName)
    {
        poolName = newPoolName;
    }

    public void Disable()
    {
        Enabled = false;
        Collider.enabled = false;
    }

    void Update()
    {
        if (!Enabled)
            return;

        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTime)
        {
            Explode();
        }
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (!Enabled)
            return;

        if (IsInLayerMask(coll.gameObject, mask))
        {
            SimpleAI ai = coll.gameObject.GetComponent<SimpleAI>();
            if (ai == null)
                return;

            ai.Hit();
        }
        Explode();
    }

    void Explode()
    {
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
