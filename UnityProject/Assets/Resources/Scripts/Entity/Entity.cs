using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Pooling
    public string PoolName = "";
    public virtual void SetPoolName(string value)
    {
        PoolName = value;
    }

    public virtual void Reset()
    {

    }
    #endregion

    public Player Owner = null;

    public delegate void EntityEvent(Entity entity);
    public static event EntityEvent EntityDied;
    public static void TriggerEntityDied(Entity entity)
    {
        if (EntityDied != null) EntityDied(entity);
    }

    public Transform Transform;
    public Vector3 Position
    {
        get
        {
            return Transform.position;
        }
    }
    void Awake()
    {
        Transform = transform;
    }

    public virtual float Health
    {
        get
        {
            return 0f;
        }
    }
    public virtual bool IsAlive
    {
        get
        {
            return true;
        }
    }

    public virtual void GotHit(float value, Entity other)
    {

    }

    public void Hit(float value, Entity other)
    {
        GotHit(value, other);
        if (!IsAlive) Die();
    }

    public virtual void OnDeath()
    {
        
    }

    public void Die()
    {
        OnDeath();
        GameObjectPool.Instance.Despawn(PoolName, gameObject);
    }
}

