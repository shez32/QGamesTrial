using UnityEngine;

public class Enemy_Shooter : Enemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform pivot;
    [SerializeField] private float shootingInterval;
    
    private float shootingTimer;

    private Transform player; 
    protected override void Start()
    {
        base.Start();
        shootingTimer = shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody>().transform;
    }

    protected override void Move()
    {
        rb.velocity = Vector3.down * moveSpeed;
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0)
        {
            Shoot();
            shootingTimer = shootingInterval;
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
    }
}
