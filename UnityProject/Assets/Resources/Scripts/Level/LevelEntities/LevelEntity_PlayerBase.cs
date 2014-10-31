using UnityEngine;

[System.Serializable]
public class LevelEntity_PlayerBase : LevelEntity
{
    public int minResource = 2;
    public int maxResource = 64;

    public int PlayerID = 0;

    protected override void AfterSpawn(GameObject go)
    {
        base.AfterSpawn(go);
        Entity_PlayerBase playerBase = go.GetComponent<Entity_PlayerBase>();
        if (playerBase)
        {
            playerBase.Controller = EntityController.Get(PlayerID);
            playerBase.Owner = playerBase.Controller;
        }
    }
}
