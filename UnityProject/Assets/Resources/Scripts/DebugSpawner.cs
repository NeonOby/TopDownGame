using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnPerButton
{
    public string ButtonString = "";
    public string PoolName = "";
    public int PlayerID = 0;
    public bool SpawnAtMousePos = true;
}

public class DebugSpawner : MonoBehaviour 
{
    public SpawnPerButton[] spawner;

	// Use this for initialization
	void Start () 
    {
	    if(spawner.Length == 0)
        {
            enabled = false;
            return;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i < spawner.Length; i++)
        {
            if (Controls.GetButtonDown(spawner[i].ButtonString))
            {
                TrySpawnThing(spawner[i]);
            }
        }
	}

    public void TrySpawnThing(SpawnPerButton info)
    {
        Vector3 position = Vector3.zero;
        if (info.SpawnAtMousePos)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance = 0f;
            if (!plane.Raycast(ray, out distance))
                return;

            position = ray.origin + ray.direction * distance;
        }
        GameObject go = GameObjectPool.Instance.Spawn(info.PoolName, position, Quaternion.identity);

        Entity entity = go.GetComponent<Entity>();
        entity.Owner = EntityController.Get(info.PlayerID);
    }
}
