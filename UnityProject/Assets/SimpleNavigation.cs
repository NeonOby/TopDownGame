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

    public void AddOpen(CellInfo cell)
    {
        openQueue.Enqueue(0, cell);
    }
    public Cell GetFirstOpen()
    {
        return openQueue.Dequeue();
    }
    public bool EmptyOpen()
    {
        return openQueue.IsEmpty;
    }

    public void AddClosed(CellInfo cell)
    {
        closedQueue.Add(cell);
    }
    public bool ContainsClosed(CellInfo cell)
    {
        return closedQueue.Contains(cell);
    }

    public PriorityQueue<double, CellInfo> openQueue = new PriorityQueue<double, CellInfo>();
    public List<CellInfo> closedQueue = new List<CellInfo>();
}

public class CellInfo : Cell
{
    public CellInfo Parent;
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
        newGenerator.Start = (CellInfo)start;
        newGenerator.End = (CellInfo)end;
        newGenerator.CallBack = callBack;
        newGenerator.CurrentPath = new Path();

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

    public bool UpdatePath(PathToGenerate pathGenerator)
    {
        Cell currentCell = pathGenerator.GetFirstOpen();

        if (currentCell == pathGenerator.End)
        {
            if(pathGenerator.CallBack != null)
                pathGenerator.CallBack(pathGenerator.CurrentPath);
            return true;
        }

        if (pathGenerator.EmptyOpen())
            return true; //Finished but no path

        return false;
    }
}
