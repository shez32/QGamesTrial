using UnityEngine;

public class Enemy_StraightMover : Enemy
{
    [SerializeField] private MeshFilter[] meshFilters;
    [SerializeField] private float rotationSpeed = 45f;

    protected override void Start()
    {
        base.Start();
        
        //Get a random mesh from the mesh filters array and apply it
        GetComponentInChildren<MeshFilter>().mesh = meshFilters[Random.Range(0, meshFilters.Length)].sharedMesh;
    }
    protected override void Move()
    {
        //this enemy types moves straight down 
        rb.velocity = Vector3.down * moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        
        //rotate along the x-axis to give some life-like feeling
        transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
    }
}
