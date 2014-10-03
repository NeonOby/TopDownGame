using UnityEngine;
using System.Collections;

public class TimedEffect : MonoBehaviour 
{

    public float LifeTime = 0.5f;
    private float LifeTimer = 0f;
    private string poolName = "";

	void Reset () 
    {
        LifeTimer = 0f;
	}

    public void SetPoolName(string value)
    {
        poolName = value;
    }

	// Update is called once per frame
	void Update () 
    {
        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTime)
        {
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
	}
}
