using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Chunk
{
    public bool Loaded = false;
    
    [JsonProperty]
    public Cell[,] cells;

    [JsonProperty]
    public List<LevelEntity> spawnedEntities = new List<LevelEntity>();

    [JsonProperty]
    public int posX = 0, posZ = 0;

    public Chunk()
    {
        cells = new Cell[LevelGenerator.ChunkSize, LevelGenerator.ChunkSize];
    }

    public void AddEntity(LevelEntity entity)
    {
        spawnedEntities.Add(entity);
    }

    public void RemoveLevelEntityFromList(LevelEntity entity)
    {
        if (spawnedEntities.Contains(entity))
        {
            spawnedEntities.Remove(entity);
        }
    }

    public Cell GetCell(float x, float z)
    {
        return GetCell((int)x, (int)z);
    }

    public Cell GetCell(int x, int z)
    {
        if (x < 0 || x > cells.GetUpperBound(0) || z < 0 || z > cells.GetUpperBound(1))
            return null;
        
        return cells[x, z];
    }

    public void SetCell(int x, int z, Cell cell)
    {
        if (x < 0 || x > cells.GetUpperBound(0) || z < 0 || z > cells.GetUpperBound(1))
            return;
        cells[x, z] = cell;
    }

    #region DEBUG
    private GameObject chunkInfoInfo;

    private int currentState = -1;

    public bool finishedGenerating = false;

    public void DespawnChunkInfo(GameObject info)
    {
        if (info == null)
            return;
		PriorityWorker_SimplePool_Despawn.Create(info, null);
    }
    public void SpawnChunkInfo(SimpleLibrary.PoolInfo poolName)
    {
        GameObject go = SimpleLibrary.SimplePool.Spawn(poolName, new Vector3(posX * LevelGenerator.ChunkSize, 0, posZ * LevelGenerator.ChunkSize), Quaternion.identity);
        chunkInfoInfo = go;
    }

    private void ShowLoadedGrid()
    {
        if (currentState == -1)
        {
            SpawnChunkInfo(LevelGenerator.Instance.LoadedChunk);
        }
        else
        {
            if (currentState == 0)
            {
                DespawnChunkInfo(chunkInfoInfo);
				SpawnChunkInfo(LevelGenerator.Instance.LoadedChunk);
            }
        }
        currentState = 1;
    }
    private void ShowUnLoadedGrid()
    {
        if (currentState == -1)
        {
            SpawnChunkInfo(LevelGenerator.Instance.UnloadedChunk);
        }
        else
        {
            if (currentState == 1)
            {
                DespawnChunkInfo(chunkInfoInfo);
				SpawnChunkInfo(LevelGenerator.Instance.UnloadedChunk);
            }
        }
        currentState = 0;
    }
    #endregion

    public void Load()
    {
        if (!finishedGenerating)
        {
            ShowUnLoadedGrid();
            return;
        }

        ShowLoadedGrid();

        if (Loaded)
            return;

        LevelEntity entity;
        for (int i = 0; i < spawnedEntities.Count; i++)
        {
            entity = spawnedEntities[i];
            if (entity == null)
                continue;
            entity.Spawn();
        }

        Loaded = true;
    }

    public void Unload()
    {
        ShowUnLoadedGrid();

        if (!Loaded)
            return;

        LevelEntity entity;
        for (int i = 0; i < spawnedEntities.Count; i++)
        {
            entity = spawnedEntities[i];
            if (entity == null)
                continue;
            entity.Despawn();
        }

        Loaded = false;
    }

    public void UpdateCellNeighbours()
    {
        foreach (var cell in cells)
        {
            cell.ChunkChanged();
        }
    }
}

