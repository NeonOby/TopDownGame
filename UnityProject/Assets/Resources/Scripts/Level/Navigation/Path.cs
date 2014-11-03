using System.Collections.Generic;
public class Path
{
    public List<Cell> path = new List<Cell>();

    public int currentIndex = -1;

    public void AddWaypoint(Cell cell)
    {
        path.Add(cell);
    }

    public bool IsEmpty
    {
        get
        {
            return path.Count == 0;
        }
    }
    public bool IsLast
    {
        get
        {
            return currentIndex == 0;
        }
    }

    public Cell GetLast()
    {
        if (IsEmpty)
            return null;
        return path[path.Count-1];
    }

    public Cell GetNext()
    {
        if (path.Count == 0)
            return null;
        if (currentIndex == 0)
            return null;
        if (currentIndex == -1)
            currentIndex = path.Count - 1;
        else
            currentIndex--;

        if (currentIndex < 0 || currentIndex > path.Count)
            return null;

        return path[currentIndex];
    }

    public Cell Destination { get; set; }
}