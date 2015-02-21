using UnityEngine;
using System.Collections;

public class SingletonExample_GameManagerUI : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.Label(""); //Empty line
        GUILayout.Label(new GUIContent(string.Format("Deaths Counter: {0}", SingletonExample_GameManager.Instance.DeathCounter)));
    }
}
