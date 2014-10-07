using System;
using UnityEngine;
using System.Collections.Generic;

public class Path
{
    public List<Cell> Waypoints = new List<Cell>();

    public void AddWaypoint(Cell cell)
    {
        Waypoints.Add(cell);
    }
}

