using UnityEngine;

public class Enemy_StraightMover : Enemy
{
    protected override void Move()
    {
        rb.velocity = Vector3.down * moveSpeed;
    }
}
