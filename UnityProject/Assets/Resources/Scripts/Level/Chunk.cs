using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectInfo
{
    public GameObjectInfo(string pool, GameObject go)
    {
        poolName = pool;
        gameObject = go;
    }

    public string poolName = "";
    public GameObject gameObject = null;
}

[JsonObject(MemberSerialization.OptIn)]
public class Chunk
{
    private bool Loaded = false;

    //TODO change save system to two dimensional array with different type of grid-information
    //This should make it possible to implement an easy A* into the levelgeneration system.

    [JsonProperty]
    public List<Entity> entities = new List<Entity>();

    private List<GameObjectInfo> spawnedObjects = new List<GameObjectInfo>();

    [JsonProperty]
    public int posX = 0, posZ = 0;

    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }

    private GameObjectInfo chunkInfoInfo = new GameObjectInfo("", null);

    private int currentState = -1;

    private string loadedPool = "LoadedChunk";
    private string unloadedPool = "UnLoadedChunk";

    public void DespawnChunkInfo(GameObjectInfo info)
    {
        if (info.poolName == "")
            return;
        GameObjectPool.Instance.Despawn(info.poolName, info.gameObject);
    }
    public void SpawnChunkInfo(string poolName)
    {
        GameObject go = GameObjectPool.Instance.Spawn(poolName, new Vector3(posX * LevelGenerator.ChunkSize, 0, posZ * LevelGenerator.ChunkSize), Quaternion.identity);
        chunkInfoInfo = new GameObjectInfo(poolName, go);
    }

    public void Load()
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

        if (Loaded)
            return;

        Entity entity;
        GameObject gameObject;
        for (int i = 0; i < entities.Count; i++)
        {
            entity = entities[i];
            if (entity == null)
                continue;

            gameObject = GameObjectPool.Instance.Spawn(entity.PoolName, entity.Position.Value, entity.Rotation.Value);

            spawnedObjects.Add(new GameObjectInfo(entity.PoolName, gameObject));
        }

        Loaded = true;
    }

    public void Unload()
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

        if (!Loaded)
            return;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i].poolName == "")
                continue;

            GameObjectPool.Instance.Despawn(spawnedObjects[i].poolName, spawnedObjects[i].gameObject);
        }
        spawnedObjects.Clear();

        Loaded = false;
    }
}

