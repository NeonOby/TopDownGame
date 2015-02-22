using SimpleLibrary;
using System.Threading;
using UnityEngine;

public class ChunkGenerator
{
	//Changed from inside, returns
	public Chunk GeneratedChunk { protected set; get; }
	public bool IsAlive
	{
		get
		{
			if (GeneratingThread == null)
				return false;
			return GeneratingThread.IsAlive;
		}
	}

	//Changed only inside, for checks
	public int ChunkX { protected set; get; }
	public int ChunkZ { protected set; get; }

	//Changed from outside
	public int ChunkSize { protected get; set; }
	public PoolInfo PlayerBase { protected get; set; }
	public PoolInfo ResourceBlock { protected get; set; }

	public float SeedXPos { protected get; set; }
	public float SeedZPos { protected get; set; }


	private Thread GeneratingThread;
	private int chunkX = 0;
	private int chunkZ = 0;
	private Vector3 CurrentPos;
	private int startX;
	private int startZ;

    public ChunkGenerator(int x, int z, int chunkSize)
    {
        chunkX = x;
		chunkZ = z;
		ChunkX = x;
		ChunkZ = z;
		ChunkSize = chunkSize;

		startX = chunkX * ChunkSize;
		startZ = chunkZ * ChunkSize;
		CurrentPos = new Vector3(startX, 0, startZ);
    }

	public void StartGenerating()
	{
		GeneratingThread = new Thread(Generate);
		GeneratingThread.Start();
	}

    public void Generate()
    {
		Loom.DispatchToMainThread(() =>
					Debug.Log("Start Generating"), true);
        //Generating
		GeneratedChunk = new Chunk();
		GeneratedChunk.posX = chunkX;
		GeneratedChunk.posZ = chunkZ;

        Vector3 zeroPos = Vector3.zero;

        float distance = Vector3.Distance(zeroPos, CurrentPos);

        LevelEntity entity;
		for (int x = 0; x < ChunkSize; x++)
        {
			for (int z = 0; z < ChunkSize; z++)
            {
                CurrentPos.x = startX + x;
                CurrentPos.z = startZ + z;

                CurrentPos.x += 0.5f;
                CurrentPos.z += 0.5f;

                float NoiseScale = 0.5f;
				float NoiseX = (float)(SeedXPos + CurrentPos.x) * NoiseScale;
				float NoiseY = (float)(SeedZPos + CurrentPos.z) * NoiseScale;
                float Noise = LevelGenerator.PerlinNoise2D(NoiseX, NoiseY);

				GeneratedChunk.SetCell(x, z, new Cell());
				GeneratedChunk.GetCell(x, z).X = CurrentPos.x;
				GeneratedChunk.GetCell(x, z).Z = CurrentPos.z;
				GeneratedChunk.GetCell(x, z).Y = CurrentPos.y;

                if (CurrentPos.x == 0.5f && CurrentPos.z == 0.5f)
                {
                    entity = new LevelEntity_PlayerBase();
                    entity.PoolName = "PlayerBase";
					entity.pool = PlayerBase;
                    entity.Position.Value = CurrentPos;

					GeneratedChunk.GetCell(x, z).LevelEntity = entity;

					GeneratedChunk.AddEntity(entity);
                    continue;
                }

                distance = Vector3.Distance(zeroPos, CurrentPos);
                if (distance < LevelGenerator.SafeDistance)
                    continue;

                if (Noise > 0.5f)
                {
                    float value = Mathf.Clamp01((Noise - 0.5f) * 2f);

                    entity = new LevelEntity_ResourceBlock();
                    entity.PoolName = "ResourceBlock";
					entity.pool = ResourceBlock;
                    entity.Position.Value = CurrentPos;
                    entity.Init(value);

					GeneratedChunk.GetCell(x, z).LevelEntity = entity;

					GeneratedChunk.AddEntity(entity);
                }
            }
        }

		Loom.DispatchToMainThread(() =>
					Debug.Log("End Generating"), true);

        //LevelGenerator.GeneratorFinished(this);
    }
}