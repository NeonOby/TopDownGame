using UnityEngine;
using System.Collections;

public class TopDownCamera : MonoBehaviour
{

    public bool SmoothInput = false;

    public float ZoomSpeed = 500.0f;
    public Vector2 MinMaxHeight = new Vector2(5f, 40f);
    public bool ExtraZoomSmoothing = false;
    public float ZoomDamping = 5.0f;

    public float ScroolSpeed = 8.0f;
    public float ScrollDamping = 10.0f;
    public float CalculateHeightIntoSpeed = 2f;

    private float wantedHeight = 0;
    private Vector3 wantedPosition;

    //Used for angled camera Zoom towards points etc.
    //private Plane groundPlane;

	void Start () 
    {
        wantedPosition = transform.position;
        wantedHeight = wantedPosition.y;
	}

	void Update () 
    {
        UpdateWantedScrollHeight();
        UpdateCameraWantedPosition();

        UpdateScrollHeight();
        UpdateCameraPosition();
	}

    private void UpdateCameraPosition()
    {
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * ScrollDamping);
    }

    private void UpdateCameraWantedPosition()
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

        horizontalInput += horizontalInput * CalculateHeightIntoSpeed * ((wantedHeight - MinMaxHeight.x) / (MinMaxHeight.y - MinMaxHeight.x));
        verticalInput += verticalInput * CalculateHeightIntoSpeed * ((wantedHeight - MinMaxHeight.x) / (MinMaxHeight.y - MinMaxHeight.x));

        wantedPosition += (Vector3.right * horizontalInput + Vector3.forward * verticalInput) * ScroolSpeed * Time.deltaTime;
    }

    private void UpdateScrollHeight()
    {
        if (ExtraZoomSmoothing)
        {
            wantedPosition.y = Mathf.Lerp(wantedPosition.y, wantedHeight, Time.deltaTime * ZoomDamping);
        }
        else
        {
            wantedPosition.y = wantedHeight;
        }
    }

    private void UpdateWantedScrollHeight()
    {
        float change = -Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        wantedHeight = Mathf.Clamp(wantedHeight + change, MinMaxHeight.x, MinMaxHeight.y);
    }
}
