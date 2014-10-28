using UnityEngine;

[System.Serializable]
public class LevelEntity
{
    public string PoolName = "";

    public Vector3Position Position = new Vector3Position();
    public QuaternionRotation Rotation = new QuaternionRotation();

    protected GameObject gameObject = null;

    public Entity SpawnedEntity = null;

    PriorityWorker currentWorker = null;

    protected virtual void AfterSpawn(GameObject go)
    {
        gameObject = go;
        SpawnedEntity = gameObject.GetComponent<Entity>();
        currentWorker = null;
    }

    protected virtual void AfterDespawn()
    {
        gameObject = null;
        SpawnedEntity = null;
        currentWorker = null;
    }

    public void Spawn()
    {
        PriorityWorker_Entity_Spawn.Create(PoolName, Position.Value, Rotation.Value, AfterSpawn);
    }
    public void Despawn()
    {
        PriorityWorker_Entity_Despawn.Create(PoolName, gameObject, AfterDespawn);
    }

    public virtual void Init(float value)
    {

    }
}

