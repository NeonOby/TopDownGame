using UnityEngine;
using System.Collections;

public class TimedEffect : MonoBehaviour 
{

    public float LifeTime = 0.5f;
    private float LifeTimer = 0f;

    public bool Enabled = false;

	void Reset () 
    {
        LifeTimer = 0f;
        Enabled = true;
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
            SimpleLibrary.SimplePool.Despawn(gameObject);
        }
	}
}
