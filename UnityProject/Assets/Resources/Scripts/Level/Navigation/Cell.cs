using System;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Cell : IHasNeighbours<Cell>
{
    public float X = 0, Z = 0;
    public float Y = 0;

    public bool Walkable = true;
    public int WalkCost = 1;

    public bool BreakAble = false;
    public float BreakCost = 0f;

    public bool ContainsEntity = false;
    public LevelEntity Entity = null;

    //Not used yet
    public byte Layer = 0;

    public UnityEngine.Vector3 Position
    {
        get{
            return new UnityEngine.Vector3(X, Y, Z);
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

    //TODO OPTIMIZING Cell Distance
    public static float Distance(Cell cell1, Cell cell2)
    {
        return Vector3.Distance(cell1.Position, cell2.Position);
    }
    public float Distance(Cell other)
    {
        return Vector3.Distance(Position, other.Position);
    }

    //Beim Generieren und Ändern eines Chunks
    public void ChunkChanged()
    {
        Cell top = LevelGenerator.level.GetCell(X, (float)Z + 1f, false);
        Cell bottom = LevelGenerator.level.GetCell(X, (float)Z - 1f, false);
        Cell right = LevelGenerator.level.GetCell((float)X + 1f, Z, false);
        Cell left = LevelGenerator.level.GetCell((float)X - 1f, Z, false);

        Cell topRight = LevelGenerator.level.GetCell((float)X + 1f, (float)Z + 1f, false);
        Cell bottomRight = LevelGenerator.level.GetCell((float)X + 1f, (float)Z - 1f, false);
        Cell topLeft = LevelGenerator.level.GetCell((float)X - 1f, (float)Z + 1f, false);
        Cell bottomLeft = LevelGenerator.level.GetCell((float)X - 1f, (float)Z - 1f, false);

        AddNeighbour(top);
        AddNeighbour(bottom);
        AddNeighbour(right);
        AddNeighbour(left);

        AddNeighbour(topRight);
        AddNeighbour(bottomRight);
        AddNeighbour(topLeft);
        AddNeighbour(bottomLeft);

        /*
         * Check to avoid block edges
         * Shouldn't be part of this, should be somewhere else ?
         * (Could be hard to calculate somewhere else)
         * 
        if (((top && top.Walkable) || (right && right.Walkable)) && topRight && !((top && !top.Walkable) || (right && !right.Walkable)))
            AddNeighbour(topRight);
        if (((top && top.Walkable) || (left && left.Walkable)) && topLeft && !((top && !top.Walkable) || (left && !left.Walkable)))
            AddNeighbour(topLeft);

        if (((bottom && bottom.Walkable) || (right && right.Walkable)) && bottomRight && !((bottom && !bottom.Walkable) || (right && !right.Walkable)))
            AddNeighbour(bottomRight);
        if (((bottom && bottom.Walkable) || (left && left.Walkable)) && bottomLeft && !((bottom && !bottom.Walkable) || (left && !left.Walkable)))
            AddNeighbour(bottomLeft);
        */

        foreach (var item in neighbours)
        {
            item.AddNeighbour(this);
        }

    }

    public void AddNeighbour(Cell cell)
    {
        if (!cell)
            return;
        if (neighbours.Contains(cell))
            return;

        neighbours.Add(cell);
    }

    public List<Cell> neighbours = new List<Cell>();


    public IEnumerable<Cell> Neighbours
    {
        get
        {
            return neighbours;
        }
        set
        {
            
        }
    }
}

