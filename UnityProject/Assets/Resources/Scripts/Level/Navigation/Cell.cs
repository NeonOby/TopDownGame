using System;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    VOID,
    EMPTY,
    WALL,
    SLOW
}

[System.Serializable]
public class Cell : PathFind.IHasNeighbours<Cell>, IHasPosition<Cell>
{
    public float X = 0, Z = 0;

    public bool Walkable = true;
    public int WalkCost = 1;

    public bool BreakAble = false;
    public float BreakCost = 0f;

    public CellType Type = CellType.EMPTY;

    public bool ContainsEntity = false;
    public string PoolName = "";

    //Not used yet
    public byte Layer = 0;

    public UnityEngine.Vector3 Position
    {
        get{
            return new UnityEngine.Vector3(X, 0, Z);
        }
    }

    public override string ToString()
    {
        return String.Format("Cell:{0} {1}:{2}", Walkable, X, Z);
    }

    public static implicit operator bool(Cell cell)
    {
        return cell != null;
    }

    public static float Distance(Cell cell1, Cell cell2)
    {
        return Vector3.Distance(cell1.Position, cell2.Position);
    }
    public float Distance(Cell other)
    {
        return Vector3.Distance(Position, other.Position);
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

            Cell topRight = LevelGenerator.level.GetCell((float)X + 1f, (float)Z + 1f);
            Cell bottomRight = LevelGenerator.level.GetCell((float)X + 1f, (float)Z - 1f);
            Cell topLeft = LevelGenerator.level.GetCell((float)X - 1f, (float)Z + 1f);
            Cell bottomLeft = LevelGenerator.level.GetCell((float)X - 1f, (float)Z - 1f);
            List<Cell> cells = new List<Cell>();

            if (top)
                cells.Add(top);
            if (bottom)
                cells.Add(bottom);
            if (right)
                cells.Add(right);
            if (left)
                cells.Add(left);

            if (((top && top.Walkable) || (right && right.Walkable)) && topRight && !((top && !top.Walkable) || (right && !right.Walkable)))
                cells.Add(topRight);
            if (((top && top.Walkable) || (left && left.Walkable)) && topLeft && !((top && !top.Walkable) || (left && !left.Walkable)))
                cells.Add(topLeft);

            if (((bottom && bottom.Walkable) || (right && right.Walkable)) && bottomRight && !((bottom && !bottom.Walkable) || (right && !right.Walkable)))
                cells.Add(bottomRight);
            if (((bottom && bottom.Walkable) || (left && left.Walkable)) && bottomLeft && !((bottom && !bottom.Walkable) || (left && !left.Walkable)))
                cells.Add(bottomLeft);

            return cells.ToArray();
        }
    }
}

