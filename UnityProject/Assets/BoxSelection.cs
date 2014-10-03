using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxSelection : MonoBehaviour {

    public static bool Selecting = false;

    public static Vector3 StartMousePos = Vector3.zero;
    public static Vector3 EndMousePos = Vector3.zero;
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Selecting = true;
            StartMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Selecting = false;
            EndMousePos = Input.mousePosition;
            TrySelection();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance = 0f;
            if(!plane.Raycast(ray, out distance))
                return;

            Vector3 targetPos = ray.origin + ray.direction * distance;
            for (int i = 0; i < selectedGridders.Count; i++)
            {
                selectedGridders[i].SetTargetPos(targetPos);
            }
        }
	}

    public List<SimpleAI> selectedGridders = new List<SimpleAI>();
    public Rect selectionRect;
    public void TrySelection()
    {
        CalculateStartAndEndPosition();
        selectionRect = new Rect(StartPos.x, Screen.height - StartPos.y, EndPos.x - StartPos.x, StartPos.y - EndPos.y);
        selectedGridders.Clear();
        SimpleAI[] allAIs = GameObject.FindObjectsOfType<SimpleAI>();
        for (int i = 0; i < allAIs.Length; i++)
        {
            if (TransformInSelectionBox(allAIs[i].transform, selectionRect))
            {
                selectedGridders.Add(allAIs[i]);
            }
        }
    }

    public bool TransformInSelectionBox(Transform target, Rect rect)
    {
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        targetScreenPos.y = Screen.height - targetScreenPos.y;
        return rect.Contains(targetScreenPos);
    }

    Vector2 StartPos = Vector2.zero;
    Vector2 EndPos = Vector2.zero;
    private void CalculateStartAndEndPosition()
    {
        StartPos = Vector2.zero;
        StartPos.x = StartMousePos.x < Input.mousePosition.x ? StartMousePos.x : Input.mousePosition.x;
        StartPos.y = (Screen.height - StartMousePos.y) < (Screen.height - Input.mousePosition.y) ? StartMousePos.y : Input.mousePosition.y;
        EndPos = Vector2.zero;
        EndPos.x = StartMousePos.x > Input.mousePosition.x ? StartMousePos.x : Input.mousePosition.x;
        EndPos.y = (Screen.height - StartMousePos.y) > (Screen.height - Input.mousePosition.y) ? StartMousePos.y : Input.mousePosition.y;

    }

    void OnGUI()
    {
        if (Selecting)
        {
            CalculateStartAndEndPosition();
            if (Mathf.Abs(StartPos.x - EndPos.x) > 10 && Mathf.Abs(StartPos.y - EndPos.y) > 10)
            {
                GUI.Box(new Rect(StartPos.x, Screen.height - StartPos.y, EndPos.x - StartPos.x, StartPos.y - EndPos.y), "");
            }
        }
        for (int i = 0; i < selectedGridders.Count; i++)
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(selectedGridders[i].transform.position);
            GUI.Box(new Rect(screenpos.x - 10, Screen.height - (screenpos.y + 10), 20, 20), "X");
        }
    }
}
