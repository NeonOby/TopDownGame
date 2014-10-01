using UnityEngine;
using System.Collections;

public class TopDownController : MonoBehaviour 
{

    public TopDownMovement movement;

    public bool RotateTowardsMouse = true;
    public float RotationDamping = 5.0f;

    private Vector3 WantedLookDirection;
    private float speed = 0f;

    private Plane targetPlane;

	// Use this for initialization
	void Start () 
    {
        targetPlane = new Plane(Vector3.up, Vector3.zero);

        movement.Init(transform);
        WantedLookDirection = transform.forward;
	}

	// Update is called once per frame
	void Update () 
    {
        UpdateRotation();

        speed = Vector3.Dot(transform.forward, WantedLookDirection);
        speed = Mathf.Max(speed, 0.0f);

        movement.Update(speed);
	}

    public void UpdateRotation()
    {
        if (RotateTowardsMouse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance = 0f;
            targetPlane.Raycast(ray, out distance);
            Vector3 position = Camera.main.transform.position + ray.direction * distance;

            position.y = transform.position.y;
            WantedLookDirection = position - transform.position;
        }
        else
        {
            float horizontalInput = Controls.GetAxis("Horizontal");
            float verticalInput = Controls.GetAxis("Vertical");

            if (Controls.GetButton("LEFT"))
                horizontalInput = -1;
            if (Controls.GetButton("RIGHT"))
                horizontalInput = 1;
            if (Controls.GetButton("DOWN"))
                verticalInput = -1;
            if (Controls.GetButton("UP"))
                verticalInput = 1;

            WantedLookDirection = (Vector3.right * horizontalInput + Vector3.forward * verticalInput).normalized;
        }

        if (WantedLookDirection.magnitude > 0.2f)
        {
            Quaternion wantedRotation = Quaternion.LookRotation(WantedLookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * RotationDamping);
        }

    }
}
