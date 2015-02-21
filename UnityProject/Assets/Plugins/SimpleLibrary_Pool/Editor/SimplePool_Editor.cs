#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleLibrary
{
	[CustomEditor(typeof(SimplePool))]
	public class SimplePool_Editor : Editor
	{

		#region TopMenu
		[MenuItem("GameObject/SimpleLibrary/SimplePoolManager/SimplePool")]
		public static void CreateSimplePool()
		{
			GameObject go = new GameObject();
			SimplePool pool = go.AddComponent<SimplePool>();
			go.name = "SimplePool";
			if (Selection.activeGameObject)
				go.transform.SetParent(Selection.activeGameObject.transform);
			pool.PoolName = go.name;

			Undo.RecordObject(SimplePoolManager.Instance, "Add Pool");
			SimplePoolManager.Add(pool);
			if (go.transform.parent == null)
				go.transform.SetParent(SimplePoolManager.Instance.transform);

			Selection.activeGameObject = pool.gameObject;
		}
		#endregion

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		const float TabButtonHeight = 30f;

		public override void OnInspectorGUI()
		{
			SimplePool pool = (SimplePool)target;
			if (pool == null)
				return;

			if (SimplePoolManager.Instance != null)
			{
				if (!SimplePoolManager.Contains(pool))
				{
					Undo.RecordObject(SimplePoolManager.Instance, "Add Pool");
					pool.PoolName = pool.gameObject.name;
					SimplePoolManager.Add(pool);
				}
			}

			Color tmpColor;

			serializedObject.Update();

			//Editor
			SerializedProperty TabState = serializedObject.FindProperty("TabState");

			//SimplePool
			SerializedProperty PoolName = serializedObject.FindProperty("PoolName");
			SerializedProperty EnablePooling = serializedObject.FindProperty("EnablePooling");
			SerializedProperty ThisPrefab = serializedObject.FindProperty("ThisPrefab");
			SerializedProperty InstantiateAmountOnStart = serializedObject.FindProperty("InstantiateAmountOnStart");
			SerializedProperty DoNotDestroyOnLoad = serializedObject.FindProperty("DoNotDestroyOnLoad");
			SerializedProperty UseThisAsParent = serializedObject.FindProperty("UseThisAsParent");
			SerializedProperty Parent = serializedObject.FindProperty("Parent");
			SerializedProperty ReparentOnDespawn = serializedObject.FindProperty("ReparentOnDespawn");

			SerializedProperty UseOnSpawnMessage = serializedObject.FindProperty("UseOnSpawnMessage");
			SerializedProperty UseOnDespawnMessage = serializedObject.FindProperty("UseOnDespawnMessage");

			SerializedProperty ActivateOnSpawn = serializedObject.FindProperty("ActivateOnSpawn");
			SerializedProperty DeactivateOnDespawn = serializedObject.FindProperty("DeactivateOnDespawn");

			SerializedProperty MoveOnDespawn = serializedObject.FindProperty("MoveOnDespawn");
			SerializedProperty UseTransformPosition = serializedObject.FindProperty("UseTransformPosition");
			SerializedProperty DeactivatePosition = serializedObject.FindProperty("DeactivatePosition");
			SerializedProperty TargetPositionTransform = serializedObject.FindProperty("TargetPositionTransform");

			SerializedProperty DestroyUnusedObjects = serializedObject.FindProperty("DestroyUnusedObjects");
			SerializedProperty Intelligence = serializedObject.FindProperty("Intelligence");
			SerializedProperty WantedFreeObjects = serializedObject.FindProperty("WantedFreeObjects");

			//Debug
			SerializedProperty RunningDestroyWorker = serializedObject.FindProperty("RunningDestroyWorker");
			SerializedProperty RunningInstantiateWorker = serializedObject.FindProperty("RunningInstantiateWorker");

			SerializedProperty ObjectCount = serializedObject.FindProperty("ObjectCount");
			SerializedProperty AvailableObjects = serializedObject.FindProperty("AvailableObjects");
			SerializedProperty SpawnedObjects = serializedObject.FindProperty("SpawnedObjects");

			SerializedProperty EnableProfiler = serializedObject.FindProperty("EnableProfiler");

			SerializedProperty UsedTimesCount = serializedObject.FindProperty("UsedTimesCount");
			SerializedProperty CurrentUsedTime = serializedObject.FindProperty("CurrentUsedTime");
			SerializedProperty MinUsedTime = serializedObject.FindProperty("MinUsedTime");
			SerializedProperty MaxUsedTime = serializedObject.FindProperty("MaxUsedTime");
			SerializedProperty AverageUsedTime = serializedObject.FindProperty("AverageUsedTime");



			//Tabs
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("General Settings", GUILayout.Height(TabButtonHeight)))
				TabState.intValue = 0;
			if (GUILayout.Button("Object Control", GUILayout.Height(TabButtonHeight)))
				TabState.intValue = 1;
			if (GUILayout.Button("Informations", GUILayout.Height(TabButtonHeight)))
				TabState.intValue = 2;
			EditorGUILayout.EndHorizontal();

			SimpleEditor.SplitBox();

			if (TabState.intValue == 0)
			{
				#region General Settings

				tmpColor = GUI.backgroundColor;
				GUI.backgroundColor = EnablePooling.boolValue ? Color.red : Color.green;
				if (GUILayout.Button(EnablePooling.boolValue ? "Disable Pooling" : "Enable Pooling"))
				{
					EnablePooling.boolValue = !EnablePooling.boolValue;
				}
				GUI.backgroundColor = tmpColor;

				if (!EnablePooling.boolValue)
				{
					EditorGUILayout.LabelField("SimplePool.Spawn works like Object.Instantiate");
					EditorGUILayout.LabelField("SimplePool.Despawn works like Object.Destroy");
					SimpleEditor.SplitBox();
				}

				EditorGUILayout.LabelField("Use \"/\" to create categories");
				string tmp = EditorGUILayout.TextField("PoolName:", PoolName.stringValue);
				if (tmp != PoolName.stringValue)
				{
					PoolName.stringValue = tmp;
					((SimplePool)target).gameObject.name = PoolName.stringValue;

					serializedObject.ApplyModifiedProperties();
					SimplePoolManager.Instance.UpdatePoolNames();
				}
				

				EditorGUILayout.PropertyField(ThisPrefab, new GUIContent("GameObject Prefab:"));

				InstantiateAmountOnStart.intValue = EditorGUILayout.IntField("Start Instances:", InstantiateAmountOnStart.intValue);

				DoNotDestroyOnLoad.boolValue = EditorGUILayout.Toggle("Keep this on LoadLevel:", DoNotDestroyOnLoad.boolValue);
				
				UseThisAsParent.boolValue = EditorGUILayout.Toggle("Use this as parent:", UseThisAsParent.boolValue);
				
				if (!UseThisAsParent.boolValue)
				{
					EditorGUILayout.PropertyField(Parent, new GUIContent("Custom Parent:"));
				}
				ReparentOnDespawn.boolValue = EditorGUILayout.Toggle("Reparent on Despawn:", ReparentOnDespawn.boolValue);

				#endregion
			}
			else if (TabState.intValue == 1)
			{
				#region Object Control

				EditorGUILayout.LabelField("Sends \"OnSpawn\" to any class on the spawned Object");
				UseOnSpawnMessage.boolValue = EditorGUILayout.Toggle("Use Spawn Message", UseOnSpawnMessage.boolValue);
				EditorGUILayout.LabelField("Sends \"OnDespawn\" to any class on the spawned Object");
				UseOnDespawnMessage.boolValue = EditorGUILayout.Toggle("Use Despawn Message", UseOnDespawnMessage.boolValue);

				EditorGUILayout.LabelField("Activates the Object when it gets spawned");
				ActivateOnSpawn.boolValue = EditorGUILayout.Toggle("Activate on spawn", ActivateOnSpawn.boolValue);

				EditorGUILayout.LabelField("Deactivates the Object when it gets despawned");
				DeactivateOnDespawn.boolValue = EditorGUILayout.Toggle("Deactivate on despawn", DeactivateOnDespawn.boolValue);

				EditorGUILayout.LabelField("Moves the Object to a given position on despawn");
				MoveOnDespawn.boolValue = EditorGUILayout.Toggle("Move on despawn", MoveOnDespawn.boolValue);

				if (MoveOnDespawn.boolValue)
				{
					UseTransformPosition.boolValue = EditorGUILayout.Toggle("Use Transform position", UseTransformPosition.boolValue);

					if (UseTransformPosition.boolValue)
						EditorGUILayout.PropertyField(TargetPositionTransform, new GUIContent("Position Transform:"));
					else
						EditorGUILayout.Vector3Field("World Position", DeactivatePosition.vector3Value);
				}

				EditorGUILayout.LabelField("Allows SimplePool to destroy unused Objects");
				DestroyUnusedObjects.boolValue = EditorGUILayout.Toggle("Destroy unused Objects", DestroyUnusedObjects.boolValue);
				EditorGUILayout.LabelField("Caches last actions to calculate if it needs to destroy or create Objects");
				Intelligence.boolValue = EditorGUILayout.Toggle("Use Intelligence", Intelligence.boolValue);

				EditorGUILayout.LabelField("SimplePool tries to always have this amount of available Objects");
				WantedFreeObjects.intValue = EditorGUILayout.IntField("Free Object Count:", WantedFreeObjects.intValue);


				#endregion
			}
			else if (TabState.intValue == 2)
			{
				#region Informations

				EditorGUILayout.IntField("Object Count:", ObjectCount.intValue);
				EditorGUILayout.IntField("Free Objects:", AvailableObjects.intValue);
				EditorGUILayout.IntField("Spawned Objects:", SpawnedObjects.intValue);

				EditorGUILayout.IntField("Instantiate Worker Count:", RunningDestroyWorker.intValue);
				EditorGUILayout.IntField("Destroy Worker Count:", RunningInstantiateWorker.intValue);

				SimpleEditor.SplitBox();
				EditorGUILayout.LabelField("Running Information:");
				UsedTimesCount.intValue = EditorGUILayout.IntField("Cached values amount:", UsedTimesCount.intValue);
				EnableProfiler.boolValue = EditorGUILayout.Toggle("Enable Profiler:", EnableProfiler.boolValue);

				if (EnableProfiler.boolValue)
				{
					EditorGUILayout.LabelField(string.Format("Current work: {0:0.000000}s", CurrentUsedTime.floatValue));
					EditorGUILayout.LabelField(string.Format("Min work: {0:0.000000}s", MinUsedTime.floatValue));
					EditorGUILayout.LabelField(string.Format("Max work: {0:0.000000}s", MaxUsedTime.floatValue));
					EditorGUILayout.LabelField(string.Format("Average work: {0:0.000000}s", AverageUsedTime.floatValue));
				}

				#endregion
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}

#endif