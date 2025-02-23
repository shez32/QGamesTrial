using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPanner : MonoBehaviour
{
    public float panSpeed;
    public float deceleration = 0.5f;
    public PanType panDirection;
   
    private float tileSizeY;
    private Vector3 startPosition;

    private bool isActive = true;
    private float currentPanSpeed;
    private float offset;

    void Start()
    {
        startPosition = transform.position;
        tileSizeY = transform.localScale.y;
        currentPanSpeed = panSpeed;
    }

    void Update()
    {
        if (isActive)
        {
            offset += Time.deltaTime * currentPanSpeed;
        }
        else if (currentPanSpeed > 0)
        {
            currentPanSpeed -= deceleration * Time.deltaTime;
            if(currentPanSpeed < 0) currentPanSpeed = 0;
            
            offset += Time.deltaTime * currentPanSpeed;
        }
        
        Vector3 direction = GetDirectionVector();
        float newPosition = Mathf.Repeat(offset, tileSizeY);
        transform.position = startPosition + direction * newPosition;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        
        if (isActive)
        {
            currentPanSpeed = panSpeed; // Reset to original speed when reactivated
        }
    }

    private Vector3 GetDirectionVector()
    {
        switch (panDirection)
        {
            case PanType.Left:
                return Vector3.left;
            case PanType.Right:
                return Vector3.right;
            case PanType.Up:
                return Vector3.up;
            case PanType.Down:
                return Vector3.down;
            default:
                return Vector3.zero;
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
