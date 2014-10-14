using System;
using System.Collections.Generic;

public class SearchingPath<Node> where Node : PathFind.IHasNeighbours<Node>
{
    public Node Start;
    public Node Destination;

    Func<Node, Node, double> CostBetweenTwo;
    Func<Node, Node, double> Heuristic;
    Func<Node, bool> Walkable;

    HashSet<Node> Closed;
    PriorityQueue<double, PathFind.Path<Node>> Queue;

    public RightClickTester.PathFinished CallBack = null;

    public SearchingPath(
            Node start,
            Node destination,
            Func<Node, Node, double> costBetweenTwo,
            Func<Node, Node, double> heuristic,
            Func<Node, bool> walkable)
    {
        Start = start;
        Destination = destination;
        Walkable = walkable;

        CostBetweenTwo = costBetweenTwo;
        Heuristic = heuristic;

        Closed = new HashSet<Node>();
        Queue = new PriorityQueue<double, PathFind.Path<Node>>();

        Queue.Enqueue(0, new PathFind.Path<Node>(start));
    }

    public PathFind.Path<Node> finishedPath = null;

    public PathFind.Path<Node> NextStepPath(out bool result)
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

        foreach (Node currentNode in path.LastStep.Neighbours)
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
}