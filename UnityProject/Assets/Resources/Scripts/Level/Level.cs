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

    public Level(int seed)
    {
        Seed = seed;
        random = new System.Random(Seed);
        chunks = new Dictionary<string, Chunk>();
    }

    public void Clear()
    {
        chunks.Clear();
    }

    public Cell GetCell(float x, float z)
    {
        Point cellPoint = GetCellPoint(x, z);
        Point chunkPoint = GetChunkPoint(x, z);

        if (!ContainsChunk(chunkPoint.X, chunkPoint.Y))
        {
            return null;
        }

        //Debug.Log(String.Format("Chunk: {0}:{1} found, looking for {2}:{3}", chunkX, chunkZ, cellX, cellZ));
        return chunks[GetKey(chunkPoint.X, chunkPoint.Y)].GetCell(cellPoint.X, cellPoint.Y);
    }

    int NextPowerOfTwo(int x)
    {
        int power = 1;
        while (power < x)
            power *= 2;
        return power;
    }

    public bool PositionOutOfLevel(float x, float z)
    {
        if (x <= lowestX * LevelGenerator.ChunkSize || x >= (highestX) * LevelGenerator.ChunkSize)
            return true;
        if (z <= lowestZ * LevelGenerator.ChunkSize || z >= (highestZ) * LevelGenerator.ChunkSize)
            return true;

        return false;
    }

    public Cell[,] GetCurrentGrid()
    {
        int width = (highestX) - (lowestX);
        int height = (highestZ) - (lowestZ);
        width *= LevelGenerator.ChunkSize;
        height *= LevelGenerator.ChunkSize;

        width = NextPowerOfTwo(width);
        height = NextPowerOfTwo(height);

        int lowestXCells = lowestX * LevelGenerator.ChunkSize;
        int lowestZCells = lowestZ * LevelGenerator.ChunkSize;

        Debug.Log(String.Format("GridGenerated: MinX:{0} MaxX:{1} MinZ:{2} MaxZ:{3}", lowestXCells, lowestZCells, lowestXCells + width, lowestZCells + height));

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
    }

    public void UpdateChunks()
    {
        float CameraPosX = Camera.main.transform.position.x;
        float CameraPosZ = Camera.main.transform.position.z;

        float centerX = (CameraPosX / LevelGenerator.ChunkSize);
        float centerZ = (CameraPosZ / LevelGenerator.ChunkSize);

        float chunkX = 0, chunkZ = 0;

        for (int x = (Mathf.RoundToInt(centerX) - LevelGenerator.ChunkLoadDistance) - 1; x <= Mathf.RoundToInt(centerX) + LevelGenerator.ChunkLoadDistance; x++)
        {
            for (int z = (Mathf.RoundToInt(centerZ) - LevelGenerator.ChunkLoadDistance) - 1; z <= Mathf.RoundToInt(centerZ) + LevelGenerator.ChunkLoadDistance; z++)
            {
                if (Level.PosDistance(centerX, centerZ, x, z) >= LevelGenerator.ChunkLoadDistance)
                    continue;
                if (!ContainsChunk(x, z))
                {
                    AddChunk(x, z, LevelGenerator.GenerateChunk(Seed, x, z));
                }

            }
        }

        foreach (var chunkInfo in chunks)
        {
            string chunkKey = chunkInfo.Key;
            string[] args = chunkKey.Split(split);
            if (args.Length != 2)
                continue;
            if (!float.TryParse(args[0], out chunkX))
                continue;
            if (!float.TryParse(args[1], out chunkZ))
                continue;

            if (PosDistance(centerX, centerZ, chunkX, chunkZ) < LevelGenerator.ChunkLoadDistance)
            {
                if (!ContainsChunk((int)chunkX, (int)chunkZ))
                {
                    AddChunk((int)chunkX, (int)chunkZ, LevelGenerator.GenerateChunk(Seed, (int)chunkX, (int)chunkZ));
                }
                chunkInfo.Value.Load();
            }
            else
            {
                chunkInfo.Value.Unload();
            }
        }
    }

    public Point GetCellPoint(float x, float z)
    {
        Point point = new Point((int)x, (int)z);
        point.X = point.X % LevelGenerator.ChunkSize;
        point.Y = point.Y % LevelGenerator.ChunkSize;
        point.X = x < 0 ? LevelGenerator.ChunkSize + point.X : point.X;
        point.Y = z < 0 ? LevelGenerator.ChunkSize + point.Y : point.Y;
        point.X = point.X == LevelGenerator.ChunkSize ? point.X - 1 : point.X;
        point.Y = point.Y == LevelGenerator.ChunkSize ? point.Y - 1 : point.Y;
        return point;
    }
    public Point GetChunkPoint(float x, float z)
    {
        Point point = new Point((int)x, (int)z);
        point.X = (int)(x / LevelGenerator.ChunkSize);
        point.Y = (int)(z / LevelGenerator.ChunkSize);
        point.X = x < 0 ? point.X - 1 : point.X;
        point.Y = z < 0 ? point.Y - 1 : point.Y;
        return point;
    }

    public bool ContainsChunk(float x, float z)
    {
        Point p = GetChunkPoint(x, z);
        return ContainsChunk(p.X, p.Y);
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
