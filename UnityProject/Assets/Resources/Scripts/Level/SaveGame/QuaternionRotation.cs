using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class QuaternionRotation
{
    [JsonProperty]
    public float RotX = 0, RotY = 0, RotZ = 0, RotW = 0;

    public Quaternion Value
    {
        get
        {
            return new Quaternion(RotX, RotY, RotZ, RotW);
        }
        set
        {

            RotX = value.x;
            RotY = value.y;
            RotZ = value.z;
            RotW = value.w;
        }
    }
}

