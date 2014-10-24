using UnityEngine;
using System.Collections;


/*
 * Everything using RightClick
 * 
 */
public class RightClickAction : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}

    SearchingPath currentFindingPath = null;
    bool finished = false;
    PathNode path = null;

    int updatesPerFrame = 50;

    float startTime = 0;
    float LastNeededTime = 0;

    public Vector3 lastClickedPosition = Vector3.zero;

    public string RightClickEffectPool = "RightClickEffect";

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(1) && GridSystem.GridPositionUnderMouse(out lastClickedPosition, Camera.main))
        {
            DoRightClick();
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
                    {
                        currentFindingPath.CallBack(currentFindingPath.Owner, GeneratePath(currentFindingPath, path));
                    }

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

    public Path GeneratePath(SearchingPath pathSearcher, PathNode path)
    {
        Path newPath = new Path();
        foreach (var item in path)
        {
            newPath.AddWaypoint(item);
        }
        newPath.Destination = pathSearcher.Destination;
        return newPath;
    }

    public void DoRightClick()
    {
        if (MouseSelection.State == MouseSelection.States.NOTHING_SELECTED)
            return;

        Cell start = LevelGenerator.level.GetCell(MouseSelection.GetSelected()[0].transform.position.x, MouseSelection.GetSelected()[0].transform.position.z);

        if (!start)
            return;
        
        Cell end = LevelGenerator.level.GetCell(lastClickedPosition.x, lastClickedPosition.z);
        if (!end || !end.Walkable)
        {
            end = FindNeighborWalkableCell(end, start);
        }

        if (!end || !end.Walkable)
            return;

        path = null;
        finished = false;

        currentFindingPath = new SearchingPath(start, end);
        if (MouseSelection.State == MouseSelection.States.SINGLE_SELECTED && MouseSelection.GetSelected()[0] is Worker)
        {
            currentFindingPath.CallBack = ((Worker)MouseSelection.GetSelected()[0]).PathFinished;
        }
        else
        {
            //Multiple Selection
            //TODO Group Flocking
        }
        startTime = Time.realtimeSinceStartup;

        //Effekt
        GameObjectPool.Instance.Spawn(RightClickEffectPool, lastClickedPosition, Quaternion.identity);
    }

    public Cell FindNeighborWalkableCell(Cell cell, Cell start)
    {
        float minDistance = -1f;
        Cell foundCell = null;
        foreach (var neighbor in cell.Neighbours)
        {
            if (neighbor.Walkable)
            {
                if (minDistance == -1 || start.Distance(neighbor) < minDistance)
                {
                    foundCell = neighbor;
                    minDistance = Cell.Distance(neighbor, start);
                }
            }
        }
        return foundCell;
    }

}
