using UnityEngine;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour
{
    public static int ChunkSize = 8;
    public static int ChunkLoadDistance = 4;

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
        level.UpdateChunks();
    }

    private static float safeDistance = 2f;
    private static float chancePerDistance = 0.001f;
    private static float strengthPerDistance = 0.5f;

    private static float maxChance = 0.1f;

    public static Chunk GenerateChunk(int seed, int relX, int relZ, float randomizedMapPositionX, float randomizedMapPositionZ)
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

        System.Random random = new System.Random(seed);

        Entity entity;
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

                absChance = distance * chancePerDistance;

                float NoiseScale = 0.5f;

                float NoiseX = (float)(randomizedMapPositionX + currentPos.x) * NoiseScale;
                float NoiseY = (float)(randomizedMapPositionZ + currentPos.z) * NoiseScale;

                float Noise = PerlinNoise2D(NoiseX, NoiseY);

                //Versuche auf jedem meter dinge zu erstellen
                if (Noise > 0.5f)
                {
                    newChunk.GetCell(x, z).Walkable = false;
                    newChunk.GetCell(x, z).ContainsEntity = true;
                    newChunk.GetCell(x, z).PoolName = "Block";

                    entity = new Entity();
                    entity.PoolName = "Block";
                    entity.Position.Value = currentPos;
                    newChunk.AddEntity(entity);
                }
            }
        }

        return newChunk;
    }

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

    public static int NumberOfNeighbors(int x, int z, string PoolName)
    {
        int count = 0;

        Cell top = LevelGenerator.level.GetCell(x, (float)z + 1f, false);
        Cell bottom = LevelGenerator.level.GetCell(x, (float)z - 1f, false);
        Cell right = LevelGenerator.level.GetCell((float)x + 1f, z, false);
        Cell left = LevelGenerator.level.GetCell((float)x - 1f, z, false);

        Cell topRight = LevelGenerator.level.GetCell((float)x + 1f, (float)z + 1f, false);
        Cell bottomRight = LevelGenerator.level.GetCell((float)x + 1f, (float)z - 1f, false);
        Cell topLeft = LevelGenerator.level.GetCell((float)x - 1f, (float)z + 1f, false);
        Cell bottomLeft = LevelGenerator.level.GetCell((float)x - 1f, (float)z - 1f, false);

        if (top != null && top.PoolName.Equals(PoolName))
            count++;
        if (topRight != null && topRight.PoolName.Equals(PoolName))
            count++;

        if (bottom != null && bottom.PoolName.Equals(PoolName))
            count++;
        if (bottomRight != null && bottomRight.PoolName.Equals(PoolName))
            count++;

        if (right != null && right.PoolName.Equals(PoolName))
            count++;
        if (topLeft != null && topLeft.PoolName.Equals(PoolName))
            count++;

        if (left != null && left.PoolName.Equals(PoolName))
            count++;
        if (bottomLeft != null && bottomLeft.PoolName.Equals(PoolName))
            count++;

        return count;
    }

    public void GenerateLevel(int seed)
    {
        Level newLevel = new Level(seed);
        LoadLevel(newLevel);
    }
}
