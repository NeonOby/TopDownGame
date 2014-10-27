﻿using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    #region ThreadedMapGeneration
    private static volatile System.Object fastLock = new System.Object();

    private static int chunkSize = 16;
    private static int chunkLoadDistance = 4;

    private static float safeDistance = 2f;
    private static float chancePerDistance = 0.001f;
    private static float strengthPerDistance = 0.5f;

    private static float maxChance = 0.1f;

    public static float SafeDistance
    {
        get
        {
            lock (fastLock)
            {
                return safeDistance;
            }
        }
    }

    public static int ChunkSize
    {
        get
        {
            lock (fastLock)
            {
                return chunkSize;
            }
        }
        set
        {
            lock (fastLock)
            {
                chunkSize = value;
            }
        }
    }
    public static int ChunkLoadDistance
    {
        get
        {
            lock (fastLock)
            {
                return chunkLoadDistance;
            }
        }
        set
        {
            lock (fastLock)
            {
                chunkLoadDistance = value;
            }
        }
    }

    public static float SeedXPosition
    {
        get
        {
            lock (fastLock)
            {
                return level.randomizedMapPositionX;
            }
        }
    }

    public static float SeedZPosition
    {
        get
        {
            lock (fastLock)
            {
                return level.randomizedMapPositionZ;
            }
        }
    }
    #endregion

    private static LevelGenerator instance;
    public static LevelGenerator Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<LevelGenerator>();
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public static Level level = null;

    public string[] DespawnPoolsOnLoad;

    public float ChunkUpdateTime = 1f;
    private float ChunkUpdateTimer = 0f;

    public List<ChunkGenerator> generators = new List<ChunkGenerator>();

    void Start()
    {
        GenerateLevel(0);
    }

    public bool ContainsGenerator(int x, int z)
    {
        foreach (var item in generators)
        {
            if (item.ChunkX == x && item.ChunkZ == z)
                return true;
        }
        return false;
    }

    public void AddChunkGenerator(int x, int z)
    {
        if (!level.ContainsChunk(x, z) && !ContainsGenerator(x, z))
        {
            generators.Add(new ChunkGenerator(x, z, level.Seed));
        }
    }

    void Update()
    {
        if (level == null)
            return;

        ChunkUpdateTimer += Time.deltaTime;
        if (ChunkUpdateTimer > ChunkUpdateTime)
        {
            ChunkUpdateTimer = 0f;
            CheckChunks();
        }

        foreach (var generator in generators.ToArray())
        {
            if (!generator.Generating)
            {
                Chunk chunk = generator.GeneratedChunk;
                level.AddChunk(chunk.posX, chunk.posZ, chunk);
                generators.Remove(generator);
                chunk.finishedGenerating = true;
            }
        }
    }

    [Obsolete]
    private void CheckForEmptyChunks()
    {
        float CameraPosX = Camera.main.transform.position.x;
        float CameraPosZ = Camera.main.transform.position.z;

        float centerX = (CameraPosX / ChunkSize)+1;
        float centerZ = (CameraPosZ / ChunkSize)+1;

        for (int x = (Mathf.RoundToInt(centerX) - ChunkLoadDistance)-1; x <= Mathf.RoundToInt(centerX) + ChunkLoadDistance; x++)
        {
            for (int z = (Mathf.RoundToInt(centerZ) - ChunkLoadDistance)-1; z <= Mathf.RoundToInt(centerZ) + ChunkLoadDistance; z++)
            {
                if (Level.PosDistance(centerX, centerZ, x, z) >= ChunkLoadDistance)
                    continue;
                if (!level.ContainsChunk(x, z))
                {
                    level.AddChunk(x, z, GenerateChunk(level.Seed, x, z, 0, 0));
                }
            }
        }
    }

    public void LoadLevel(Level newLevel)
    {
        if (level != null)
        {
            level.UnloadEverything();
        }

        for (int i = 0; i < DespawnPoolsOnLoad.Length; i++)
        {
            GameObjectPool.TriggerDespawn(DespawnPoolsOnLoad[i]);
        }

        level = newLevel;
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
        float CameraPosX = Camera.main.transform.position.x;
        float CameraPosZ = Camera.main.transform.position.z;

        float centerX = (CameraPosX / ChunkSize);
        float centerZ = (CameraPosZ / ChunkSize);

        for (int i = 0; i < level.loadedChunks.Count; i++)
        {
            if (Level.PosDistance(centerX, centerZ, level.loadedChunks[i].posX, level.loadedChunks[i].posZ) >= LevelGenerator.ChunkLoadDistance)
            {
                if (level.loadedChunks[i].Loaded)
                {
                    PriorityWorker_Chunk_Unload.Create(level.loadedChunks[i], ChunkUnloaded); 
                }
            }
        }

        for (int x = (Mathf.RoundToInt(centerX) - LevelGenerator.ChunkLoadDistance); x < Mathf.RoundToInt(centerX) + LevelGenerator.ChunkLoadDistance; x++)
        {
            for (int z = (Mathf.RoundToInt(centerZ) - LevelGenerator.ChunkLoadDistance); z < Mathf.RoundToInt(centerZ) + LevelGenerator.ChunkLoadDistance; z++)
            {
                if (Level.PosDistance(centerX, centerZ, x, z) >= LevelGenerator.ChunkLoadDistance)
                    continue;

                AddChunkGenerator(x, z);
                if (level.ContainsChunk(x, z))
                {
                    Chunk currentChunk = level.chunks[level.GetKey(x, z)];
                    if (!currentChunk.Loaded)
                    {
                        PriorityWorker_Chunk_Load.Create(currentChunk, ChunkLoaded);   
                    }                 
                }
            }
        }
    }

    public void ChunkLoaded(Chunk chunk)
    {
        level.loadedChunks.Add(chunk);
    }
    public void ChunkUnloaded(Chunk chunk)
    {
        level.loadedChunks.Remove(chunk);
    }

    //new one in "ChunkGenerator" Thread Safe?
    [Obsolete]
    private static Chunk GenerateChunk(int seed, int relX, int relZ, float randomizedMapPositionX, float randomizedMapPositionZ)
    {
        Chunk newChunk = new Chunk();
        newChunk.posX = relX;
        newChunk.posZ = relZ;
        int startX = relX * ChunkSize;
        int startZ = relZ * ChunkSize;

        Vector3 zeroPos = Vector3.zero;
        Vector3 currentPos = new Vector3(startX, 0, startZ);

        float distance = Vector3.Distance(zeroPos, currentPos);

        LevelEntity entity;
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                currentPos.x = startX + x;
                currentPos.z = startZ + z;

                currentPos.x += 0.5f;
                currentPos.z += 0.5f;

                newChunk.SetCell(x, z, new Cell());
                newChunk.GetCell(x, z).X = currentPos.x;
                newChunk.GetCell(x, z).Z = currentPos.z;

                distance = Vector3.Distance(zeroPos, currentPos);
                if (distance - safeDistance < safeDistance)
                    continue;

                float NoiseScale = 0.5f;

                float NoiseX = (float)(randomizedMapPositionX + currentPos.x) * NoiseScale;
                float NoiseY = (float)(randomizedMapPositionZ + currentPos.z) * NoiseScale;

                float Noise = PerlinNoise2D(NoiseX, NoiseY);

                //Versuche auf jedem meter dinge zu erstellen
                if (Noise > 0.5f)
                {
                    entity = new ResourceBlockEntity();
                    entity.PoolName = "ResourceCube";
                    entity.Position.Value = currentPos;

                    newChunk.GetCell(x, z).Walkable = false;
                    newChunk.GetCell(x, z).Entity = entity;
                    newChunk.GetCell(x, z).ContainsEntity = true;
                    
                    newChunk.AddEntity(entity);
                }
            }
        }

        return newChunk;
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