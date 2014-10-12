using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class SearchingPath<Node> where Node : PathFind.IHasNeighbours<Node>
{
	Node Start;
	Node Destination;

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


public class RightClickTester : MonoBehaviour 
{
    public delegate void PathFinished(Path path);

    SearchingPath<Cell> currentFindingPath = null;
    bool finished = false;
	PathFind.Path<Cell> path = null;

	int updatesPerFrame = 50;

	float startTime = 0;
	float LastNeededTime = 0;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(1))
		{
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			float distance = 0f;
			if (!plane.Raycast(ray, out distance))
				return;
			Vector3 targetPos = ray.origin + ray.direction * distance;
			Do(targetPos); 
		}
		if (currentFindingPath != null && !finished)
		{
			for (int i = 0; i < updatesPerFrame; i++)
			{
				path = currentFindingPath.NextStepPath(out finished);
				if (finished)
				{
					LastNeededTime = Time.realtimeSinceStartup - startTime;
					break;
				}
			}
		}
		
		if (path != null && path.PreviousSteps != null)
		{
			Vector3 lastPos = path.LastStep.Position();
			lastPos.y = 0;
			foreach (var item in path)
			{
				Vector3 currentPos = item.Position();
				currentPos.y = 0.5f;
				Debug.DrawLine(lastPos, currentPos, Color.green);
				lastPos = currentPos;
			}
		}
	}

	void OnGUI()
	{
		GUILayout.Label(LastNeededTime.ToString());
	}

    public Cell FindNeighborWalkableCell(Cell cell, Cell start)
    {
        float minDistance = -1f;
        Cell foundCell = null;
        foreach (var neighbor in cell.Neighbours)
        {
            if (neighbor.Walkable)
            {
                if (minDistance == -1 || Distance(neighbor, start) < minDistance)
                {
                    foundCell = neighbor;
                    minDistance = Distance(neighbor, start);
                }
            }
        }
        return foundCell;
    }

	public void Do(Vector3 pos)
	{
		Cell start = LevelGenerator.level.GetCell(0, 0);
		Cell end = LevelGenerator.level.GetCell(pos.x, pos.z);
        if (!end.Walkable)
        {
            end = FindNeighborWalkableCell(end, start);
        }

		if (end == null || !end.Walkable)
			return;

        path = null;
        finished = false;

		currentFindingPath = new SearchingPath<Cell>(start, end, CostBetweenNeighbors, Heuristic, Walkable);
		startTime = Time.realtimeSinceStartup;
		//path = PathFind.PathFind.FindPath<Cell>(start, end, CostBetweenNeighbors, Heuristic);
	}

    public float Distance(Cell cell1, Cell cell2)
    {
        return Vector3.Distance(cell1.Position(), cell2.Position());
    }

	public double CostBetweenNeighbors(Cell cell1, Cell cell2)
	{
		return Vector3.Distance(cell1.Position(), cell2.Position()) + cell2.WalkCost;
	}

	public double Heuristic(Cell currentCell, Cell end)
	{
		return Mathf.Pow(Vector3.Distance(currentCell.Position(), end.Position()), 2) * 10f;
	}

    public bool Walkable(Cell cell)
    {
        return cell.Walkable;
    }
}
