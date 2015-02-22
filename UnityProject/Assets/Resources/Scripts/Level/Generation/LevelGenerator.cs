using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using SimpleLibrary;

public class LevelGenerator : Singleton<LevelGenerator>
{
	protected override void Awake()
	{
		base.Awake();
		ChunkSize = chunkSize;
		SafeDistance = safeDistance;
	}

	public static int ChunkSize;
	public static float SafeDistance;

    public int chunkSize = 16;
    public int chunkLoadDistance = 4;

    public float safeDistance = 2f;

	public PoolInfo LoadedChunk;
	public PoolInfo UnloadedChunk;
	public PoolInfo ResourceBlock;
	public PoolInfo ResourceCube;
	public PoolInfo PlayerBase;

    private Level level = null;
	public static Level Level
	{
		get
		{
			return Instance.level;
		}
	}

    public string[] DespawnPoolsOnLoad;

    public float ChunkUpdateTime = 1f;
    private float ChunkUpdateTimer = 0f;

    public List<ChunkGenerator> Generators = new List<ChunkGenerator>();

    void Start()
    {
		LoadChecker = new ChunkLoadChecker();
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
        if (!level.ContainsChunk(x, z) && !ContainsGenerator(x, z))
        {
			ChunkGenerator gen = new ChunkGenerator(x, z, chunkSize);
			gen.PlayerBase = PlayerBase;
			gen.ResourceBlock = ResourceBlock;
			gen.SeedXPos = level.randomizedMapPositionX;
			gen.SeedZPos = level.randomizedMapPositionZ;
			gen.ChunkSize = chunkSize;

			gen.StartGenerating();
			Generators.Add(gen);
        }
    }

	ChunkLoadChecker LoadChecker;

    void Update()
    {
        if (level == null)
            return;
		for (int genIndex = 0; genIndex < Generators.Count; genIndex++)
		{
			if (Generators[genIndex].IsAlive)
			{
				GeneratorFinished(Generators[genIndex]);
				break;
			}
		}

		if (LoadChecker == null)
			return;

        ChunkUpdateTimer += Time.deltaTime;
        if (ChunkUpdateTimer > ChunkUpdateTime)
        {
            ChunkUpdateTimer = 0f;

			//Check if last check is over
			if (!LoadChecker.IsAlive)
            {
				if (LoadChecker.neededGens.Count > 0)
				{
					foreach (var neededGen in LoadChecker.neededGens)
					{
						AddChunkGenerator(neededGen.x, neededGen.z);
					}
					LoadChecker.neededGens.Clear();
				}

                float CameraPosX = Camera.main.transform.position.x;
                float CameraPosZ = Camera.main.transform.position.z;

				float CurrentCenterX = (CameraPosX / chunkSize);
				float CurrentCenterZ = (CameraPosZ / chunkSize);

				LoadChecker.ChunkLoadDistance = chunkLoadDistance;
				LoadChecker.ChunkLoaded = ChunkLoaded;
				LoadChecker.ChunkUnloaded = ChunkUnloaded;
				LoadChecker.CurrentCenterX = (int)CurrentCenterX;
				LoadChecker.CurrentCenterZ = (int)CurrentCenterZ;
				LoadChecker.StartCheck();
            }
        }
    }

    private void GeneratorFinished(ChunkGenerator generator)
    {
        Chunk chunk = generator.GeneratedChunk;
        chunk.finishedGenerating = true;
        level.AddChunk(chunk.posX, chunk.posZ, chunk);
        Generators.Remove(generator);
    }

    public void LoadLevel(Level newLevel)
    {
        if (level != null)
        {
			level.UnloadEverything();
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

        float centerX = (CameraPosX / chunkSize);
        float centerZ = (CameraPosZ / chunkSize);

        int chunkX = x < 0 ? x + 1 : x;
        int chunkZ = z < 0 ? z + 1 : z;

        return Level.PosDistance(centerX, centerZ, chunkX, chunkZ);
    }

    public void ChunkLoaded(Chunk chunk)
    {
		lock (Level.LevelLock)
		{
			level.loadedChunks.Add(chunk);
		}
    }
    public void ChunkUnloaded(Chunk chunk)
    {
		lock (Level.LevelLock)
		{
			level.loadedChunks.Remove(chunk);
		}
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

public struct NeededChunkGen
{
	public int x;
	public int z;
}

public class ChunkLoadChecker
{

	public int CurrentCenterX;
	public int CurrentCenterZ;
	public int ChunkLoadDistance;

	public PriorityWorker_Chunk_Unload.AfterDespawnCallback ChunkUnloaded;
	public PriorityWorker_Chunk_Load.AfterDespawnCallback ChunkLoaded;


	public List<NeededChunkGen> neededGens = new List<NeededChunkGen>();


	Thread UpdateChunksThread = null;
	bool Checking = false;

	public bool IsAlive
	{
		get
		{
			if (Checking)
				return true;
			if (UpdateChunksThread == null)
				return false;
			return UpdateChunksThread.IsAlive;
		}
	}

	public void StartCheck()
	{
		//UpdateChunksThread = new Thread(CheckChunks);
		//UpdateChunksThread.Start();
		CheckChunks();
	}

	private Level level
	{
		get
		{
			return LevelGenerator.Level;
		}
	}

	public void CheckChunks()
	{
		Checking = true;
		for (int i = 0; i < level.loadedChunks.Count; i++)
		{
			if (Level.PosDistance(CurrentCenterX, CurrentCenterZ, level.loadedChunks[i].posX, level.loadedChunks[i].posZ) >= ChunkLoadDistance)
			{
				if (level.loadedChunks[i].Loaded)
				{
					PriorityWorker_Chunk_Unload.Create(level.loadedChunks[i], ChunkUnloaded);
				}
			}
		}

		for (int x = (Mathf.RoundToInt(CurrentCenterX) - ChunkLoadDistance); x < Mathf.RoundToInt(CurrentCenterX) + ChunkLoadDistance; x++)
		{
			for (int z = (Mathf.RoundToInt(CurrentCenterZ) - ChunkLoadDistance); z < Mathf.RoundToInt(CurrentCenterZ) + ChunkLoadDistance; z++)
			{
				if (Level.PosDistance(CurrentCenterX, CurrentCenterZ, x, z) >= ChunkLoadDistance)
					continue;

				neededGens.Add(new NeededChunkGen() { x = x, z = z });

				
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
		Checking = false;
	}

}
