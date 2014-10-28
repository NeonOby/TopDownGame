using System;
using UnityEngine;

public class GridSystem
{
    public static bool GridPositionUnderMouse(out Vector3 gridPosition, Camera Cam = null)
    {
        if (Cam == null)
            Cam = Camera.main;

        gridPosition = Vector3.zero;
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = 0f;
        if (!plane.Raycast(ray, out distance))
            return false;

        gridPosition = ray.origin + ray.direction * distance;
        return true;
    }
}

