using System;
using System.Collections.Generic;

public enum CellType
{
    VOID,
    EMPTY,
    WALL,
    SLOW
}

[System.Serializable]
public class Cell : PathFind.IHasNeighbours<Cell>
{
    public float X = 0, Z = 0;

    public bool Walkable = true;
    public int WalkCost = 1;

    public bool BreakAble = false;
    public float BreakCost = 0f;

    public CellType Type = CellType.EMPTY;

    //Not used yet
    public byte Layer = 0;

    public UnityEngine.Vector3 Position()
    {
        return new UnityEngine.Vector3(X, 0, Z);
    }

    public override string ToString()
    {
        return String.Format("Cell:{0} {1}:{2}", Walkable, X, Z);
    }

    //TODO OPTIMIZING
    public System.Collections.Generic.IEnumerable<Cell> Neighbours
    {
        get
        {
            Cell top = LevelGenerator.level.GetCell(X, (float)Z + 1f);
            Cell bottom = LevelGenerator.level.GetCell(X, (float)Z - 1f);
            Cell right = LevelGenerator.level.GetCell((float)X + 1f, Z);
            Cell left = LevelGenerator.level.GetCell((float)X - 1f, Z);
            List<Cell> cells = new List<Cell>();
            if (top != null)
                cells.Add(top);
            if (bottom != null)
                cells.Add(bottom);
            if (right != null)
                cells.Add(right);
            if (left != null)
                cells.Add(left);
            return cells.ToArray();
        }
    }
}

