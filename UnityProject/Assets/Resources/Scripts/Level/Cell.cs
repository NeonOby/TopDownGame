using System;

public enum CellType
{
    VOID,
    EMPTY,
    WALL,
    SLOW
}

[System.Serializable]
public class Cell
{
    public int X = 0, Z = 0;

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

}

