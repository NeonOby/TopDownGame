using UnityEngine;
using System.Collections;

public class SingletonExample_GameManager : SimpleLibrary.Singleton<SingletonExample_GameManager> 
{
    //You can use MonoBehaviour methods and calls just as you are used to do
    void Start()
    {
        DeathCounter = 0;
    }

    public float GameDifficulty = 0f;

    //Property can only be changed from inside this manager
    public int DeathCounter
    {
        get;
        private set;
    }


    public void AddDeath()
    {
        DeathCounter++;
    }
}
