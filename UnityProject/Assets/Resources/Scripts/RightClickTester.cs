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

	HashSet<Node> Closed;
	PriorityQueue<double, PathFind.Path<Node>> Queue;

	public SearchingPath(
			Node start,
			Node destination,
			Func<Node, Node, double> costBetweenTwo,
			Func<Node, Node, double> heuristic)
	{
		Start = start;
		Destination = destination;

		CostBetweenTwo = costBetweenTwo;
		Heuristic = heuristic;

		Closed = new HashSet<Node>();
		Queue = new PriorityQueue<double, PathFind.Path<Node>>();

		Queue.Enqueue(0, new PathFind.Path<Node>(start));
	}

	public PathFind.Path<Node> finishedPath = null;

	public bool NextStep()
	{
		if (Queue.IsEmpty)
			return true;

		var path = Queue.Dequeue();

		if (Closed.Contains(path.LastStep))
			return false;
		if (path.LastStep.Equals(Destination))
		{
			finishedPath = path;
			return true;
		}

		Closed.Add(path.LastStep);

		foreach (Node currentNode in path.LastStep.Neighbours)
		{
			double d = CostBetweenTwo(path.LastStep, currentNode);
			var newPath = path.AddStep(currentNode, d);
			Queue.Enqueue(newPath.TotalCost + Heuristic(currentNode, Destination), newPath);
		}
		return false;
	}

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
			double d = CostBetweenTwo(path.LastStep, currentNode);
			var newPath = path.AddStep(currentNode, d);
			Queue.Enqueue(newPath.TotalCost + Heuristic(currentNode, Destination), newPath);
		}
		return path;
	}
}


public class RightClickTester : MonoBehaviour 
{

	PathFind.Path<Cell> path = null;


	SearchingPath<Cell> currentFindingPath = null;
	bool finished = false;

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
			/*
			if (currentFindingPath.NextStep())
			{
				path = currentFindingPath.finishedPath;
			}
			*/
		}

		
		if (path != null && path.PreviousSteps != null)
		{
			Vector3 lastPos = path.LastStep.Position();
			lastPos.y = 0;
			foreach (var item in path)
			{
				Vector3 currentPos = item.Position();
				currentPos.y = 1;
				Debug.DrawLine(lastPos, currentPos);
				lastPos = currentPos;
			}
		}
	}

	void OnGUI()
	{
		GUILayout.Label(LastNeededTime.ToString());
	}

	public void DebugAllNeighbors(Cell cell)
	{
		foreach (var item in cell.Neighbours)
		{
			Debug.Log(item);
		}
	}

	public void Do(Vector3 pos)
	{
		//Debug.Log(System.String.Format("Pos: {0}:{1} {2}", pos.x, pos.z, LevelGenerator.level.GetCell(pos.x, pos.z)));
		//DebugAllNeighbors(LevelGenerator.level.GetCell(pos.x, pos.z));
		//return;

		path = null;
		finished = false;
		Cell start = LevelGenerator.level.GetCell(0, 0);
		Cell end = LevelGenerator.level.GetCell(pos.x, pos.z);
		if (end == null)
			return;

		currentFindingPath = new SearchingPath<Cell>(start, end, CostBetweenNeighbors, Heuristic);
		startTime = Time.realtimeSinceStartup;
		//path = PathFind.PathFind.FindPath<Cell>(start, end, CostBetweenNeighbors, Heuristic);
	}

	public double CostBetweenNeighbors(Cell cell1, Cell cell2)
	{
		return Vector3.Distance(cell1.Position(), cell2.Position()) + cell2.WalkCost;
	}

	public double Heuristic(Cell currentCell, Cell end)
	{
		return Mathf.Pow(Vector3.Distance(currentCell.Position(), end.Position()), 2);
	}
}
