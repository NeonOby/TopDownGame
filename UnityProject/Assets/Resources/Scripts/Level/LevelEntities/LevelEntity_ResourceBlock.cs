using UnityEngine;

[System.Serializable]
public class LevelEntity_ResourceBlock : LevelEntity
{
    public int minResource = 2;
    public int maxResource = 64;

    public int Resources = 0;

    protected override void AfterSpawn(GameObject go)
    {
        base.AfterSpawn(go);
        Entity_ResourceBlock cube = go.GetComponent<Entity_ResourceBlock>();
        if(cube)
        {
            cube.SetResourceAmount(Resources);
            cube.SetLevelEntity(this);
            cube.Owner = EntityController.Get(1);
        }
    }

    public override void Init(float value)
    {
        Resources = minResource + (int)(value * (maxResource - minResource));
    }

}
