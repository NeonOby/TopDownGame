using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;



[JsonObject(MemberSerialization.OptIn)]
public class Chunk
{
    public bool Loaded = false;

    //TODO change save system to two dimensional array with different type of grid-information
    //This should make it possible to implement an easy A* into the levelgeneration system.

    [JsonProperty]
    public Cell[,] cells;

    [JsonProperty]
    public List<LevelEntity> entities = new List<LevelEntity>();

    [JsonProperty]
    public int posX = 0, posZ = 0;

    public Chunk()
    {
        cells = new Cell[LevelGenerator.ChunkSize, LevelGenerator.ChunkSize];
    }

    public void AddEntity(LevelEntity entity)
    {
        entities.Add(entity);
    }

    public void RemoveLevelEntityFromList(LevelEntity entity)
    {
        if (entities.Contains(entity))
        {
            entities.Remove(entity);
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
    private GameObjectInfo chunkInfoInfo = new GameObjectInfo("", null);

    private int currentState = -1;

    public bool finishedGenerating = false;

    private string loadedPool = "LoadedChunk";
    private string unloadedPool = "UnLoadedChunk";

    public void DespawnChunkInfo(GameObjectInfo info)
    {
        if (info.poolName == "")
            return;
        PriorityWorker_Entity_Despawn.Create(info.poolName, info.gameObject, null);
    }
    public void SpawnChunkInfo(string poolName)
    {
        GameObject go = GameObjectPool.Instance.Spawn(poolName, new Vector3(posX * LevelGenerator.ChunkSize, 0, posZ * LevelGenerator.ChunkSize), Quaternion.identity);
        chunkInfoInfo = new GameObjectInfo(poolName, go);
    }

    private void ShowLoadedGrid()
    {
        if (currentState == -1)
        {
            SpawnChunkInfo(loadedPool);
        }
        else
        {
            if (currentState == 0)
            {
                DespawnChunkInfo(chunkInfoInfo);
                SpawnChunkInfo(loadedPool);
            }
        }
        currentState = 1;
    }
    private void ShowUnLoadedGrid()
    {
        if (currentState == -1)
        {
            SpawnChunkInfo(unloadedPool);
        }
        else
        {
            if (currentState == 1)
            {
                DespawnChunkInfo(chunkInfoInfo);
                SpawnChunkInfo(unloadedPool);
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
        for (int i = 0; i < entities.Count; i++)
        {
            entity = entities[i];
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
        for (int i = 0; i < entities.Count; i++)
        {
            entity = entities[i];
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

