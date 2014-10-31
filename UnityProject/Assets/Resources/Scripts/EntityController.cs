using UnityEngine;
using System.Collections.Generic;

public class EntityController : Worker
{
    #region Instance
    public override void Awake()
    {
        base.Awake();
        Controller.Add(PlayerID, this);
    }
    #endregion

    public int PlayerID = 0;

    public static Dictionary<int, EntityController> Controller = new Dictionary<int, EntityController>();

    public static EntityController Get(int id)
    {
        return Controller[id];
    }

    public Entity_PlayerBase Base;

    #region Ressources
    //Inpsektor
    public int StartResources = 100;
     
    #endregion


    public virtual void Start()
    {
        AddResources(StartResources);
    }

    public virtual void RightClick()
    {

    }
    public virtual void ShiftRightClick()
    {

    }

    public virtual void GenerateJobForSelectedEntities()
    {

    }

}

