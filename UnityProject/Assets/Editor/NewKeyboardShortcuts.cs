using UnityEngine;
using UnityEditor;
using System.Collections;

public class NewKeyboardShortcuts : MonoBehaviour {

	[MenuItem ("GameObject/ToggleGameObject Active State #&a")]
	static void ToggleGameObjectActiveState () {
		Selection.activeGameObject.SetActive(!Selection.activeGameObject.activeSelf);
	}

	[MenuItem ("GameObject/New Children #&n")]
	static void CreateNewEmptyGameObjectChild () {
		GameObject go = new GameObject("Child");
		go.transform.parent = Selection.activeTransform;
		Selection.activeTransform = go.transform;
	}

	[MenuItem("GameObject/Wrap in GameObject #&w")]
	static void WrapInObject() {
		if(Selection.gameObjects.Length == 0)
			return;
		GameObject go = new GameObject("Wrapper:NameMe");
		go.transform.parent = Selection.activeTransform.parent;
		go.transform.position = Vector3.zero;
		foreach(GameObject g in Selection.gameObjects) {
			g.transform.parent = go.transform;
		}

        Selection.activeTransform = go.transform;
	}

}
