using UnityEngine;

[System.Serializable]
public class LevelEntity
{
    public string PoolName = "";

    public Vector3Position Position = new Vector3Position();
    public QuaternionRotation Rotation = new QuaternionRotation();

    protected GameObject gameObject = null;

    protected virtual void AfterSpawnSetup()
    {

    }

    public void Spawn()
    {
        gameObject = GameObjectPool.Instance.Spawn(PoolName, Position.Value, Rotation.Value);
        AfterSpawnSetup();
    }
    public void Despawn()
    {
        GameObjectPool.Instance.Despawn(PoolName, gameObject);
        gameObject = null;
    }
}

