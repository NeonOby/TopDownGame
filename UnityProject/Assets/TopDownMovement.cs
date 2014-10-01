using UnityEngine;
using System.Collections;

[System.Serializable]
public class TopDownMovement
{
    public bool SmoothInput = false;

    public float Speed = 8.0f;
    public float Damping = 10.0f;

    private Vector3 wantedPosition;

    private Transform transform;

    public Vector3 MovementDirection
    {
        get
        {
            return wantedPosition - transform.position;
        }
    }

    public void Init(Transform target)
    {
        transform = target;

        wantedPosition = transform.position;
    }

    public void Update(float speedMult = 0.0f)
    {
        float horizontalInput = 0;
        float verticalInput = 0;

        if (SmoothInput)
        {
            horizontalInput = Controls.GetAxis("Horizontal");
            verticalInput = Controls.GetAxis("Vertical");
        }
        else
        {
            if (Controls.GetButton("LEFT"))
                horizontalInput = -1;
            if (Controls.GetButton("RIGHT"))
                horizontalInput = 1;
            if (Controls.GetButton("DOWN"))
                verticalInput = -1;
            if (Controls.GetButton("UP"))
                verticalInput = 1;
        }

        horizontalInput *= speedMult;
        verticalInput *= speedMult;

        wantedPosition += (Vector3.right * horizontalInput + Vector3.forward * verticalInput).normalized * Speed * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * Damping);
    }

    
}