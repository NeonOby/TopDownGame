using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RightClickTester : MonoBehaviour 
{

    SearchingPath currentFindingPath = null;
    bool finished = false;
	PathNode path = null;

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
			Vector3 lastPos = path.LastStep.Position;
			lastPos.y = 0;
			foreach (var item in path)
			{
				Vector3 currentPos = item.Position;
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

		currentFindingPath = new SearchingPath(start, end);
		startTime = Time.realtimeSinceStartup;
		//path = PathFind.PathFind.FindPath<Cell>(start, end, CostBetweenNeighbors, Heuristic);
	}

    public float Distance(Cell cell1, Cell cell2)
    {
        return Vector3.Distance(cell1.Position, cell2.Position);
    }

	public double CostBetweenNeighbors(Cell cell1, Cell cell2)
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
