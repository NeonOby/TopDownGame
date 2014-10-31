using UnityEngine;
using System.Collections.Generic;

public class Waypoint
{
    public Vector3 Position = Vector3.zero;
    public float MinWidthForPath = 1f;


    #region Connections
    public List<Waypoint> Connections = new List<Waypoint>();

    public void AddConnection(Waypoint other, bool check)
    {
        if (Connections.Contains(other))
            Connections.Add(other);
    }
    #endregion


    #region StaticFunctions
    public static float Distance(Waypoint wp1, Waypoint wp2)
    {
        return Vector3.Distance(wp1.Position, wp2.Position);
    }
    #endregion
}

