using UnityEngine;

[System.Serializable]
public class LevelEntity
{
    public string PoolName = "";
	public SimpleLibrary.PoolInfo pool;

    public Vector3Position Position = new Vector3Position();
    public QuaternionRotation Rotation = new QuaternionRotation();

    protected GameObject gameObject = null;

    public Entity SpawnedEntity = null;

    protected virtual void AfterSpawn(GameObject go)
    {
        gameObject = go;
        SpawnedEntity = gameObject.GetComponent<Entity>();
    }

    protected virtual void AfterDespawn()
    {
        gameObject = null;
        SpawnedEntity = null;
    }

    public void Spawn()
    {
		PriorityWorker_SimplePool_Spawn.Create(pool, Position.Value, Rotation.Value, AfterSpawn);
    }
    public void Despawn()
    {
		PriorityWorker_SimplePool_Despawn.Create(gameObject, AfterDespawn);
    }

    public virtual void Init(float value)
    {

    }
}

