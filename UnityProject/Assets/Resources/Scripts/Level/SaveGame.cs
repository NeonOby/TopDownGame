using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class SaveGame
{

    public Vector3Position CameraPosition = new Vector3Position();
    public Level level = null;
}

