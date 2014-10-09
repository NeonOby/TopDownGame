using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace PathFind
{

    public static class PathFind
    {
        public static Path<Node> FindPath<Node>(
            Node start,
            Node destination,
            Func<Node, Node, double> costBetweenTwo,
            Func<Node, Node, double> heuristic)
            where Node : IHasNeighbours<Node>
        {
            var closed = new HashSet<Node>();
            var queue = new PriorityQueue<double, Path<Node>>();
            queue.Enqueue(0, new Path<Node>(start));

            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();

                if (closed.Contains(path.LastStep))
                    continue;
                if (path.LastStep.Equals(destination))
                    return path;

                closed.Add(path.LastStep);

                foreach (Node currentNode in path.LastStep.Neighbours)
                {
                    double d = costBetweenTwo(path.LastStep, currentNode);
                    var newPath = path.AddStep(currentNode, d);
                    queue.Enqueue(newPath.TotalCost + heuristic(currentNode, destination), newPath);
                }
            }

            return null;
        }
    }
}