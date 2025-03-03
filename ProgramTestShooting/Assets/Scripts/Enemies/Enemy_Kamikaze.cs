using UnityEngine;

public class Enemy_Kamikaze : Enemy
{
    private Transform playerTransform;
    protected override void Start()
    {
        base.Start();
        
        // On spawn, this enemy type will locate the player
        // if the player is found, it will home in on the location where the player was identified
        // note that this is not called on update as it would be unfair to the player
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody>().transform;
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    //Move method has to be implemented, however, in this case, we leave it empty
    protected override void Move()
    {
        
    }
}
