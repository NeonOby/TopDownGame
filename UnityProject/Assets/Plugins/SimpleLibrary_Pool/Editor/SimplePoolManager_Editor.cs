#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleLibrary
{
	[CustomEditor(typeof(SimplePoolManager))]
	public class SimplePoolManager_Editor : Editor
	{
		#region TopMenu
		[MenuItem("GameObject/SimpleLibrary/SimplePoolManager")]
		public static void CreateSimplePoolManager()
		{
			GameObject go = new GameObject();
			SimplePoolManager manager = go.AddComponent<SimplePoolManager>();
			SimplePoolManager.Instance = manager;
			go.name = "SimplePoolManager";
		}


		public static void CreateSimplePoolManager(SimplePool firstPool)
		{
			GameObject go = new GameObject();
			SimplePoolManager manager = go.AddComponent<SimplePoolManager>();
			SimplePoolManager.Instance = manager;
			SimplePoolManager.Add(firstPool);
			go.name = "SimplePoolManager";
		}
		#endregion

		const float TabButtonHeight = 30f;

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			SimplePoolManager manager = (SimplePoolManager)target;
			//SimplePoolManager.Instance = manager;


			SerializedProperty Pools_FoldOut = serializedObject.FindProperty("Pools_FoldOut");

			SerializedProperty SplitWorkload = serializedObject.FindProperty("SplitWorkload");
			SerializedProperty UpdateEveryXFrames = serializedObject.FindProperty("UpdateEveryXFrames");
			SerializedProperty MaxUsedDeltaTimePerFrame = serializedObject.FindProperty("MaxUsedDeltaTimePerFrame");


			SerializedProperty EnableProfiler = serializedObject.FindProperty("EnableProfiler");

			SerializedProperty UsedTimesCount = serializedObject.FindProperty("UsedTimesCount");
			SerializedProperty CurrentUsedTime = serializedObject.FindProperty("CurrentUsedTime");
			SerializedProperty MinUsedTime = serializedObject.FindProperty("MinUsedTime");
			SerializedProperty MaxUsedTime = serializedObject.FindProperty("MaxUsedTime");
			SerializedProperty AverageUsedTime = serializedObject.FindProperty("AverageUsedTime");

			if (GUILayout.Button("Add Pool", GUILayout.Height(TabButtonHeight)))
			{
				SimplePool pool;
				Undo.RegisterFullObjectHierarchyUndo(manager, "Find Pools");
				GameObject go = new GameObject();
				go.name = "NewPool";
				go.transform.SetParent(manager.transform);
				pool = go.AddComponent<SimplePool>();
				pool.PoolName = go.name;
				SimplePoolManager.Add(pool);
				//manager.FindAllPools();
				EditorUtility.SetDirty(manager);
			}

			EditorGUILayout.IntField("Pool Count:", manager.Pools.Count);

			if (manager.Pools.Count > 0)
			{
				Pools_FoldOut.boolValue = EditorGUILayout.Foldout(Pools_FoldOut.boolValue, "Pool-List");
				if (Pools_FoldOut.boolValue)
				{
					SimplePool pool = null;
					int moveFrom = 0;
					int moveTo = 0;
					int removePool = -1;
					EditorGUI.indentLevel++;
					for (int poolIndex = 0; poolIndex < manager.Pools.Count; poolIndex++)
					{
						pool = manager.Pools[poolIndex];
						if (pool == null)
						{
							removePool = poolIndex;
							break;
						}
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(pool, typeof(SimplePool), false);

						if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(30f)))
						{
							Selection.activeGameObject = pool.gameObject;
						}
						GUI.enabled = poolIndex != 0;
						if (GUILayout.Button("⤴", EditorStyles.miniButton, GUILayout.Width(20f)))
						{
							moveFrom = poolIndex;
							moveTo = poolIndex - 1;
						}
						GUI.enabled = poolIndex + 1 < manager.Pools.Count;
						if (GUILayout.Button("⤵", EditorStyles.miniButton, GUILayout.Width(20f)))
						{
							moveFrom = poolIndex;
							moveTo = poolIndex + 1;
						}
						GUI.enabled = true;
						if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20f)))
						{
							removePool = poolIndex;
						}
						EditorGUILayout.EndHorizontal();
					}

					if (removePool >= 0)
					{
						Undo.RegisterFullObjectHierarchyUndo(manager, "Remove Pool");
						GameObject go = null;
						if (manager.Pools[removePool]) go = manager.Pools[removePool].gameObject;
						//Undo.RegisterFullObjectHierarchyUndo(manager, "Remove Pool");
						//if (go) Undo.RecordObject(go, "Remove Pool");
						SimplePoolManager.RemoveAt(removePool);
						if (go) Undo.DestroyObjectImmediate(go);
						EditorUtility.SetDirty(manager);
					}
					if (moveFrom >= 0 && moveTo >= 0)
					{
						Undo.RecordObject(manager, "Move Pool");
						SimplePoolManager.MovePool(moveFrom, moveTo);
						EditorUtility.SetDirty(manager);
					}
					EditorGUI.indentLevel--;
				}
			}

			EditorGUILayout.LabelField("Uses a worker queue to split operations to different frames");
			SplitWorkload.boolValue = EditorGUILayout.Toggle("Split Workload:", SplitWorkload.boolValue);

			UpdateEveryXFrames.intValue = EditorGUILayout.IntField("Work every x frame:", UpdateEveryXFrames.intValue);

			MaxUsedDeltaTimePerFrame.floatValue = EditorGUILayout.FloatField("Max work (seconds):", MaxUsedDeltaTimePerFrame.floatValue);


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

			serializedObject.ApplyModifiedProperties();
		}
	}
}

#endif