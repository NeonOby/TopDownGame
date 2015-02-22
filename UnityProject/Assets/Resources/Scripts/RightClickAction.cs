using UnityEngine;
using System.Collections;
using SimpleLibrary;


/*
 * Everything using RightClick
 * 
 */
public class RightClickAction : MonoBehaviour 
{

	public Vector3 lastClickedPosition = Vector3.zero;

	public PoolInfo RightClickEffekt;

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(1) && GridSystem.GridPositionUnderMouse(out lastClickedPosition, Camera.main))
		{
			DoRightClick();
		}
	}

	public void DoRightClick()
	{
		if (MouseSelection.State == MouseSelection.States.NOTHING_SELECTED)
			return;

		bool shift = Controls.GetButton("Shift");

		Cell targetCell = LevelGenerator.Level.GetCell(lastClickedPosition.x, lastClickedPosition.z);

		foreach (var item in MouseSelection.GetSelected())
		{
			if (item is Worker)
			{
				((Worker)item).SetTargetCell(targetCell, shift);
			}
		}

		//Effekt
		SimplePool.Spawn(RightClickEffekt, lastClickedPosition, Quaternion.identity);
	}

}
