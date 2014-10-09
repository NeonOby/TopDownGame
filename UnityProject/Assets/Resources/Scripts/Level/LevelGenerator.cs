using UnityEngine;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour
{
    public static int ChunkSize = 4;
    public static int ChunkLoadDistance = 2;

    public static Level level = null;

    public string[] DespawnPoolsOnLoad;

    public float ChunkUpdateTime = 1f;
    private float ChunkUpdateTimer = 0f;

    void Start()
    {
        GenerateLevel(0);
    }

    void Update()
    {
        if (level == null)
            return;

        ChunkUpdateTimer += Time.deltaTime;
        if (ChunkUpdateTimer > ChunkUpdateTime)
        {
            ChunkUpdateTimer = 0f;
            //CheckForEmptyChunks();
            level.UpdateChunks();
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
                    level.AddChunk(x, z, GenerateChunk(level.Seed, x, z));
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
        //CheckForEmptyChunks();
        level.UpdateChunks();
    }

    private static float safeDistance = 16f;
    private static float chancePerDistance = 0.0001f;
    private static float strengthPerDistance = 0.5f;

    private static float maxChance = 0.1f;

    public static Chunk GenerateChunk(int seed, int relX, int relZ)
    {
        Chunk newChunk = new Chunk();
        newChunk.posX = relX;
        newChunk.posZ = relZ;
        int startX = relX * ChunkSize;
        int startZ = relZ * ChunkSize;

        Vector3 zeroPos = Vector3.zero;
        Vector3 currentPos = new Vector3(startX, 0, startZ);

        float distance = Vector3.Distance(zeroPos, currentPos);

        float relChance = distance * chancePerDistance;
        float absChance = 0f;

        Entity entity;
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int z = 0; z < ChunkSize; z++)
            {
                currentPos.x = relX < 0 ? startX + x : startX + x;
                currentPos.z = relZ < 0 ? startZ + z : startZ + z;

                currentPos.x += 0.5f;
                currentPos.z += 0.5f;

                newChunk.SetCell(x, z, new Cell());
                newChunk.GetCell(x, z).X = currentPos.x;
                newChunk.GetCell(x, z).Z = currentPos.z;

                distance = Vector3.Distance(zeroPos, currentPos);
                if (distance - safeDistance < safeDistance)
                    continue;

                absChance = distance * chancePerDistance;

                //Versuche auf jedem meter dinge zu erstellen
                if (level.random.NextDouble() < Mathf.Min(relChance * absChance, maxChance))
                {
                    newChunk.GetCell(x, z).Walkable = false;
                    entity = new Entity();
                    entity.PoolName = "Gridder";
                    entity.Position.Value = currentPos;
                    newChunk.AddEntity(entity);
                }
            }
        }

        return newChunk;
    }

    public void GenerateLevel(int seed)
    {
        Level newLevel = new Level(seed);
        LoadLevel(newLevel);
    }
}
