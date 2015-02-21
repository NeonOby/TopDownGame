using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using SimpleLibrary;

public class LevelGenerator : Singleton<LevelGenerator>
{
    public int chunkSize = 16;
    public int chunkLoadDistance = 4;

    public float safeDistance = 2f;

	public PoolInfo LoadedChunk;
	public PoolInfo UnloadedChunk;
	public PoolInfo ResourceBlock;
	public PoolInfo ResourceCube;
	public PoolInfo PlayerBase;


    #region ThreadedMapGeneration
    private static volatile System.Object fastLock = new System.Object();

    public static float SafeDistance
    {
        get
        {
            lock (fastLock)
            {
                return instance.safeDistance;
            }
        }
    }

    public static int ChunkSize
    {
        get
        {
            lock (fastLock)
            {
                return instance.chunkSize;
            }
        }
        set
        {
            lock (fastLock)
            {
                instance.chunkSize = value;
            }
        }
    }
    public static int ChunkLoadDistance
    {
        get
        {
            lock (fastLock)
            {
                return instance.chunkLoadDistance;
            }
        }
        set
        {
            lock (fastLock)
            {
                instance.chunkLoadDistance = value;
            }
        }
    }

    public static float SeedXPosition
    {
        get
        {
            lock (fastLock)
            {
                return Level.randomizedMapPositionX;
            }
        }
    }

    public static float SeedZPosition
    {
        get
        {
            lock (fastLock)
            {
                return Level.randomizedMapPositionZ;
            }
        }
    }
    #endregion

    private static LevelGenerator instance;
    public static LevelGenerator Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    private static System.Object levelLock = new System.Object();
    private static Level lev = null;

    public static Level Level
    {
        get
        {
            lock (levelLock)
            {
                return lev;
            }
        }
        set
        {
            lock (levelLock)
            {
                lev = value;
            }
        }
    }

    public string[] DespawnPoolsOnLoad;

    public float ChunkUpdateTime = 1f;
    private float ChunkUpdateTimer = 0f;

    private static System.Object genLock = new System.Object();
    private static List<ChunkGenerator> generators = new List<ChunkGenerator>();
    public static List<ChunkGenerator> Generators
    {
        get
        {
            lock (genLock)
            {
                return generators;
            }
        }
        set
        {
            lock (genLock)
            {
                generators = value;
            }
        }
    }



    void Start()
    {
        GenerateLevel(0);
    }

    public bool ContainsGenerator(int x, int z)
    {
        foreach (var item in Generators)
        {
            if (item.ChunkX == x && item.ChunkZ == z)
                return true;
        }
        return false;
    }

    public void AddChunkGenerator(int x, int z)
    {
        if (!Level.ContainsChunk(x, z) && !ContainsGenerator(x, z))
        {
            Generators.Add(new ChunkGenerator(x, z, Level.Seed));
        }
    }

    Thread UpdateChunksThread = null;

    void Update()
    {
        if (Level == null)
            return;

        ChunkUpdateTimer += Time.deltaTime;
        if (!CheckingChunks && ChunkUpdateTimer > ChunkUpdateTime)
        {
            ChunkUpdateTimer = 0f;
            if (UpdateChunksThread == null || !UpdateChunksThread.IsAlive)
            {
                float CameraPosX = Camera.main.transform.position.x;
                float CameraPosZ = Camera.main.transform.position.z;

                CurrentCenterX = (CameraPosX / ChunkSize);
                CurrentCenterZ = (CameraPosZ / ChunkSize);

                CheckingChunks = true;
                UpdateChunksThread = new Thread(CheckChunks);
                UpdateChunksThread.Start();
            }
        }

        /*
        foreach (var generator in Generators.ToArray())
        {
            if (!generator.Generating)
            {
                Chunk chunk = generator.GeneratedChunk;
                Level.AddChunk(chunk.posX, chunk.posZ, chunk);
                Generators.Remove(generator);
                chunk.finishedGenerating = true;
            }
        }
        */
    }

    public static void GeneratorFinished(ChunkGenerator generator)
    {
        Chunk chunk = generator.GeneratedChunk;
        chunk.finishedGenerating = true;
        Level.AddChunk(chunk.posX, chunk.posZ, chunk);
        Generators.Remove(generator);
    }

    float CurrentCenterX = 0f;
    float CurrentCenterZ = 0f;
    bool CheckingChunks = false;

    public void LoadLevel(Level newLevel)
    {
        if (Level != null)
        {
            Level.UnloadEverything();
        }

        Level = newLevel;
    }

    public float DistanceToCam(Chunk chunk)
    {
        return DistanceToCam(chunk.posX, chunk.posZ);
    }

    public float DistanceToCam(int x, int z)
    {
        float CameraPosX = Camera.main.transform.position.x;
        float CameraPosZ = Camera.main.transform.position.z;

        float centerX = (CameraPosX / ChunkSize);
        float centerZ = (CameraPosZ / ChunkSize);

        int chunkX = x < 0 ? x + 1 : x;
        int chunkZ = z < 0 ? z + 1 : z;

        return Level.PosDistance(centerX, centerZ, chunkX, chunkZ);
    }

    public void CheckChunks()
    {
        for (int i = 0; i < Level.loadedChunks.Count; i++)
        {
            if (Level.PosDistance(CurrentCenterX, CurrentCenterZ, Level.loadedChunks[i].posX, Level.loadedChunks[i].posZ) >= LevelGenerator.ChunkLoadDistance)
            {
                if (Level.loadedChunks[i].Loaded)
                {
                    PriorityWorker_Chunk_Unload.Create(Level.loadedChunks[i], ChunkUnloaded); 
                }
            }
        }

        for (int x = (Mathf.RoundToInt(CurrentCenterX) - LevelGenerator.ChunkLoadDistance); x < Mathf.RoundToInt(CurrentCenterX) + LevelGenerator.ChunkLoadDistance; x++)
        {
            for (int z = (Mathf.RoundToInt(CurrentCenterZ) - LevelGenerator.ChunkLoadDistance); z < Mathf.RoundToInt(CurrentCenterZ) + LevelGenerator.ChunkLoadDistance; z++)
            {
                if (Level.PosDistance(CurrentCenterX, CurrentCenterZ, x, z) >= LevelGenerator.ChunkLoadDistance)
                    continue;

                AddChunkGenerator(x, z);
                if (Level.ContainsChunk(x, z))
                {
                    Chunk currentChunk = Level.chunks[Level.GetKey(x, z)];
                    if (!currentChunk.Loaded)
                    {
                        PriorityWorker_Chunk_Load.Create(currentChunk, ChunkLoaded);   
                    }                 
                }
            }
        }
        CheckingChunks = false;
    }

    public void ChunkLoaded(Chunk chunk)
    {
        Level.loadedChunks.Add(chunk);
    }
    public void ChunkUnloaded(Chunk chunk)
    {
        Level.loadedChunks.Remove(chunk);
    }

    #region Noise
    private static float persistence = 1f;
    private static int NumberOfOctaves = 2;

    public static float NoiseN(float x, float y)
    {
        return Mathf.PerlinNoise(x, y);
    }

    public static float Noise(int x, int y)
    {
        long n = x + (y * 57);
        n = (long)((n << 13) ^ n);
        return (float)(1.0F - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
    }

    public static float SmoothNoise(float x, float y)
    {
        float corners = (Noise((int)(x - 1), (int)(y - 1)) + Noise((int)(x + 1), (int)(y - 1)) + Noise((int)(x - 1), (int)(y + 1)) + Noise((int)(x + 1), (int)(y + 1))) / 16;
        float sides = (Noise((int)(x - 1), (int)y) + Noise((int)(x + 1), (int)y) + Noise((int)x, (int)(y - 1)) + Noise((int)x, (int)(y + 1))) / 8;
        float center = Noise((int)x, (int)y) / 4;
        return corners + sides + center;
    }

    public static float CosineInterpolate(float a, float b, float x)
    {
        double ft = x * 3.1415927;
        double f = (1 - Math.Cos(ft)) * 0.5;

        return (float)((a * (1 - f)) + (b * f));
    }

    public static float InterpolatedNoise(float x, float y)
    {
        int intX = (int)x;
        float fractX = x - intX;

        int intY = (int)y;
        float fractY = y - intY;

        float v1 = SmoothNoise(intX, intY);
        float v2 = SmoothNoise(intX + 1, intY);
        float v3 = SmoothNoise(intX, intY + 1);
        float v4 = SmoothNoise(intX + 1, intY + 1);

        float i1 = CosineInterpolate(v1, v2, fractX);
        float i2 = CosineInterpolate(v3, v4, fractX);

        return CosineInterpolate(i1, i2, fractY);
    }

    public static float PerlinNoise2D(float x, float y)
    {
        float total = 0;
        float p = persistence;
        int n = NumberOfOctaves;

        for (int i = 0; i < n; i++)
        {
            int frequency = (int)Math.Pow(2, i);
            float amplitude = (int)Math.Pow(p, i);
            total = total + InterpolatedNoise(x * frequency, y * frequency) * amplitude;
        }
        return total;
    }
    #endregion
    
    public void GenerateLevel(int seed)
    {
        Level newLevel = new Level(seed);
        LoadLevel(newLevel);
    }
}
