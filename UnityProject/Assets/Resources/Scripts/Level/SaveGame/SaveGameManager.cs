using UnityEngine;
using Newtonsoft.Json;

using System.IO;

public class SaveGameManager : MonoBehaviour
{
    private LevelGenerator levelGen;

    public SaveGame currentSaveGame = null;

    void Start()
    {
        if (levelGen == null)
        {
            levelGen = GameObject.FindObjectOfType<LevelGenerator>();
        }
        if(levelGen == null)
        {
            Debug.LogWarning("LevelGenerator missing");
            enabled = false;
            return;
        }
    }

    public SaveGame Load(string SlotName)
    {
        SaveGame saveGame;
        string json = "";
        if (File.Exists(SlotName))
        {
            using (var reader = new StreamReader(SlotName))
            {
                json = reader.ReadToEnd();
            }
        }
        else
        {
            return null;
        }

        if (json == "")
        {
            Debug.Log("Nothing Saved");
            return null;
        }

        saveGame = JsonConvert.DeserializeObject<SaveGame>(json);
        return saveGame;
    }

    public void Save(string SlotName, SaveGame saveGame)
    {
        string json = JsonConvert.SerializeObject(saveGame);

        using (var writer = new StreamWriter(SlotName))
        {
            writer.Write(json);
        }
    }

    void OnGUI()
    {
        return;
        if (GUILayout.Button("Load Game"))
        {
            currentSaveGame = Load("this");
            if (currentSaveGame != null)
            {
                levelGen.LoadLevel(currentSaveGame.level);
                Camera.main.transform.position = currentSaveGame.CameraPosition.Value;
            }
        }
        if (GUILayout.Button("Save Game"))
        {
            currentSaveGame = new SaveGame();
            currentSaveGame.CameraPosition.Value = Camera.main.transform.position;
            currentSaveGame.level = LevelGenerator.Level;
            Save("this", currentSaveGame);
        }
        if (GUILayout.Button("Generate Level"))
        {
            levelGen.GenerateLevel(System.DateTime.Now.Second);
        }
        if (currentSaveGame != null)
        {
            GUILayout.Label("Loaded Game");
            GUILayout.Label(System.String.Format("{0}", currentSaveGame.CameraPosition.Value));
            float CameraPosX = Camera.main.transform.position.x;
            float CameraPosZ = Camera.main.transform.position.z;

            int centerX = (int)(CameraPosX / LevelGenerator.ChunkSize);
            int centerZ = (int)(CameraPosZ / LevelGenerator.ChunkSize);

            GUILayout.Label(System.String.Format("Pos {0}:{1}", centerX, centerZ));


        }
    }
}

