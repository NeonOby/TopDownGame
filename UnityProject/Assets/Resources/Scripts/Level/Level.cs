using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Level 
{
    private char split = '.';

    public System.Random random;

    public Dictionary<string, Chunk> chunks;

    public int Seed = 0;

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

    public void AddChunk(int x, int z, Chunk chunk)
    {
        AddChunk(System.String.Format("{0}{1}{2}", x, split, z), chunk);
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

        int camX = (int)(CameraPosX / LevelGenerator.ChunkSize);
        int camZ = (int)(CameraPosZ / LevelGenerator.ChunkSize);

        float chunkX = 0, chunkZ = 0;

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

            //chunkX += 0.5f;
            //chunkZ += 0.5f;


            if (PosDistance(camX, camZ, (int)chunkX, (int)chunkZ) < LevelGenerator.ChunkLoadDistance)
            {
                chunkInfo.Value.Load();
            }
            else
            {
                chunkInfo.Value.Unload();
            }
        }
    }

    public bool ContainsChunk(int x, int z)
    {
        return ContainsChunk(System.String.Format("{0}{1}{2}", x, split, z));
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
