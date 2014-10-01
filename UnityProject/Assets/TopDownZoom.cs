using UnityEngine;
using System.Collections;

[System.Serializable]
public class TopDownZoom
{
    public float Speed = 500.0f;
    public Vector2 MinMaxHeight = new Vector2(5f, 40f);
    public bool Smooth = false;
    public float Damping = 5.0f;

    private float wantedHeight = 0f;

    private Transform transform;

    public void Init(Transform owner)
    {
        transform = owner;
    }

    public float Height
    {
        get
        {
            return wantedHeight;
        }
    }

    public float procentage
    {
        get
        {
            return ((wantedHeight - MinMaxHeight.x) / (MinMaxHeight.y - MinMaxHeight.x));
        }
    }

    public void SetWantedHeight(float height)
    {
        wantedHeight = height;
    }

    public void Update()
    {
        float change = -Input.GetAxis("Mouse ScrollWheel") * Speed * Time.deltaTime;
        wantedHeight = Mathf.Clamp(wantedHeight + change, MinMaxHeight.x, MinMaxHeight.y);
        
        if (Smooth)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, wantedHeight, transform.position.z), Time.deltaTime * Damping);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, wantedHeight, transform.position.z);
        }
    }
}
