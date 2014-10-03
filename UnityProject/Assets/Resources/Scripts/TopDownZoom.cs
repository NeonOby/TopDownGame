using UnityEngine;
using System.Collections;

[System.Serializable]
public class TopDownZoom
{
    public float Speed = 500.0f;
    public Vector2 MinMaxHeight = new Vector2(5f, 40f);
    public bool Smooth = false;
    public float Damping = 5.0f;

    public float wantedMovement = 0f;

    private Transform transform;

    public void Init(Transform owner)
    {
        transform = owner;
    }

    public float Height
    {
        get
        {
            return transform.position.y;
        }
    }

    public float Percentage
    {
        get
        {
            return ((transform.position.y - MinMaxHeight.x) / (MinMaxHeight.y - MinMaxHeight.x));
        }
    }

    public void Update(float speedMult = 1.0f)
    {
        float change = -Input.GetAxis("Mouse ScrollWheel") * Speed * Time.deltaTime * speedMult;
        float newY = Mathf.Clamp(transform.position.y + wantedMovement + change, MinMaxHeight.x, MinMaxHeight.y);

        wantedMovement = newY - transform.position.y;

        float currentMovement = wantedMovement * Time.deltaTime * Damping;

        wantedMovement -= currentMovement;
        transform.position += Vector3.up * currentMovement;
    }
}
