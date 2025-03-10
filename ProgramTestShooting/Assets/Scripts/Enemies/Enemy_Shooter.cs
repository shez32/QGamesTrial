using System.Collections;
using UnityEngine;

public class Enemy_Shooter : Enemy
{
    [Header("Shooting")] 
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform pivot;
    [SerializeField] private float shootingInterval;
    [SerializeField] private AudioClip shootAudioClip;

    [Header("Evasion")] 
    [SerializeField] private float dodge = 5f; // Maximum evasion movement
    [SerializeField] private float smoothing = 2f; // Smoothing factor for evasion movement
    [SerializeField] Vector2 startWait  = new Vector2(0.5f, 1f); // Initial wait time before starting evasion
    [SerializeField] Vector2 maneuverTime = new Vector2(1f, 2f); // Duration of evasion maneuvers
    [SerializeField] Vector2 maneuverWait = new Vector2(1f, 2f); // Time between maneuvers
    
    private float targetManeuver; // Target x-axis movement for evasion
    private float originalSpeed; // Stores the original movement speed
    
    private float xMin, xMax, yMin, yMax; // Movement boundaries
    
    private float shootingTimer;

    private Transform player; 
    protected override void Start()
    {
        base.Start();
        
        // Get stage boundaries
        xMin = StageLoop.Instance.leftCollider.bounds.min.x;
        xMax = StageLoop.Instance.rightCollider.bounds.max.x;
        yMin = StageLoop.Instance.topCollider.bounds.min.y;
        yMax = StageLoop.Instance.bottomCollider.bounds.max.y;
        
        shootingTimer = shootingInterval;
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody>().transform;

        originalSpeed = moveSpeed;
        
        // Apply slight random variation to movement speed
        moveSpeed = Random.Range(originalSpeed * 0.8f, originalSpeed * 1.2f);

        StartCoroutine(Evade());
    }

    protected override void Move()
    {
        // Set constant downward movement
        Vector3 newVelocity = new Vector3(rb.velocity.x, -moveSpeed, 0f);
        rb.velocity = newVelocity;
        
        // Adjust horizontal movement based on evasion maneuvers
        float newManeuver = Mathf.MoveTowards(rb.velocity.x, targetManeuver, smoothing * Time.deltaTime);
        rb.velocity = new Vector3(newManeuver, rb.velocity.y, 0f);
        
        /*
        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, xMin, xMax), Mathf.Clamp(rb.position.y, yMin, yMax), 0.0f
        );
        */
        
        // Handle shooting
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0)
        {
            Shoot();
            shootingTimer = shootingInterval;
        }
    }

    private IEnumerator Evade()
    {
        // Wait before starting evasive maneuvers
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));

        while (true)
        {
            // Choose a random dodge direction
            targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
            yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
            
            // Reset maneuver
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
        }
    }

    private void Shoot()
    {
        if (player == null) return;

        // Calculate the angle to the player
        Vector3 direction = player.position - pivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f; // Adjust to match the pivot's default orientation
        
        // Rotate the pivot to aim at the player
        pivot.rotation = Quaternion.Euler(0, 0, angle);
        
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        PlaySound(shootAudioClip);
    }
}
