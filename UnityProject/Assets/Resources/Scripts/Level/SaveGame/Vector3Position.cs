using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Vector3Position
{
    [JsonProperty]
    public float PosX = 0, PosY = 0, PosZ = 0;

    public Vector3 Value
    {
        get
        {
            return new Vector3(PosX, PosY, PosZ);
        }
        set
        {
            PosX = value.x;
            PosY = value.y;
            PosZ = value.z;
        }
    }
}