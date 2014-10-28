using UnityEngine;
using System.Collections;


/*
 * Everything using RightClick
 * 
 */
public class RightClickAction : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}

    SearchingPath currentFindingPath = null;
    bool finished = false;
    PathNode path = null;

    int updatesPerFrame = 50;

    float startTime = 0;
    float LastNeededTime = 0;

    public Vector3 lastClickedPosition = Vector3.zero;

    public string RightClickEffectPool = "RightClickEffect";

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(1) && GridSystem.GridPositionUnderMouse(out lastClickedPosition, Camera.main))
        {
            DoRightClick();
        }

        /*
        if (currentFindingPath != null && !finished)
        {
            for (int i = 0; i < updatesPerFrame; i++)
            {
                path = currentFindingPath.NextStepPath(out finished);
                if (finished)
                {
                    //TODO Create and set job to selected workers

                    LastNeededTime = Time.realtimeSinceStartup - startTime;
                    if (currentFindingPath.CallBack != null)
                    {
                        currentFindingPath.CallBack(currentFindingPath.Owner, currentFindingPath.GeneratePath());
                    }

                    break;
                }
            }
        }

        if (path != null && path.PreviousSteps != null)
        {
            Vector3 lastPos = path.LastStep.Position;
            lastPos.y = 0;
            foreach (var item in path)
            {
                Vector3 currentPos = item.Position;
                currentPos.y = 0.5f;
                Debug.DrawLine(lastPos, currentPos, Color.green);
                lastPos = currentPos;
            }
        }
        */ 
	}

    void OnGUI()
    {
        GUILayout.Label(LastNeededTime.ToString());
    }

    public void DoRightClick()
    {
        if (MouseSelection.State == MouseSelection.States.NOTHING_SELECTED)
            return;

        Cell targetCell = LevelGenerator.Level.GetCell(lastClickedPosition.x, lastClickedPosition.z);

        foreach (var item in MouseSelection.GetSelected())
        {
            if (item is Worker)
            {
                ((Worker)item).SetTargetCell(targetCell);
            }
        }

        //Effekt
        GameObjectPool.Instance.Spawn(RightClickEffectPool, lastClickedPosition, Quaternion.identity);
    }

}
