using System;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Cell
{
    public float X = 0, Z = 0;
    public float Y = 0;

    private bool walkAble = true;
    public bool Walkable
    {
        get
        {
            if (!walkAble) return false;
            if (ContainsEntity || ContainsLevelEntity) return false;
            return true;
        }
    }
    public int WalkCost = 1;

    public bool BreakAble = false;
    public float BreakCost = 0f;

    public bool ContainsEntity
    {
        get
        {
            return EntityCount > 0;
        }
    }
    public int EntityCount = 0;

    public bool ContainsLevelEntity
    {
        get
        {
            return LevelEntity != null;
        }
    }
    public LevelEntity LevelEntity = null;

    //Not used yet
    public byte Layer = 0;

    public void EntityEnter(Entity entity)
    {
        bool was = Walkable;
        EntityCount++;
        if (was)
        {
            //DirectNeighbourEntity(1);
        }
    }
    public void EntityLeave(Entity entity)
    {
        EntityCount--;
        if (Walkable)
        {
            //DirectNeighbourEntity(-1);
        }
    }

    public void DirectNeighbourEntity(int value)
    {
        Cell top = LevelGenerator.Level.GetCell(X, (float)Z + 1f, false);
        Cell bottom = LevelGenerator.Level.GetCell(X, (float)Z - 1f, false);
        Cell right = LevelGenerator.Level.GetCell((float)X + 1f, Z, false);
        Cell left = LevelGenerator.Level.GetCell((float)X - 1f, Z, false);

        if (top)
            top.EntityCount += value;
        if (bottom)
            bottom.EntityCount += value;
        if (right)
            right.EntityCount += value;
        if (left)
            left.EntityCount += value;
    }

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
        Cell top = LevelGenerator.Level.GetCell(X, (float)Z + 1f, false);
        Cell bottom = LevelGenerator.Level.GetCell(X, (float)Z - 1f, false);
        Cell right = LevelGenerator.Level.GetCell((float)X + 1f, Z, false);
        Cell left = LevelGenerator.Level.GetCell((float)X - 1f, Z, false);

        Cell topRight = LevelGenerator.Level.GetCell((float)X + 1f, (float)Z + 1f, false);
        Cell bottomRight = LevelGenerator.Level.GetCell((float)X + 1f, (float)Z - 1f, false);
        Cell topLeft = LevelGenerator.Level.GetCell((float)X - 1f, (float)Z + 1f, false);
        Cell bottomLeft = LevelGenerator.Level.GetCell((float)X - 1f, (float)Z - 1f, false);

        AddNeighbour(top);
        AddNeighbour(bottom);
        AddNeighbour(right);
        AddNeighbour(left);

        /*
        AddNeighbour(topRight);
        AddNeighbour(bottomRight);
        AddNeighbour(topLeft);
        AddNeighbour(bottomLeft);
        */

        /*
         * Check to avoid block edges
         * Shouldn't be part of this, should be somewhere else ?
         * (Could be hard to calculate somewhere else)
        */

        if (((top && top.Walkable) || (right && right.Walkable)) && topRight && !((top && !top.Walkable) || (right && !right.Walkable)))
            AddNeighbour(topRight);
        if (((top && top.Walkable) || (left && left.Walkable)) && topLeft && !((top && !top.Walkable) || (left && !left.Walkable)))
            AddNeighbour(topLeft);

        if (((bottom && bottom.Walkable) || (right && right.Walkable)) && bottomRight && !((bottom && !bottom.Walkable) || (right && !right.Walkable)))
            AddNeighbour(bottomRight);
        if (((bottom && bottom.Walkable) || (left && left.Walkable)) && bottomLeft && !((bottom && !bottom.Walkable) || (left && !left.Walkable)))
            AddNeighbour(bottomLeft);
        

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

