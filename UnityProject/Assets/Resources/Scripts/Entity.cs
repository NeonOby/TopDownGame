using UnityEngine;

public class Entity : MonoBehaviour
{
    public string PoolName = "";
    public virtual void SetPoolName(string value)
    {
        PoolName = value;
    }
    public virtual void Reset()
    {

    }
    public virtual void Die()
    {
        GameObjectPool.Instance.Despawn(PoolName, gameObject);
    }
}

