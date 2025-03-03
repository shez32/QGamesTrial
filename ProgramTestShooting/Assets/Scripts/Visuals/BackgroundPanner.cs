using UnityEngine;

public class BackgroundPanner : MonoBehaviour
{
    [SerializeField] private float panSpeed;  // Initial speed at which the background moves
    [SerializeField] private float deceleration = 0.5f; // Rate at which movement slows down when game is over
    [SerializeField] private PanType panDirection; 
   
    private float tileSizeY; // Stores the height of the background tile
    private Vector3 startPosition; // Starting position to reset the background position

    private bool isActive = true;
    private float currentPanSpeed;
    private float offset; // Tracks the total movement offset

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
            // Increase the offset based on speed, causing continuous movement
            offset += Time.deltaTime * currentPanSpeed;
        }
        else if (currentPanSpeed > 0)
        {   // If the game is over...........and the background is panning.....
            // Gradually decrease speed until it reaches zero
            currentPanSpeed -= deceleration * Time.deltaTime;
            if(currentPanSpeed < 0) currentPanSpeed = 0; // Ensure it doesn't go negative
            
            // Continue applying movement with reduced speed
            offset += Time.deltaTime * currentPanSpeed;
        }
        
        Vector3 direction = GetDirectionVector();
        
        // Uses Mathf.Repeat to loop the offset within tileSizeY to create an endless scrolling effect
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
        // Returns a unit vector corresponding to the selected pan direction
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
