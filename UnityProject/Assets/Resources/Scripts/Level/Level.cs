using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class Level 
{
    public static char split = '.';

    public System.Random random;

    public Dictionary<string, Chunk> chunks;

    public int Seed = 0;

    public int lowestX = 0, highestX = 0;
    public int lowestZ = 0, highestZ = 0;

    public float randomizedMapPositionX = 0, randomizedMapPositionZ = 0;

    public List<Chunk> loadedChunks = new List<Chunk>();

    public Level(int seed)
    {
        Seed = seed;
        random = new System.Random(Seed);
        chunks = new Dictionary<string, Chunk>();

        randomizedMapPositionX = random.Next(-20000, 20000);
        randomizedMapPositionZ = random.Next(-20000, 20000);
    }

    public void Clear()
    {
        chunks.Clear();
    }

    public Cell GetCell(float x, float z, bool autoGen = true)
    {
        if (!ContainsChunkf(x, z))
        {
            if (!autoGen)
                return null;
            return null;
        }

        Vector3 cellPoint = GetCellPoint(x, z);
        return chunks[GetKey(x, z)].GetCell(cellPoint.x, cellPoint.z);
    }

    public bool PositionOutOfLevel(float x, float z)
    {
        if (x <= lowestX * LevelGenerator.ChunkSize || x >= (highestX) * LevelGenerator.ChunkSize)
            return true;
        if (z <= lowestZ * LevelGenerator.ChunkSize || z >= (highestZ) * LevelGenerator.ChunkSize)
            return true;

        return false;
    }

    //Astar "PathFinderFast.cs" was using this
    //Copies complete grid to two dimensional Cell array
    //to sloooooooooow
    [Obsolete]
    public Cell[,] GetCurrentGrid()
    {
        int width = (highestX) - (lowestX);
        int height = (highestZ) - (lowestZ);
        width *= LevelGenerator.ChunkSize;
        height *= LevelGenerator.ChunkSize;

        width = Mathf.NextPowerOfTwo(width);
        height = Mathf.NextPowerOfTwo(height);

        int lowestXCells = lowestX * LevelGenerator.ChunkSize;
        int lowestZCells = lowestZ * LevelGenerator.ChunkSize;

        Cell[,] grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Cell cell = GetCell(x + lowestXCells, z + lowestZCells);
                if (cell == null)
                    cell = new Cell() { Walkable = false };
                grid[x, z] = cell;
            }
        }
        return grid;
    }

    public string GetKey(float x, float z)
    {
        Vector3 chunkPoint = GetChunkPoint(x, z);
        return GetKey((int)chunkPoint.x, (int)chunkPoint.z);
    }

    public string GetKey(int x, int z)
    {
        return System.String.Format("{0}{1}{2}", x, split, z);
    }

    public void AddChunk(int x, int z, Chunk chunk)
    {
        if (x < lowestX)
            lowestX = x;
        if (x > highestX)
            highestX = x+1;
        if (z < lowestZ)
            lowestZ = z;
        if (z > highestZ)
            highestZ = z+1;
        
        AddChunk(GetKey(x, z), chunk);
    }

    private void AddChunk(string key, Chunk chunk)
    {
        if(chunks.ContainsKey(key))
        {
            Debug.Log("Chunk already exists");
            return;
        }
        chunks.Add(key, chunk);

        chunk.UpdateCellNeighbours();
    }

    public Cell FindNeighborWalkableCell(Cell cell, Cell start)
    {
        float minDistance = -1f;
        Cell foundCell = null;
        foreach (var neighbor in cell.Neighbours)
        {
            if (neighbor.Walkable)
            {
                if (minDistance == -1 || Cell.Distance(neighbor, start) < minDistance)
                {
                    foundCell = neighbor;
                    minDistance = Cell.Distance(neighbor, start);
                }
            }
        }
        return foundCell;
    }

    public Vector3 GetCellPoint(float x, float z)
    {
        Vector3 point = new Vector3((int)x, 0, (int)z);
        point.x = point.x % LevelGenerator.ChunkSize;
        point.z = point.z % LevelGenerator.ChunkSize;
        point.x = x < 0 ? (LevelGenerator.ChunkSize - 1) - Mathf.Abs(point.x) : point.x;
        point.z = z < 0 ? (LevelGenerator.ChunkSize - 1) - Mathf.Abs(point.z) : point.z;
        return point;
    }
    public Vector3 GetChunkPoint(float x, float z)
    {
        Vector3 point = new Vector3((int)x, 0, (int)z);
        point.x = (int)(x / LevelGenerator.ChunkSize);
        point.z = (int)(z / LevelGenerator.ChunkSize);
        point.x = x < 0 ? point.x - 1 : point.x;
        point.z = z < 0 ? point.z - 1 : point.z;
        return point;
    }

    public bool ContainsChunkf(float x, float z)
    {
        Vector3 p = GetChunkPoint(x, z);
        return ContainsChunk((int)p.x, (int)p.z);
    }
    public bool ContainsChunk(int x, int z)
    {
        return ContainsChunk(GetKey(x, z));
    }
    private bool ContainsChunk(string key)
    {
        if (chunks == null)
            return false;
        return chunks.ContainsKey(key);
    }

    private static int MaxDist(int x1, int y1, int x2, int y2)
    {
        int max = 0;
        max = Mathf.Max(Mathf.Abs(x1 - x2), max);
        max = Mathf.Max(Mathf.Abs(y1 - y2), max);
        
        return max;
    }

    

    public static float PosDistance(float x1, float y1, float x2, float y2)
    {
        return Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));
    }

    public void UnloadEverything()
    {
        if (chunks == null)
            return;
        foreach (var item in chunks)
        {
            item.Value.Unload();
        }
    }
}
