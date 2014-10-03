using UnityEngine;
using System.Collections;

[System.Serializable]
public class TopDownMovement
{
    public bool SmoothInput = false;

    public float Speed = 8.0f;
    public float Damping = 10.0f;

    private Vector3 wantedMovement;

    private Transform transform;

    public Vector3 MovementDirection
    {
        get
        {
            return wantedMovement - transform.position;
        }
    }

    public void Init(Transform target)
    {
        transform = target;

        wantedMovement = transform.position;
    }

    public void Update(float speedMult = 1.0f)
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

        wantedMovement += (Vector3.right * horizontalInput + Vector3.forward * verticalInput) * Speed * Time.deltaTime * speedMult;

        Vector3 currentMovement = wantedMovement * Time.deltaTime * Damping;
        wantedMovement -= currentMovement;
        transform.position += currentMovement;
    }

    
}