using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameObjectInfo
{
    public GameObjectInfo(string pool, GameObject go)
    {
        poolName = pool;
        gameObject = go;
    }
    public string poolName;
    public GameObject gameObject;
}

