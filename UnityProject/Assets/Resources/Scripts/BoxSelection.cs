using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxSelection : MonoBehaviour 
{

    public static bool Selecting = false;

    public static Vector3 StartMousePos = Vector3.zero;
    public static Vector3 EndMousePos = Vector3.zero;

    public string RightClickEffectPool = "RightClickEffect";

    SearchingPath<Cell> currentFindingPath = null;
    bool finished = false;
    PathFind.Path<Cell> path = null;

    int updatesPerFrame = 50;

    float startTime = 0;
    float LastNeededTime = 0;

    void Start()
    {
        SimpleAI.SimpleAIDied += OnSimpleAIDied;
    }

    void OnSimpleAIDied(SimpleAI sender)
    {
        if (selectedGridders.Contains(sender))
        {
            selectedGridders.Remove(sender);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Selecting = true;
            StartMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Selecting = false;
            EndMousePos = Input.mousePosition;
            TrySelection();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance = 0f;
            if(!plane.Raycast(ray, out distance))
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
                    if (currentFindingPath.CallBack != null)
                        currentFindingPath.CallBack(GeneratePath(currentFindingPath, path));
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

    public Path GeneratePath(SearchingPath<Cell> pathSearcher, PathFind.Path<Cell> path)
    {
        Path newPath = new Path();
        foreach (var item in path)
        {
            newPath.AddWaypoint(item);
        }
        newPath.Destination = pathSearcher.Destination;
        return newPath;
    }

    public void Do(Vector3 pos)
    {
        if (selectedGridders.Count == 0)
            return;

        Cell start = LevelGenerator.level.GetCell(selectedGridders[0].transform.position.x, selectedGridders[0].transform.position.z);
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
        currentFindingPath.CallBack = selectedGridders[0].PathFinished;
        startTime = Time.realtimeSinceStartup;
        //path = PathFind.PathFind.FindPath<Cell>(start, end, CostBetweenNeighbors, Heuristic);
    }

    public List<SimpleAI> selectedGridders = new List<SimpleAI>();
    public Rect selectionRect;
    public void TrySelection()
    {
        CalculateStartAndEndPosition();
        selectionRect = new Rect(StartPos.x, Screen.height - StartPos.y, EndPos.x - StartPos.x, StartPos.y - EndPos.y);
        selectedGridders.Clear();
        SimpleAI[] allAIs = GameObject.FindObjectsOfType<SimpleAI>();
        for (int i = 0; i < allAIs.Length; i++)
        {
            if (TransformInSelectionBox(allAIs[i].transform, selectionRect))
            {
                selectedGridders.Add(allAIs[i]);
            }
        }
    }

    public bool TransformInSelectionBox(Transform target, Rect rect)
    {
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        targetScreenPos.y = Screen.height - targetScreenPos.y;

        return rect.Contains(targetScreenPos);
    }

    Vector2 StartPos = Vector2.zero;
    Vector2 EndPos = Vector2.zero;
    private void CalculateStartAndEndPosition()
    {
        StartPos = Vector2.zero;
        StartPos.x = StartMousePos.x < Input.mousePosition.x ? StartMousePos.x : Input.mousePosition.x;
        StartPos.y = (Screen.height - StartMousePos.y) < (Screen.height - Input.mousePosition.y) ? StartMousePos.y : Input.mousePosition.y;
        EndPos = Vector2.zero;
        EndPos.x = StartMousePos.x > Input.mousePosition.x ? StartMousePos.x : Input.mousePosition.x;
        EndPos.y = (Screen.height - StartMousePos.y) > (Screen.height - Input.mousePosition.y) ? StartMousePos.y : Input.mousePosition.y;

    }

    void OnGUI()
    {
        if (Selecting)
        {
            CalculateStartAndEndPosition();
            if (Mathf.Abs(StartPos.x - EndPos.x) > 10 && Mathf.Abs(StartPos.y - EndPos.y) > 10)
            {
                GUI.Box(new Rect(StartPos.x, Screen.height - StartPos.y, EndPos.x - StartPos.x, StartPos.y - EndPos.y), "");
            }
        }
        for (int i = 0; i < selectedGridders.Count; i++)
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(selectedGridders[i].transform.position);
            GUI.Box(new Rect(screenpos.x - 10, Screen.height - (screenpos.y + 10), 20, 20), "X");
        }
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
}
