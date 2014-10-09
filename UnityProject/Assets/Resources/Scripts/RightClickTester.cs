using UnityEngine;
using System.Collections;

public class RightClickTester : MonoBehaviour 
{

    PathFind.Path<Cell> path = null;

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
        Cell start = LevelGenerator.level.GetCell(0, 0);
        Cell end = LevelGenerator.level.GetCell(pos.x, pos.z);
        if (end == null)
            return;
        path = PathFind.PathFind.FindPath<Cell>(start, end, CostBetweenNeighbors, Heuristic);
    }

    public double CostBetweenNeighbors(Cell cell1, Cell cell2)
    {
        return Vector3.Distance(cell1.Position(), cell2.Position());
    }

    public double Heuristic(Cell currentCell, Cell end)
    {
        return Vector3.Distance(currentCell.Position(), end.Position());
    }
}
