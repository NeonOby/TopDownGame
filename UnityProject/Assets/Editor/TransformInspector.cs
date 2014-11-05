 // Alternative version, with redundant code removed
using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{
	private bool showParams = false;
	
	public override void OnInspectorGUI()
	{
		Transform t = (Transform)target;
		EditorGUI.indentLevel = 0;

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button ("reset all")){
			Undo.RecordObject(t, "Transform Reset All");
			t.localPosition = Vector3.zero;
			t.localEulerAngles = Vector3.zero;
			t.localScale = new Vector3(1,1,1);
		}
		if(GUILayout.Button ("position")){
            Undo.RecordObject(t, "Transform Reset Position");
			t.localPosition = Vector3.zero;
		}
		if(GUILayout.Button ("rotation")){
            Undo.RecordObject(t, "Transform Reset Rotation");
			t.localEulerAngles = Vector3.zero;
		}
		if(GUILayout.Button ("scale")){
            Undo.RecordObject(t, "Transform Reset Scale");
			t.localScale = new Vector3(1,1,1);
		}
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button ("round all")){
            Undo.RecordObject(t, "Transform Reset All");
			t.localPosition = new Vector3(Mathf.Round(t.localPosition.x), Mathf.Round(t.localPosition.y), Mathf.Round(t.localPosition.z));
			t.localEulerAngles = new Vector3(Mathf.Round(t.localRotation.eulerAngles.x), Mathf.Round(t.localRotation.eulerAngles.y), Mathf.Round(t.localRotation.eulerAngles.z));
			t.localScale = new Vector3(Mathf.Round(t.localScale.x), Mathf.Round(t.localScale.y), Mathf.Round(t.localScale.z));
		}
		if(GUILayout.Button ("position")){
            Undo.RecordObject(t, "Transform Reset Position");
			t.localPosition = new Vector3(Mathf.Round(t.localPosition.x), Mathf.Round(t.localPosition.y), Mathf.Round(t.localPosition.z));
		}
		if(GUILayout.Button ("rotation")){
            Undo.RecordObject(t, "Transform Reset Rotation");
			t.localEulerAngles = new Vector3(Mathf.Round(t.localRotation.eulerAngles.x), Mathf.Round(t.localRotation.eulerAngles.y), Mathf.Round(t.localRotation.eulerAngles.z));
		}
		if(GUILayout.Button ("scale")){
            Undo.RecordObject(t, "Transform Reset Scale");
			t.localScale = new Vector3(Mathf.Round(t.localScale.x), Mathf.Round(t.localScale.y), Mathf.Round(t.localScale.z));
		}
		EditorGUILayout.EndHorizontal();
			
        //Randomize
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("randomize all"))
        {
            Undo.RecordObject(t, "Transform Randomize All");
            t.localPosition = Random.insideUnitSphere;
            t.localEulerAngles = Random.insideUnitSphere * 360.0f;
            t.localScale = Random.insideUnitSphere;
        }
        if (GUILayout.Button("position"))
        {
            Undo.RecordObject(t, "Transform Randomize Position");
            t.localPosition = Random.insideUnitSphere;
        }
        if (GUILayout.Button("rotation"))
        {
            Undo.RecordObject(t, "Transform Randomize Rotation");
            t.localEulerAngles = Vector3.up * Random.Range(0, 360);
        }
        if (GUILayout.Button("scale"))
        {
            Undo.RecordObject(t, "Transform Randomize Scale");
            t.localScale = new Vector3(1, 1, 1) * Random.Range(1.4f, 3.0f);
        }
        EditorGUILayout.EndHorizontal();

		//Position
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button ("R", GUILayout.Width (20))){
            Undo.RecordObject(t, "Transform Reset Position");
			t.localPosition = Vector3.zero;
		}
		Vector3 position = EditorGUILayout.Vector3Field("LocalPosition", t.localPosition);
		EditorGUILayout.EndHorizontal();
		
		//Rotation
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button ("R", GUILayout.Width (20))){
            Undo.RecordObject(t, "Transform Reset Rotation");
			t.localEulerAngles = Vector3.zero;
		}
		Vector3 eulerAngles = EditorGUILayout.Vector3Field("LocalRotation", t.localEulerAngles);
		EditorGUILayout.EndHorizontal();
		
		//Scale
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button ("R", GUILayout.Width (20))){
            Undo.RecordObject(t, "Transform Reset Scale");
			t.localScale = new Vector3(1,1,1);
		}
		Vector3 scale = EditorGUILayout.Vector3Field("LocalScale", t.localScale);
		EditorGUILayout.EndHorizontal();
		
		//World Attributes
		showParams = EditorGUILayout.Foldout(showParams, "World Attributes:");
		if(showParams){
			Vector3FieldEx("Position", t.position);
			Vector3FieldEx("Rotation", t.eulerAngles);
			Vector3FieldEx("Scale", t.lossyScale);
		}
		
		if (GUI.changed)
		{
            Undo.RecordObject(t, "Transform Change");
 
			t.localPosition = FixIfNaN(position);
			t.localEulerAngles = FixIfNaN(eulerAngles);
			t.localScale = FixIfNaN(scale);
		}
	}
 
	private Vector3 FixIfNaN(Vector3 v)
	{
		if (float.IsNaN(v.x))
		{
			v.x = 0;
		}
		if (float.IsNaN(v.y))
		{
			v.y = 0;
		}
		if (float.IsNaN(v.z))
		{
			v.z = 0;
		}
		return v;
	}
	
	public static Vector3 Vector3FieldEx(string label, Vector3 value) {
	    EditorGUILayout.BeginHorizontal();
			GUILayout.Label(label);

	        EditorGUILayout.LabelField("X", GUILayout.Width (12));
	        value.x = EditorGUILayout.FloatField(value.x);
	
	        EditorGUILayout.LabelField("Y", GUILayout.Width (12));
	        value.y = EditorGUILayout.FloatField(value.y);
	
	        EditorGUILayout.LabelField("Z", GUILayout.Width (12));
	        value.z = EditorGUILayout.FloatField(value.z);
	    EditorGUILayout.EndHorizontal();
	    return value;
	}
 
}