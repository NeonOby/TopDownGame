using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseSelection : MonoBehaviour
{

    #region Instance
    private static MouseSelection instance;
    public static MouseSelection Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MouseSelection>();
            return instance;
        }
    }
    #endregion

    #region State
    public enum States
    {
        NOTHING_SELECTED,
        SINGLE_SELECTED,
        MULTIPLE_SELECTED
    }

    private States currentState;
    public States CurrentState
    {
        get
        {
            return currentState;
        }
    }

    void Awake()
    {
        instance = this;
        currentState = States.NOTHING_SELECTED;
    }

    public static void SetSelection(Entity[] entities)
    {
        instance.selectedEntities.Clear();
        foreach (var entity in entities)
        {
            instance.selectedEntities.Add(entity);
        }
        instance.UpdateState();
    }

    private void UpdateState()
    {
        if (selectedEntities.Count == 0)
        {
            currentState = States.NOTHING_SELECTED;
        }
        else if (selectedEntities.Count == 1)
        {
            currentState = States.SINGLE_SELECTED;
        }
        else
        {
            currentState = States.MULTIPLE_SELECTED;
        }
    }
    #endregion

    #region Selecting
    public static bool Selecting = false;

    public static Vector3 StartMousePos = Vector3.zero;
    public static Vector3 EndMousePos = Vector3.zero;

    public List<Entity> selectedEntities = new List<Entity>();
    public Rect selectionRect;

    public void TrySelection()
    {
        CalculateStartAndEndPosition();
        selectionRect = new Rect(StartPos.x, Screen.height - StartPos.y, EndPos.x - StartPos.x, StartPos.y - EndPos.y);
        selectedEntities.Clear();
        Entity[] allAIs = GameObject.FindObjectsOfType<Entity>();
        for (int i = 0; i < allAIs.Length; i++)
        {
            if (TransformInSelectionBox(allAIs[i].transform, selectionRect))
            {
                selectedEntities.Add(allAIs[i]);
            }
        }
        UpdateState();
    }

    public bool TransformInSelectionBox(Transform target, Rect rect)
    {
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.position);
        targetScreenPos.y = Screen.height - targetScreenPos.y;

        return rect.Contains(targetScreenPos);
    }
    #endregion


    #region StaticMethods
    public static States State
    {
        get
        {
            return instance.CurrentState;
        }
    }
    public static Entity[] GetSelected()
    {
        return instance.selectedEntities.ToArray();
    }
    #endregion

    

    void Start()
    {
        SimpleAI.SimpleAIDied += OnSimpleAIDied;
    }

    void OnSimpleAIDied(SimpleAI sender)
    {
        if (selectedEntities.Contains(sender))
        {
            selectedEntities.Remove(sender);
        }
    }

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
        for (int i = 0; i < selectedEntities.Count; i++)
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(selectedEntities[i].transform.position);
            GUI.Box(new Rect(screenpos.x - 10, Screen.height - (screenpos.y + 10), 20, 20), "X");
        }
    }

    

    
}
