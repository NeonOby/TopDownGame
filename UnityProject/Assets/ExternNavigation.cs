using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Algorithms;

public class Path
{
    public Cell[,] grid;
    public List<PathFinderNode> path = new List<PathFinderNode>();

    public Cell GetCell(int index)
    {
        Cell cell;
        int x = path[index].X;
        int y = path[index].Y;
        cell = grid[x, y];
        return cell;
    }

    public Cell GetParentCell(int index)
    {
        Cell cell;
        int x = path[index].PX;
        int y = path[index].PY;
        cell = grid[x, y];
        return cell;
    }
}

public class ExternNavigation : MonoBehaviour
{

    private Path currentPath = new Path();

    public List<PathFinderNode> GetPath(float startX, float startZ, float endX, float endZ)
    {
        if (LevelGenerator.level == null)
            return null;

        /*
        if (LevelGenerator.level.PositionOutOfLevel(endX, endZ))
            return null;
        */

        if (!LevelGenerator.level.ContainsChunk(endX, endZ))
            return null;

        PathFinderFast fast;
        startX -= LevelGenerator.level.lowestX * LevelGenerator.ChunkSize;
        startZ -= LevelGenerator.level.lowestZ * LevelGenerator.ChunkSize;
        endX -= LevelGenerator.level.lowestX * LevelGenerator.ChunkSize;
        endZ -= LevelGenerator.level.lowestZ * LevelGenerator.ChunkSize;

        Point start = new Point((int)startX, (int)startZ);
        Point end = new Point((int)endX, (int)endZ);
   

        currentPath.grid = LevelGenerator.level.GetCurrentGrid();
        fast = new PathFinderFast(currentPath.grid);

        fast.Formula = HeuristicFormula.Manhattan;
        fast.Diagonals = true;
        fast.HeavyDiagonals = false;
        fast.HeuristicEstimate = 2;
        fast.PunishChangeDirection = false;
        fast.TieBreaker = true;
        fast.SearchLimit = 2000;
        fast.DebugProgress = false;
        fast.DebugFoundPath = false;
        
        return fast.FindPath(start, end);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance = 0f;
            if (!plane.Raycast(ray, out distance))
                return;

            Vector3 targetPos = ray.origin + ray.direction * distance;

            currentPath.path = GetPath(0, 0, targetPos.x, targetPos.z);
            //Debug.Log("Path Finished: " + (currentPath == null ? "Not Found" : "Found"));
        }
        if (currentPath != null && currentPath.path != null)
        {
            for (int i = 0; i < currentPath.path.Count; i++)
            {
                Vector3 currentPos = currentPath.GetCell(i).Position() + Vector3.right * 0.5f + Vector3.forward * 0.5f;
                currentPos.y = 1;
                Vector3 positionBefore = currentPath.GetParentCell(i).Position() + Vector3.right * 0.5f + Vector3.forward * 0.5f;
                positionBefore.y = 1;
                Debug.DrawLine(positionBefore, currentPos);
            }
        }
        
    }
}
