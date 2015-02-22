using SimpleLibrary;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public static bool Friends(Entity entity1, Entity entity2)
    {
        if (entity1 == null || entity2 == null || entity1.Owner == null || entity2.Owner == null)
            return false;
        return entity1.Owner.PlayerID == entity2.Owner.PlayerID;
    }
    public static bool Enemies(Entity entity1, Entity entity2)
    {
        if (entity1 == null || entity2 == null || entity1.Owner == null || entity2.Owner == null)
            return false;
        return entity1.Owner.PlayerID != entity2.Owner.PlayerID;
    }

	public virtual void OnSpawn()
	{

	}

    public Collider colliderForRaycasts = null;

    public EntityController Owner = null;

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
    public virtual void Awake()
    {
        Transform = transform;
        if (colliderForRaycasts == null)
            colliderForRaycasts = collider;
    }

    public virtual int Health
    {
        get
        {
            return 1;
        }
    }
    public virtual bool IsAlive
    {
        get
        {
            return Health > 0;
        }
    }
    public virtual bool IsDead
    {
        get
        {
            return Health == 0;
        }
    }

    public virtual void GotHit(int value, Entity other)
    {

    }

    public void Hit(int value, Entity other)
    {
        GotHit(value, other);
        if (!IsAlive) Despawn();
    }

    public virtual void OnDeath()
    {
        
    }

    public void Despawn()
    {
        OnDeath();
        SimplePool.Despawn(gameObject);
        TriggerEntityDied(this);
    }

    public bool Walkable = true;
    public float lastCellX = 0, lastCellZ = 0;

    public bool AddedEntityToLast = false;

    public virtual void UpdateWalkable()
    {
        Cell tmp = null;
        if (AddedEntityToLast)
        {
            tmp = LevelGenerator.Level.GetCell(lastCellX, lastCellZ);
            if (tmp != null)
                tmp.EntityLeave(this);
        }

        if (!Walkable)
        {
            lastCellX = Position.x;
            lastCellZ = Position.z;
            tmp = LevelGenerator.Level.GetCell(lastCellX, lastCellZ);
            if (tmp != null)
                tmp.EntityEnter(this);
            AddedEntityToLast = tmp != null;
        }
    }

    public virtual void Update()
    {
        UpdateWalkable();
    }
}

