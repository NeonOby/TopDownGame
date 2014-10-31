using UnityEngine;
using System.Collections;

public class Entity_PlayerBase : Worker {

    private EntityController controller;
    public EntityController Controller
    {
        get
        {
            return controller;
        }
        set
        {
            controller = value;
            if (controller != null)
                controller.Base = this;
        }
    }

    public override int CurResources
    {
        get
        {
            return controller.resources + IncomingResource;
        }
    }


}
