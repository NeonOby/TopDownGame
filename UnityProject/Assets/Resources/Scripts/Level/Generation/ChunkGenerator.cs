using System.Threading;
using UnityEngine;

public class ChunkGenerator
{
    private Thread GeneratingThread;

    //Only lock this for a short time
    private volatile System.Object fastLock = new System.Object();
    //You can lock this longer
    private volatile System.Object slowLock = new System.Object();

    //Used from two or more threads
    private volatile bool generating = false;
    private volatile Chunk generatedChunk;

    //Only used from one thread at once
    private volatile int chunkX = 0;
    private volatile int chunkZ = 0;

    public int ChunkX
    {
        get
        {
            lock (fastLock)
            {
                return chunkX;
            }
        }
        set
        {
            lock (fastLock)
            {
                chunkX = value;
            }
        }
    }
    public int ChunkZ
    {
        get
        {
            lock (fastLock)
            {
                return chunkZ;
            }
        }
        set
        {
            lock (fastLock)
            {
                chunkZ = value;
            }
        }
    }

    public bool Generating
    {
        get
        {
            lock (fastLock)
            {
                return generating;
            }
        }
        set
        {
            lock (fastLock)
            {
                generating = value;
            }
        }
    }
    public Chunk GeneratedChunk
    {
        get
        {
            lock (slowLock)
            {
                return generatedChunk;
            }
        }
        set
        {
            lock (slowLock)
            {
                generatedChunk = value;
            }
        }
    }

    public ChunkGenerator(int x, int z, int seed)
    {
        ChunkX = x;
        ChunkZ = z;

        Generating = true;

        GeneratingThread = new Thread(Generate);
        GeneratingThread.Start();
    }

    public void Generate()
    {
        lock (slowLock)
        {
            //Generating
            generatedChunk = new Chunk();
            generatedChunk.posX = ChunkX;
            generatedChunk.posZ = ChunkZ;

            int startX = ChunkX * LevelGenerator.ChunkSize;
            int startZ = ChunkZ * LevelGenerator.ChunkSize;

            Vector3 zeroPos = Vector3.zero;
            Vector3 currentPos = new Vector3(startX, 0, startZ);

            float distance = Vector3.Distance(zeroPos, currentPos);

            LevelEntity entity;
            for (int x = 0; x < LevelGenerator.ChunkSize; x++)
            {
                for (int z = 0; z < LevelGenerator.ChunkSize; z++)
                {
                    currentPos.x = startX + x;
                    currentPos.z = startZ + z;

                    currentPos.x += 0.5f;
                    currentPos.z += 0.5f;

                    float NoiseScale = 0.5f;
                    float NoiseX = (float)(LevelGenerator.SeedXPosition + currentPos.x) * NoiseScale;
                    float NoiseY = (float)(LevelGenerator.SeedZPosition + currentPos.z) * NoiseScale;
                    float Noise = LevelGenerator.PerlinNoise2D(NoiseX, NoiseY);

                    //currentPos.y = Noise * 2f;

                    generatedChunk.SetCell(x, z, new Cell());
                    generatedChunk.GetCell(x, z).X = currentPos.x;
                    generatedChunk.GetCell(x, z).Z = currentPos.z;
                    generatedChunk.GetCell(x, z).Y = currentPos.y;

                    if (currentPos.x == 0.5f && currentPos.z == 0.5f)
                    {
                        entity = new LevelEntity_PlayerBase();
                        entity.PoolName = "PlayerBase";
                        entity.Position.Value = currentPos;

                        generatedChunk.GetCell(x, z).LevelEntity = entity;

                        generatedChunk.AddEntity(entity);
                        continue;
                    }

                    distance = Vector3.Distance(zeroPos, currentPos);
                    if (distance < LevelGenerator.SafeDistance)
                        continue;

                    if (Noise > 0.5f)
                    {
                        float value = Mathf.Clamp01((Noise - 0.5f) * 2f);

                        entity = new LevelEntity_ResourceBlock();
                        entity.PoolName = "ResourceBlock";
                        entity.Position.Value = currentPos;
                        entity.Init(value);

                        generatedChunk.GetCell(x, z).LevelEntity = entity;

                        generatedChunk.AddEntity(entity);
                    }
                }
            }
        }

        Generating = false;
        LevelGenerator.GeneratorFinished(this);
    }
}