using UnityEngine;

[System.Serializable]
public class ResourceBlockEntity : LevelEntity
{
    public int minResource = 2;
    public int maxResource = 64;

    public int Resources = 0;

    protected override void AfterSpawn(GameObject go)
    {
        base.AfterSpawn(go);
        ResourceCube cube = go.GetComponent<ResourceCube>();
        if(cube)
        {
            cube.SetResourceAmount(Resources);
        }
    }

    public override void Init(float value)
    {
        Resources = minResource + (int)(value * (maxResource - minResource));
    }

}
