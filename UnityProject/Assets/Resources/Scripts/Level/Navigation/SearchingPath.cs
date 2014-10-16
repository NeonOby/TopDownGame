using System;
using System.Collections.Generic;
using UnityEngine;

public class SearchingPath
{
    public Cell Start;
    public Cell Destination;

    HashSet<Cell> Closed;
    PriorityQueue<double, PathFind.Path<Cell>> Queue;

    public RightClickTester.PathFinished CallBack = null;

    public SearchingPath(
            Cell start,
            Cell destination)
    {
        Start = start;
        Destination = destination;

        Closed = new HashSet<Cell>();
        Queue = new PriorityQueue<double, PathFind.Path<Cell>>();

        Queue.Enqueue(0, new PathFind.Path<Cell>(start));
    }

    public PathFind.Path<Cell> finishedPath = null;

    public PathFind.Path<Cell> NextStepPath(out bool result)
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
        return cell.Walkable;
    }
}