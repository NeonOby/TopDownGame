using UnityEngine;
using System.Collections;

public class SimpleAI : MonoBehaviour 
{

    private NavMeshAgent agent;

    public Transform currentTarget;
    public Vector3 targetPos;

	// Use this for initialization
	void Start () 
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent ==null)
        {
            enabled = false;
            return;
        }
	}

    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public void SetTargetPos(Vector3 newTargetPos, bool removeTarget = false)
    {
        agent.SetDestination(newTargetPos);
        if (removeTarget) currentTarget = null;
    }

    private void UpdateTargetPos()
    {
        if (currentTarget)
        {
            SetTargetPos(currentTarget.transform.position);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        UpdateTargetPos();
	}
}
