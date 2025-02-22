using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPanner : MonoBehaviour
{
    public float panSpeed;
    public PanType panDirection;
   
    private float tileSizeY;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        tileSizeY = transform.localScale.y;
    }

    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * panSpeed, tileSizeY);

        switch(panDirection)
        {
            case PanType.Left:
                transform.position = startPosition + Vector3.left * newPosition;
                break;
            case PanType.Right:
                transform.position = startPosition + Vector3.right * newPosition;
                break;
            case PanType.Up:
                transform.position = startPosition + Vector3.up * newPosition;
                break;
            case PanType.Down:
                transform.position = startPosition + Vector3.down * newPosition;
                break;
        }
    }

}

public enum PanType
{
    Up,
    Down,
    Left,
    Right
}
