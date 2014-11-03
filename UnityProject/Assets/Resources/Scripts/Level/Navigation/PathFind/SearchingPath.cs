using System;
using System.Collections.Generic;
using UnityEngine;


public class SearchingPath
{
    public delegate void PathFinished(EntityController controller, Path path);

    public Cell Start;
    public Cell Destination;

    public EntityController Owner;

    HashSet<Cell> Closed;
    PriorityQueue<double, PathNode> Queue;

    public PathFinished CallBack = null;

    public Path path = null;

    public SearchingPath(
            Cell start,
            Cell destination)
    {
        Start = start;
        Destination = destination;

        Closed = new HashSet<Cell>();
        Queue = new PriorityQueue<double, PathNode>();

        Queue.Enqueue(0, new PathNode(start));
    }

    public void GeneratePath()
    {
        path = new Path();
        Cell lastWaypointCell = Destination;

        path.AddWaypoint(Destination);
        foreach (var item in finishedPath)
        {
            if (Raycast(path.GetLast(), item))
            {
                path.AddWaypoint(lastWaypointCell);
            }

            lastWaypointCell = item;
        }
        path.AddWaypoint(Start);
        path.Destination = Destination;
    }

    public bool Raycast(Cell start, Cell end)
    {
        Vector3 direction = (end.Position - start.Position);
        Vector3 startingPoint = start.Position;

        Vector3 perp = Vector3.Cross(direction.normalized, Vector3.right);

        Ray ray1 = new Ray(startingPoint - perp * 0.5f, direction.normalized);
        Ray ray2 = new Ray(startingPoint + perp * 0.5f, direction.normalized);

        bool b1 = Physics.Raycast(ray1, direction.magnitude);
        bool b2 = Physics.Raycast(ray2, direction.magnitude);
        
        return (b1 || b2);

        //return Physics.SphereCast(ray, 0.4f, direction.magnitude);


        float StartX = start.X < end.X ? start.X : end.X;
        float StartZ = start.Z < end.Z ? start.Z : end.Z;

        float EndX = start.X > end.X ? start.X : end.X;
        float EndZ = start.Z > end.Z ? start.Z : end.Z;

        Vector3 point = Vector3.zero;
        for (float x = StartX; x < EndX; x++)
        {
            for (float z = StartZ; z < EndZ; z++)
            {
                point = new Vector3(x, 0, z);
                

                //float distance = Vector3.Cross(ray.direction, point - ray.origin).magnitude;
                float distance = DistancePointLine(point, start.Position, end.Position);
                //Debug.Log(distance);
                if (distance < 0.5)
                {
                    //if (LevelGenerator.Level.GetCell(x, z).Walkable == false)
                        //return true;
                }
            }
        }
        return false;
    }

    public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }
    public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }

    public PathNode finishedPath = null;

    public PathNode NextStepPath(out bool result)
    {
        result = false;
        if (Queue.IsEmpty)
            return null;

        var path = Queue.Dequeue();

        if (Closed.Contains(path.LastStep))
        {
            return path;
        }
        if (path.LastStep.Equals(Destination))
        {
            finishedPath = path;
            GeneratePath();
            result = true;
            return path;
        }

        Closed.Add(path.LastStep);

        foreach (Cell currentNode in path.LastStep.Neighbours)
        {
            if (!Walkable(currentNode))
            {
                Closed.Add(currentNode);
                continue;
            }

            double d = CostBetweenTwo(path.LastStep, currentNode);
            var newPath = path.AddStep(currentNode, d);
            Queue.Enqueue(newPath.TotalCost + Heuristic(currentNode, Destination), newPath);
        }
        return path;
    }

    public double CostBetweenTwo(Cell cell1, Cell cell2)
    {
        return Vector3.Distance(cell1.Position, cell2.Position) + cell2.WalkCost;
    }

    public double Heuristic(Cell currentCell, Cell end)
    {
        return Mathf.Pow(Vector3.Distance(currentCell.Position, end.Position), 2) * 10f;
    }

    public bool Walkable(Cell cell)
    {
        if (cell == null)
            return false;
        return cell.Walkable;
    }
}