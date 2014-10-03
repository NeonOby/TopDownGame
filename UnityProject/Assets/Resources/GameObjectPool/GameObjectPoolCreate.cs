using UnityEngine;
using System.Collections;

public class GameObjectPoolCreate : MonoBehaviour {

	public GameObject prefab;
	public string poolName;
	public int count;

	public bool useOwnerAsParent = false;

    void Start()
    {

    }

	// Use this for initialization
	void Awake () {
		if(useOwnerAsParent)
			GameObjectPool.Instance.CreatePool(poolName, prefab, gameObject, count);
		else
			GameObjectPool.Instance.CreatePool(poolName, prefab, null, count);
	}
}
