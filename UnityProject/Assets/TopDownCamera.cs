using UnityEngine;
using System.Collections;

[System.Serializable]
public class TargetFollower
{
    private Transform transform;
    public Transform target;

    public Vector3 Difference;

    public bool Smooth = false;
    public float Damping = 2.0f;

    public void Init(Transform owner)
    {
        transform = owner;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Update()
    {
        if (target == null)
            return;

        if (Smooth)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + Difference, Time.deltaTime * Damping);
        }
        else
        {
            transform.position = target.position + Difference;
        }
    }
}

public class TopDownCamera : MonoBehaviour
{
    public bool FollowTarget = true;

    public TopDownMovement movement;
    public TopDownZoom zoom;
    public TargetFollower targetFollower;

    public float ZoomHeightSpeedMult = 2.0f;

	void Start () 
    {
        movement.Init(transform);
        zoom.Init(transform);
        targetFollower.Init(transform);
	}

	void Update () 
    {

        zoom.Update();
        targetFollower.Difference.y = zoom.Height;

        if (FollowTarget)
        {
            targetFollower.Update();
        }
        else
        {
            movement.Update(zoom.procentage * ZoomHeightSpeedMult);
        }

	}

    
}
