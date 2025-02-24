using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player Character
/// </summary>
public class Player : MonoBehaviour
{
	public enum FireMode
	{
		Single,
		FullAuto
	};

	[Header("Shooting")] 
	public FireMode currentFireMode = FireMode.Single;
	public PlayerBullet mPrefabPlayerBullet;
	[SerializeField] private GameObject bulletSpawnActor;
	[SerializeField] private float fireRate = 0.5f;
	[SerializeField] private bool tripleShotActive = false;
	
	[Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float rotationSmoothing = 5f;
    [SerializeField] private float tiltMultiplier = 10f;

    [Header("Health")] 
    [SerializeField] private int maxLives = 5;
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.white;
    [SerializeField] private GameObject deathParticles;
    
    [Header("Boundary")] 
    [SerializeField] private float offset = 0.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip singleShotClip;
    [SerializeField] private AudioClip autoShotClip;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip deathSound;
    
    //------------------------------------------------------------------------------

    private DefaultInputActions defaultInputActions;
	private InputAction movementInput;
	private InputAction shootInput;

	private float lastShotTime;
	private bool isShooting;
	
	private Rigidbody rigidBody;
	
	private Vector2 inputDirection;
	
	private float minY, maxY;
	private float minX, maxX;
	
	private Renderer playerRenderer;
	private bool isInvincible = false;
	private Color originalColor;
	
	private int currentLives;
	
	private void Awake()
	{
		defaultInputActions = new DefaultInputActions();
	}

	private void Start()
	{
		rigidBody = GetComponentInChildren<Rigidbody>();
		playerRenderer = GetComponentInChildren<Renderer>();
		originalColor = playerRenderer.material.GetColor("_EmissionColor");
		
		currentLives = maxLives;
	}

	private void OnEnable()
	{
		movementInput = defaultInputActions.Player.Movement;
		shootInput = defaultInputActions.Player.Shoot;
		shootInput.performed += OnShootPerformed;
		shootInput.canceled += OnShootCanceled;
		movementInput.Enable();
		shootInput.Enable();
	}

	private void OnDisable()
	{
		movementInput.Disable();
		shootInput.Disable();
	}
	
	private void FixedUpdate()
	{
		//Debug.Log("Movement Values " + movementInput.ReadValue<Vector2>());

		inputDirection = movementInput.ReadValue<Vector2>();

        // Calculate the target velocity based on input
        Vector3 targetVelocity = new Vector3(inputDirection.x, inputDirection.y, 0.0f) * maxSpeed;

        // Smoothly adjust current velocity toward target velocity
        Vector3 velocityDiff = targetVelocity - rigidBody.velocity;

        // Determine acceleration rate based on whether we're speeding up or slowing down
        float accelRate = (Mathf.Abs(targetVelocity.magnitude) > Mathf.Abs(rigidBody.velocity.magnitude))
            ? acceleration : deceleration;

        // Limit the velocity change to avoid abrupt movement
        velocityDiff.x = Mathf.Clamp(velocityDiff.x, -accelRate * Time.fixedDeltaTime, accelRate * Time.fixedDeltaTime);
        velocityDiff.y = Mathf.Clamp(velocityDiff.y, -accelRate * Time.fixedDeltaTime, accelRate * Time.fixedDeltaTime);

		//Apply forces in the direction of acceleration
        rigidBody.AddForce(velocityDiff, ForceMode.VelocityChange);
        
        //Clamps the position of the player within the upper and lower limits defined by the boundary in StageLoop Class
        Vector3 clampedPosition = rigidBody.position;
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        rigidBody.position = clampedPosition;

        // Calculate desired rotation based on horizontal input for a natural tilt
        float desiredTilt = -inputDirection.x * tiltMultiplier;
        Quaternion targetRotation = Quaternion.Euler(-90f, desiredTilt, 0f);

        // Spherical lerp towards the target rotation
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, rotationSmoothing * Time.fixedDeltaTime);
        
        //Wraps the player around to other side if they go off-screen
        if(rigidBody.position.x < minX) rigidBody.position = new Vector3(maxX, rigidBody.position.y, rigidBody.position.z);
        else if(rigidBody.position.x > maxX) rigidBody.position = new Vector3(minX, rigidBody.position.y, rigidBody.position.z);
    }

	private void Update()
	{
		if (currentFireMode == FireMode.FullAuto && isShooting)
		{
			if (Time.time - lastShotTime >= fireRate)
			{
				Shoot();
				lastShotTime = Time.time;
			}
		}
	}

	private void OnShootPerformed(InputAction.CallbackContext context)
	{
		isShooting = true;

		if (currentFireMode != FireMode.FullAuto)
		{
			if (Time.time - lastShotTime >= fireRate)
			{
				Shoot();
				lastShotTime = Time.time;
			}
		}
	}

	private void OnShootCanceled(InputAction.CallbackContext context)
	{
		isShooting = false;
	}

	private void Shoot()
	{
		Instantiate(mPrefabPlayerBullet, bulletSpawnActor.transform.position, bulletSpawnActor.transform.rotation);

		if (tripleShotActive)
		{
			Vector3 leftOffset = bulletSpawnActor.transform.position + new Vector3(.7f, -1.5f, 0.0f);
			Vector3 rightOffset = bulletSpawnActor.transform.position - new Vector3(.7f, 1.5f, 0.0f);
			
			Instantiate(mPrefabPlayerBullet, leftOffset, bulletSpawnActor.transform.rotation);
			Instantiate(mPrefabPlayerBullet, rightOffset, bulletSpawnActor.transform.rotation);
		}

		if (audioSource != null)
		{
			audioSource.PlayOneShot(tripleShotActive ? autoShotClip : singleShotClip);
		}
	}
	
	public void CalculateBounds(BoxCollider topCollider, BoxCollider bottomCollider, BoxCollider leftCollider,
		BoxCollider rightCollider)
	{
		minY = bottomCollider.bounds.max.y + offset;
		maxY = topCollider.bounds.min.y - offset;
		
		minX = leftCollider.bounds.min.x + offset;
		maxX = rightCollider.bounds.max.x - offset;
	}
	public void TakeDamage()
	{
		if (isInvincible) return;

		currentLives--;
		
		//UI Here

		if (currentLives <= 0)
		{
			PlayerDeath();
		}
		else
		{
			StartCoroutine(DamageFeedback());
		}
	}

	private void PlayerDeath()
	{
		GameObject tempAudio = new GameObject();
		AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();
		tempAudioSource.clip = deathSound;
		tempAudioSource.Play();

		if (deathParticles)
		{
			Instantiate(deathParticles, rigidBody.transform.position, Quaternion.identity);
		}
		
		StageLoop.Instance.OnPlayerDeath();
		
		Destroy(tempAudioSource, deathSound.length);
		Destroy(gameObject);
	}

	private IEnumerator DamageFeedback()
	{
		isInvincible = true;
		StageLoop.Instance.OnDamage();

		if (audioSource != null)
		{
			audioSource.PlayOneShot(damageSound);
		}
		
		float elapsedTime = 0;

		while (elapsedTime < invincibilityDuration)
		{
			playerRenderer.material.SetColor("_EmissionColor", damageColor * 100); 
			yield return new WaitForSeconds(flashDuration);
			playerRenderer.material.SetColor("_EmissionColor", originalColor * 2); 
			yield return new WaitForSeconds(flashDuration);
			elapsedTime += flashDuration * 2;
		}
		
		isInvincible = false;
	}

}
