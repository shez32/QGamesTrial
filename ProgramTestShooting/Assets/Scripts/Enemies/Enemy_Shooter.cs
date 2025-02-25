using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_Shooter : Enemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform pivot;
    [SerializeField] private float shootingInterval;
    [SerializeField] private AudioClip shootAudioClip;

    [Header("Evasion")] 
    [SerializeField] private float dodge = 5f;
    [SerializeField] private float smoothing = 2f;
    [SerializeField] Vector2 startWait  = new Vector2(0.5f, 1f);
    [SerializeField] Vector2 maneuverTime = new Vector2(1f, 2f);
    [SerializeField] Vector2 maneuverWait = new Vector2(1f, 2f);
    
    private float targetManeuver;
    private float originalSpeed;
    
    private float xMin, xMax, yMin, yMax;
    
    private float shootingTimer;

    private Transform player; 
    protected override void Start()
    {
        base.Start();
        
        xMin = StageLoop.Instance.leftCollider.bounds.min.x;
        xMax = StageLoop.Instance.rightCollider.bounds.max.x;
        yMin = StageLoop.Instance.topCollider.bounds.min.y;
        yMax = StageLoop.Instance.bottomCollider.bounds.max.y;
        
        shootingTimer = shootingInterval;
        
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody>().transform;

        originalSpeed = moveSpeed;
        
        moveSpeed = Random.Range(originalSpeed * 0.8f, originalSpeed * 1.2f);

        StartCoroutine(Evade());
    }

    protected override void Move()
    {
        Vector3 newVelocity = new Vector3(rb.velocity.x, -moveSpeed, 0f);
        rb.velocity = newVelocity;
        
        float newManeuver = Mathf.MoveTowards(rb.velocity.x, targetManeuver, smoothing * Time.deltaTime);
        rb.velocity = new Vector3(newManeuver, rb.velocity.y, 0f);
        /*
        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, xMin, xMax), Mathf.Clamp(rb.position.y, yMin, yMax), 0.0f
        );
        */
        
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0)
        {
            Shoot();
            shootingTimer = shootingInterval;
        }
    }

    private IEnumerator Evade()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));

        while (true)
        {
            targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
            yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
        }
    }

    private void Shoot()
    {
        if (player == null) return;

        Vector3 direction = player.position - pivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;
        
        pivot.rotation = Quaternion.Euler(0, 0, angle);
        
        
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        PlaySound(shootAudioClip);
    }
}
