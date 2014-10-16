using UnityEngine;

[System.Serializable]
public class LevelEntity
{
    public string PoolName = "";

    public Vector3Position Position = new Vector3Position();
    public QuaternionRotation Rotation = new QuaternionRotation();
}

