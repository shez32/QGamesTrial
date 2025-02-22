using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player Character
/// </summary>
public class Player : MonoBehaviour
{
	[Header("Prefab")]
	public PlayerBullet m_prefab_player_bullet;

	[Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float rotationSmoothing = 5f;
    [SerializeField] private float tiltMultiplier = 10f;

    //------------------------------------------------------------------------------

    private DefaultInputActions defaultInputActions;
	private InputAction movementInput;

	private Rigidbody rigidBody;
	private Vector2 inputDirection;

	private void Awake()
	{
		defaultInputActions = new DefaultInputActions();
	}

	private void Start()
	{
		rigidBody = GetComponentInChildren<Rigidbody>();
    }

	private void OnEnable()
	{
		movementInput = defaultInputActions.Player.Movement;
		movementInput.Enable();
	}

	private void OnDisable()
	{
		movementInput.Disable();
	}

	private void FixedUpdate()
	{
		//Debug.Log("Movement Values " + movementInput.ReadValue<Vector2>());

		inputDirection = movementInput.ReadValue<Vector2>();

        // Calculate the target velocity based on input
        Vector3 targetVelocity = new Vector3(inputDirection.x, inputDirection.y, 0.0f) * maxSpeed;

        // Smoothly adjust current velocity toward target velocity
        Vector3 velocityDiff = targetVelocity - rigidBody.velocity;

        // Determine acceleration rate based on whether we’re speeding up or slowing down
        float accelRate = (Mathf.Abs(targetVelocity.magnitude) > Mathf.Abs(rigidBody.velocity.magnitude))
            ? acceleration : deceleration;

        // Limit the velocity change to avoid abrupt movement
        velocityDiff.x = Mathf.Clamp(velocityDiff.x, -accelRate * Time.fixedDeltaTime, accelRate * Time.fixedDeltaTime);
        velocityDiff.y = Mathf.Clamp(velocityDiff.y, -accelRate * Time.fixedDeltaTime, accelRate * Time.fixedDeltaTime);

		//Apply forces in the direction of acceleration
        rigidBody.AddForce(velocityDiff, ForceMode.VelocityChange);

        // Calculate desired rotation based on horizontal input for a natural tilt
        float desiredTilt = -inputDirection.x * tiltMultiplier;
        Quaternion targetRotation = Quaternion.Euler(-90f, desiredTilt, 0f);

        // Spherical lerp towards the target rotation
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, rotationSmoothing * Time.fixedDeltaTime);
    }

	
	public void StartRunning()
	{
		
	}
	
}
