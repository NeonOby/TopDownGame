using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    public static bool GamePaused = false;

	// Use this for initialization
	void Start () 
    {
        lastTime = Time.realtimeSinceStartup;
	}

    public float TogglePauseTime = 0.5f;

    float delta = 0f;
    float lastTime = 0f;
    float TogglePauseTimer = 0f;

    public void TogglePause()
    {
        TogglePauseTimer = 0f;
        lastTime = Time.realtimeSinceStartup;

        if (GamePaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void PauseGame()
    {        
        GamePaused = true;
    }
    private void ResumeGame()
    {        
        GamePaused = false;
    }

    void UpdatePause()
    {
        if (Controls.GetButtonDown("SPACE"))
        {
            TogglePause();
        }
        if (TogglePauseTimer < 1f && TogglePauseTime > 0)
        {
            float fromTimeScale = GamePaused ? 1f : 0f;
            float toTimeScale = GamePaused ? 0f : 1f;

            delta = Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;
            TogglePauseTimer += delta;
            TogglePauseTimer = Mathf.Min(TogglePauseTimer, TogglePauseTime);

            Time.timeScale = Mathf.Lerp(fromTimeScale, toTimeScale, TogglePauseTimer / TogglePauseTime);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        UpdatePause();
        
	}
}
