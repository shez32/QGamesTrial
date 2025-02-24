using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy_Kamikaze : Enemy
{
    private Transform playerTransform;
    protected override void Start()
    {
        base.Start();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody>().transform;
        
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    protected override void Move()
    {
        
    }
}
