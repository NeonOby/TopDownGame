using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLibrary
{
	[System.Serializable]
	public class PoolInfo
	{
		public int SelectedPoolIndex = 0;

		public void NameIndexChanged(int from, int to)
		{
			if (from == SelectedPoolIndex)
				SelectedPoolIndex = to;
		}
	}

	public class SimplePool : MonoBehaviour
	{

		

		public struct DestroyInfo
		{
			public SimplePool pool;
		}
		public struct InstantiateInfo
		{
			public SimplePool pool;
		}

		#region Static
		//If you forget how to use it :P
		public static GameObject Spawn(PoolInfo info)
		{
			return SimplePoolManager.Spawn(info);
		}
		public static GameObject Spawn(PoolInfo info, Vector3 position)
		{
			return SimplePoolManager.Spawn(info, position);
		}
		public static GameObject Spawn(PoolInfo info, Vector3 position, Quaternion rotation)
		{
			return SimplePoolManager.Spawn(info, position, rotation);
		}
		public static GameObject Spawn(GameObject prefab)
		{
			return SimplePoolManager.Spawn(prefab);
		}
		public static GameObject Spawn(GameObject prefab, Vector3 position)
		{
			return SimplePoolManager.Spawn(prefab, position);
		}
		public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			return SimplePoolManager.Spawn(prefab, position, rotation);
		}
		public static void Despawn(GameObject go)
		{
			SimplePoolManager.Despawn(go);
		}
		#endregion

		[SerializeField]
		public string PoolName = "Unknown";
		public bool EnablePooling = true;
		public GameObject ThisPrefab = null;
		public int InstantiateAmountOnStart = 0;

		public bool DoNotDestroyOnLoad = false;

		public bool UseThisAsParent = true;
		public Transform Parent = null;
		public bool ReparentOnDespawn = true;

		public bool UseOnSpawnMessage = true;
		public bool UseOnDespawnMessage = false;

		public bool ActivateOnSpawn = true;
		public bool DeactivateOnDespawn = true;

		public bool MoveOnDespawn = false;
		public bool UseTransformPosition = false;
		public Vector3 DeactivatePosition = Vector3.zero;
		public Transform TargetPositionTransform = null;


		public bool DestroyUnusedObjects = true;
		public bool Intelligence = true;

		public int WantedFreeObjects = 10;

		public Queue<GameObject> availableObjects = new Queue<GameObject>();
		public List<GameObject> spawnedObjects = new List<GameObject>();

		//Editor
		public int ObjectCount = 0;
		public int AvailableObjects = 0;
		public int SpawnedObjects = 0;
		public int TabState = 0;
		public int RunningDestroyWorker = 0;
		public int RunningInstantiateWorker = 0;

		protected int UpdateFrame = 0;

		public bool EnableProfiler = false;
		public int UsedTimesCount = 1000;
		protected Queue<float> LastUsedTimes = new Queue<float>();
		protected int ProfilingFrame = 0;

		public float StartTime = 0f;
		public float CurrentUsedTime = 0f;
		public float MinUsedTime = 0f;
		public float MaxUsedTime = 0f;
		public float AverageUsedTime = 0f;

		void Update()
		{
			UpdateFrame = UpdateFrame == 1 ? 0 : 1;
		}

		protected void ProfilingStart()
		{
			if (!EnableProfiler)
				return;
			StartTime = Time.realtimeSinceStartup;
		}
		protected void ProfilingEnd()
		{
			if (!EnableProfiler)
				return;
			if(ProfilingFrame != UpdateFrame)
			{
				ProfilingFrame = UpdateFrame;

				if (LastUsedTimes.Count == UsedTimesCount)
					LastUsedTimes.Dequeue();
				LastUsedTimes.Enqueue(CurrentUsedTime);

				MinUsedTime = LastUsedTimes.Min();
				MaxUsedTime = LastUsedTimes.Max();
				AverageUsedTime = LastUsedTimes.Average();

				CurrentUsedTime = 0;
			}
			CurrentUsedTime += TimeDiff(StartTime);
		}

		public virtual bool HasSpawned(GameObject go)
		{
			return spawnedObjects.Contains(go);
		}

		public virtual bool Uses(GameObject prefab)
		{
			return prefab == ThisPrefab;
		}

		public virtual GameObject Spawn()
		{
			ProfilingStart();
			if (availableObjects.Count == 0)
				InstantiateNewObject();
			if (availableObjects.Count == 0)
			{
				ProfilingEnd();
				return null;
			}
				
			GameObject go = availableObjects.Dequeue();
			AvailableObjects = availableObjects.Count;

			spawnedObjects.Add(go);
			SpawnedObjects = spawnedObjects.Count;

			RegisterAction(Action.Spawn);
			ProfilingEnd();
			return go;
		}
		public virtual GameObject Spawn(Vector3 position)
		{
			GameObject go = Spawn();
			if(go)
				go.transform.position = position;
			return go;
		}
		public virtual GameObject Spawn(Vector3 position, Quaternion rotation)
		{
			GameObject go = Spawn();
			if (go)
			{
				go.transform.position = position;
				go.transform.rotation = rotation;
			}
			return go;
		}
		public virtual void AfterSpawn(GameObject go)
		{
			if (!go)
				return;
			if (ActivateOnSpawn)
				go.SetActive(true);
			if (UseOnSpawnMessage)
				go.SendMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
		}

		

		protected virtual void DespawnObject(GameObject go, bool countAction = true)
		{
			if (UseOnDespawnMessage)
				go.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);

			if (DeactivateOnDespawn)
				go.SetActive(false);
			if (MoveOnDespawn)
			{
				if (UseTransformPosition && TargetPositionTransform != null)
					go.transform.position = TargetPositionTransform.position;
				else
					go.transform.position = DeactivatePosition;
			}
			if (ReparentOnDespawn)
				go.transform.parent = Parent;

			availableObjects.Enqueue(go);
			AvailableObjects = availableObjects.Count;

			if (countAction)
			{
				RegisterAction(Action.Despawn);
			}
		}

		public virtual bool TryDespawnObject(GameObject go, bool countAction = true)
		{
			if (!HasSpawned(go))
				return false;
			ProfilingStart();

			spawnedObjects.Remove(go);
			SpawnedObjects = spawnedObjects.Count;

			if (!EnablePooling)
			{
				DestroyObject(go);
				return true;
			}
			DespawnObject(go);
			ProfilingEnd();
			return true;
		}
		public virtual void InstantiateNewObject()
		{
			if (ThisPrefab == null)
				return;
			GameObject go = GameObject.Instantiate(ThisPrefab) as GameObject;
			go.transform.parent = Parent;
			go.SendMessage("SetPoolPrefab", ThisPrefab, SendMessageOptions.DontRequireReceiver);

			DespawnObject(go, false);
			ObjectCount++;

			//decrease worker count, but do not go under 0 (should never happen, but safety)
			RunningInstantiateWorker = Mathf.Max(RunningInstantiateWorker - 1, 0);
		}

		public virtual void DestroyOneObject()
		{
			//decrease worker count, but do not go under 0 (should never happen, but safety)
			RunningDestroyWorker = Mathf.Max(RunningDestroyWorker - 1, 0);

			//We have no free objects, we can not destroy anything
			if (availableObjects.Count == 0)
			{
				return;
			}
			DestroyObject(availableObjects.Dequeue());
			AvailableObjects = availableObjects.Count;
		}
		protected virtual void DestroyObject(GameObject go)
		{
			Object.Destroy(go);
			ObjectCount--;
		}

		
		protected virtual void Awake()
		{
			Init();
		}

		protected virtual void Init()
		{
			ObjectCount = 0;
			AvailableObjects = 0;
			SpawnedObjects = 0;
			RunningDestroyWorker = 0;
			RunningInstantiateWorker = 0;
			intValue = 0;

			availableObjects.Clear();
			spawnedObjects.Clear();
			lastActions.Clear();
			LastUsedTimes.Clear();

			if (DoNotDestroyOnLoad)
				DontDestroyOnLoad(gameObject);
			if (UseThisAsParent)
				Parent = transform;

			if (Parent == null)
				Parent = SimplePoolManager.Instance.transform;

			if (ThisPrefab != null)
			{
				//Prefill
				StartInstantiateWorker(InstantiateAmountOnStart);
			}
		}

		private float TimeDiff(float last)
		{
			return Time.realtimeSinceStartup - last;
		}

		protected void StartDestroyWorker()
		{
			//We can't destroy anything, when everything is spawned
			if (availableObjects.Count == 0)
				return;
			SimplePoolManager.Instance.AddDestroyInfo(new DestroyInfo() { pool = this });
			RunningDestroyWorker++;
		}
		protected void StartDestroyWorker(int amount)
		{
			for (int i = 0; i < amount; i++)
			{
				StartDestroyWorker();
			}
		}
		protected void StartInstantiateWorker()
		{
			SimplePoolManager.Instance.AddInstantiateInfo(new InstantiateInfo() { pool = this });
			RunningInstantiateWorker++;
		}
		protected void StartInstantiateWorker(int amount)
		{
			for (int i = 0; i < amount; i++)
			{
				StartInstantiateWorker();
			}
		}

		#region Intelligence

		public enum Action
		{
			Spawn = 1,
			Despawn = -1
		}

		protected float IntelligenceValue
		{
			get
			{
				return (intValue + ActionErrorCorrection) / (float)actionCount;
			}
		}
		
		public int ExpectedFreeObjectsCount
		{
			get
			{
				return availableObjects.Count + RunningInstantiateWorker - RunningDestroyWorker;
			}
		}

		public int ExpectedFreeObjectDifference
		{
			get
			{
				return (int)(IntelligenceValue * WantedIntelligenceObjectDifference);
			}
		}

		public int WantedIntelligenceObjectDifference = 10;

		public int ActionErrorCorrection = 2;

		public int intValue = 0;
		public int actionCount = 50;

		public Queue<int> lastActions = new Queue<int>();

		public void RegisterAction(Action action)
		{
			if (!EnablePooling)
				return;

			//Do not use Intelligence, ok, just try to stay between min and max free object count
			if (!Intelligence)
			{
				if (ExpectedFreeObjectsCount < WantedFreeObjects)
				{
					StartInstantiateWorker(WantedFreeObjects - ExpectedFreeObjectsCount);
				}
				else if (ExpectedFreeObjectsCount > WantedFreeObjects)
				{
					if (DestroyUnusedObjects) StartDestroyWorker(ExpectedFreeObjectsCount - WantedFreeObjects);
				}
				return;
			}


			if (lastActions.Count == actionCount)
				intValue -= lastActions.Dequeue();
			intValue += (int)action;
			lastActions.Enqueue((int)action);

			int objectDifference = 0;
			if (ExpectedFreeObjectsCount < WantedFreeObjects - ActionErrorCorrection)
				objectDifference -= WantedFreeObjects - ExpectedFreeObjectsCount;
			if (ExpectedFreeObjectsCount > WantedFreeObjects + ActionErrorCorrection)
				objectDifference += ExpectedFreeObjectsCount - WantedFreeObjects;

			objectDifference -= ExpectedFreeObjectDifference;

			if (objectDifference < 0)
			{
				StartInstantiateWorker(-objectDifference);
			}
			else if (objectDifference > 0)
			{
				if (DestroyUnusedObjects) StartDestroyWorker(objectDifference);
			}
		}
		#endregion
	}
}
