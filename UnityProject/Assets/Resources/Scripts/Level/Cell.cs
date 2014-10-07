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

    public bool Walkable = false;

    public CellType Type = CellType.EMPTY;

    //Not used yet
    public byte Layer = 0;

}

