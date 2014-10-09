using UnityEngine;
using System.Collections;

public class Despawner : MonoBehaviour {

    private string poolName = "";

    void SetPoolName(string value)
    {
        poolName = value;
    }

    void Start()
    {
        GameObjectPool.DespawnAllPerPool += DespawnAllPerPool;
    }

    public void DespawnAllPerPool(string pool)
    {
        if (poolName == "" || pool != poolName)
        {
            return;
        }
        GameObjectPool.Instance.Despawn(poolName, gameObject);
    }
}
