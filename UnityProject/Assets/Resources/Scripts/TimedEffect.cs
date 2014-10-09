using UnityEngine;
using System.Collections;

public class TimedEffect : MonoBehaviour 
{

    public float LifeTime = 0.5f;
    private float LifeTimer = 0f;
    private string poolName = "";

    public bool Enabled = false;

	void Reset () 
    {
        LifeTimer = 0f;
        Enabled = true;
	}

    public void SetPoolName(string value)
    {
        poolName = value;
    }

    public void Disable()
    {
        Enabled = false;
    }

	// Update is called once per frame
	void Update () 
    {
        if (!Enabled)
            return;

        LifeTimer += Time.deltaTime;
        if (LifeTimer >= LifeTime)
        {
            GameObjectPool.Instance.Despawn(poolName, gameObject);
        }
	}
}
