using UnityEngine;

public class Enemy_StraightMover : Enemy
{
    [SerializeField] private MeshFilter[] meshFilters;
    [SerializeField] private float rotationSpeed = 45f;

    protected override void Start()
    {
        base.Start();
        
        GetComponentInChildren<MeshFilter>().mesh = meshFilters[Random.Range(0, meshFilters.Length)].sharedMesh;
    }
    protected override void Move()
    {
        rb.velocity = Vector3.down * moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        
        transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
    }
}
