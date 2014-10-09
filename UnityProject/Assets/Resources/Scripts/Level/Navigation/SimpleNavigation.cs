using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<P, V>
{
    private SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();
    public void Enqueue(P priority, V value)
    {
        Queue<V> q;
        if (!list.TryGetValue(priority, out q))
        {
            q = new Queue<V>();
            list.Add(priority, q);
        }
        q.Enqueue(value);
    }
    public V Dequeue()
    {
        var pair = list.First();
        var v = pair.Value.Dequeue();
        if (pair.Value.Count == 0)
            list.Remove(pair.Key);
        return v;
    }
    public bool Contains(V value)
    {
        foreach (var item in list)
        {
            if (item.Value.Contains(value))
                return true;
        }
        return false;
    }
    public bool IsEmpty
    {
        get { return !list.Any(); }
    }
}

public class PathToGenerate
{
    public Path CurrentPath;
    public CellInfo Start;
    public CellInfo End;

    public SimpleNavigation.PathFinished CallBack;

    public void AddOpen(CellInfo cellInfo)
    {
        openQueue.Enqueue(cellInfo.Cost, cellInfo);
    }
    public CellInfo GetFirstOpen()
    {
        return openQueue.Dequeue();
    }
    public bool ContainsOpen(CellInfo cellInfo)
    {
        return openQueue.Contains(cellInfo);
    }
    public bool EmptyOpen()
    {
        return openQueue.IsEmpty;
    }

    public void AddClosed(CellInfo cell)
    {
        closedQueue.Add(cell);
    }
    public bool ContainsClosed(Cell cell)
    {
        for (int i = 0; i < closedQueue.Count; i++)
        {
            if (closedQueue[i].cell == cell)
                return true;
        }
        return false;
    }

    public PriorityQueue<double, CellInfo> openQueue = new PriorityQueue<double, CellInfo>();
    public List<CellInfo> closedQueue = new List<CellInfo>();
}

public class CellInfo
{
    public Cell cell;
    public int X
    {
        get
        {
            return (int)cell.X;
        }
    }
    public int Z
    {
        get
        {
            return (int)cell.Z;
        }
    }

    public CellInfo Parent;

    public float CostFromStartToThisCell = 0f;
    public float HeuristicFromThisCellToEnd = 0f;

    public float Cost
    {
        get
        {
            return CostFromStartToThisCell + HeuristicFromThisCellToEnd;
        }
    }
}

public class SimpleNavigation : MonoBehaviour 
{
    public delegate void PathFinished(Path path);

    [SerializeField]
    public List<PathToGenerate> pathsToGenerate = new List<PathToGenerate>();

    public void GetPath(int startX, int startZ, int endX, int endZ, PathFinished callBack)
    {
        Cell start = LevelGenerator.level.GetCell(startX, startZ);
        Cell end = LevelGenerator.level.GetCell(endX, endZ);
        GetPath(start, end, callBack);
    }

    public void GetPath(Cell start, Cell end, PathFinished callBack)
    {
        PathToGenerate newGenerator = new PathToGenerate();
        newGenerator.Start = new CellInfo() { cell = start };
        newGenerator.End = new CellInfo() { cell = end };
        newGenerator.CallBack = callBack;
        newGenerator.CurrentPath = new Path();

        UpdatePath_ComputeHeuristic(newGenerator, newGenerator.Start);
        newGenerator.AddOpen(newGenerator.Start);

        pathsToGenerate.Add(newGenerator);
    }

    void Update()
    {
        for (int i = 0; i < pathsToGenerate.ToArray().Length; i++)
        {
            if (UpdatePath(pathsToGenerate[i]))
            {
                pathsToGenerate.Remove(pathsToGenerate[i]);
            }
        }
    }

    public Path GeneratePath(CellInfo end)
    {
        Path path = new Path();
        //TODO Generation
        return path;
    }

    public float DistanceBetween(Cell cell1, Cell cell2)
    {
        return Level.PosDistance(cell1.X, cell1.Z, cell2.X, cell2.Z);
    }

    public bool UpdatePath(PathToGenerate pathGen)
    {
        CellInfo currentCell = pathGen.GetFirstOpen();

        if (currentCell.cell == pathGen.End.cell)
        {
            pathGen.CurrentPath = GeneratePath(pathGen.End);
            if(pathGen.CallBack != null)
                pathGen.CallBack(pathGen.CurrentPath);
            return true; //Finished with path
        }

        pathGen.AddClosed(currentCell);

        UpdatePath_CellDirections(pathGen, currentCell);

        if (pathGen.EmptyOpen())
            return true; //Finished but no path

        return false;
    }

    public void UpdatePath_ComputeHeuristic(PathToGenerate pathGen, CellInfo cellInfo)
    {
        cellInfo.HeuristicFromThisCellToEnd = DistanceBetween(cellInfo.cell, pathGen.End.cell);
    }

    //If we want to go directional we can add this here with penalty
    //Just add ", 0.5f" to CellDirection Method
    public void UpdatePath_CellDirections(PathToGenerate pathGen, CellInfo cellInfo)
    {
        Cell top = LevelGenerator.level.GetCell(cellInfo.X, cellInfo.Z + 1);
        Cell bottom = LevelGenerator.level.GetCell(cellInfo.X, cellInfo.Z - 1);
        Cell right = LevelGenerator.level.GetCell(cellInfo.X + 1, cellInfo.Z);
        Cell left = LevelGenerator.level.GetCell(cellInfo.X - 1, cellInfo.Z);

        UpdatePath_CellDirection(pathGen, cellInfo, top);
        UpdatePath_CellDirection(pathGen, cellInfo, bottom);
        UpdatePath_CellDirection(pathGen, cellInfo, right);
        UpdatePath_CellDirection(pathGen, cellInfo, left);
    }

    public void UpdatePath_CellDirection(PathToGenerate pathGen, CellInfo cellInfo, Cell nextCell, float penalty = 0f)
    {
        if (nextCell == null)
            return;

        if (pathGen.ContainsClosed(nextCell))
            return;

        CellInfo nextCellInfo = new CellInfo() { cell = nextCell };
        UpdatePath_ComputeHeuristic(pathGen, nextCellInfo);

        float EdgeCost = 1.0f + penalty;
        float CostUntilNow = cellInfo.CostFromStartToThisCell + EdgeCost;

        if (pathGen.ContainsOpen(nextCellInfo) && CostUntilNow >= nextCellInfo.CostFromStartToThisCell)
        {
            //This means that we have checked this node sometime before
            //AND the cost to get from the start to it is higher than the current path
            return;
        }

        float HeuristicCostForThisCell = CostUntilNow + nextCellInfo.HeuristicFromThisCellToEnd;
        nextCellInfo.Parent = cellInfo;
        nextCellInfo.CostFromStartToThisCell = CostUntilNow;

    }
}
