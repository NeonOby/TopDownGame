using UnityEngine;
using System.Collections;

public class SingletonExample_PlayerTester : MonoBehaviour 
{
    void OnGUI()
    {
        if (GUILayout.Button("Kill Player"))
        {
            SingletonExample_GameManager.Instance.AddDeath();
        }
    }
}
