
[System.Serializable]
public class ResourceBlockEntity : LevelEntity
{
    public int minResource = 2;
    public int maxResource = 64;

    protected override void AfterSpawnSetup()
    {
        base.AfterSpawnSetup();
        ResourceCube cube = gameObject.GetComponent<ResourceCube>();
        if(cube)
        {
            cube.SetResourceAmount(UnityEngine.Random.Range(minResource, maxResource));
        }
    }

}
