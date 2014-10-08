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
        cell = grid[path[index].X, path[index].Y];
        return cell;
    }

    public Cell GetParentCell(int index)
    {
        Cell cell;
        cell = grid[path[index].PX, path[index].PY];
        return cell;
    }
}

public class ExternNavigation : MonoBehaviour
{

    private Path currentPath = new Path();

    public List<PathFinderNode> GetPath(int startX, int startZ, int endX, int endZ)
    {
        if (LevelGenerator.level == null)
            return null;

        if (LevelGenerator.level.PositionOutOfLevel(endX, endZ))
            return null;

        PathFinderFast fast;
        startX -= LevelGenerator.level.lowestX * LevelGenerator.ChunkSize;
        startZ -= LevelGenerator.level.lowestZ * LevelGenerator.ChunkSize;
        endX -= LevelGenerator.level.lowestX * LevelGenerator.ChunkSize;
        endZ -= LevelGenerator.level.lowestZ * LevelGenerator.ChunkSize;

        Point start = new Point(startX, startZ);
        Point end = new Point(endX, endZ);

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

            currentPath.path = GetPath(0, 0, (int)targetPos.x, (int)targetPos.z);
            //Debug.Log("Path Finished: " + (currentPath == null ? "Not Found" : "Found"));
        }
        if (currentPath != null && currentPath.path != null)
        {
            for (int i = 0; i < currentPath.path.Count; i++)
            {
                Vector3 currentPos = currentPath.GetCell(i).Position();
                currentPos.y = 1;
                Vector3 positionBefore = currentPath.GetParentCell(i).Position();
                positionBefore.y = 1;
                Debug.DrawLine(positionBefore, currentPos);
            }
        }
        
    }
}
