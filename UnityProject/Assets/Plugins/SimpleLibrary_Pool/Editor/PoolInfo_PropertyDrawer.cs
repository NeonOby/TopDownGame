#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleLibrary
{
	[CustomPropertyDrawer(typeof(PoolInfo))]
	public class PoolInfo_PropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.serializedObject.Update();

			if (SimplePoolManager.Instance)
			{
				PoolInfo info = (PoolInfo)fieldInfo.GetValue(property.serializedObject.targetObject);

				if (info != null)
				{
					 SimplePoolManager.Instance.NameIndexChanged -= info.NameIndexChanged;
					 SimplePoolManager.Instance.NameIndexChanged += info.NameIndexChanged;
				}

				SerializedProperty SelectedPoolIndex = property.FindPropertyRelative("SelectedPoolIndex");

				if (SelectedPoolIndex.intValue < 0 || SelectedPoolIndex.intValue >= SimplePoolManager.Instance.PoolNames.Length)
					SelectedPoolIndex.intValue = 0;

				SelectedPoolIndex.intValue = EditorGUI.Popup(position, label.text, SelectedPoolIndex.intValue, SimplePoolManager.Instance.PoolNames);

			}
			else
			{
				Color before = GUI.color;
				GUI.color = Color.red;
				EditorGUI.LabelField(position, "Error: No SimplePoolManager found");
				GUI.color = before;
			}
			
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
