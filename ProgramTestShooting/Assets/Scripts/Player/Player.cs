using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

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
	public PlayerBullet mPrefabPlayerBullet;
	[SerializeField] private FireMode currentFireMode = FireMode.Single;
	[SerializeField] private GameObject bulletSpawnActor;
	[SerializeField] private float fireRate = 0.5f;
	[SerializeField] private bool tripleShotActive = false;
	
	[Header("Bullet Time")] 
	[SerializeField] private float bulletTimeDuration = 5f; 
	[SerializeField] private float bulletTimeCooldown = 10f; 
	[SerializeField] private float bulletTimeScale = 0.5f; 
	[SerializeField] private Slider bulletTimeSlider;
	[SerializeField] private float saturationTransitionDuration = 0.5f;
	
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

    [Header("UI")] 
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject powerUpSliderPrefab;
    
    //------------------------------------------------------------------------------
    public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Deceleration { get => deceleration; set => deceleration = value; }
    public FireMode CurrentFireMode { get => currentFireMode; set => currentFireMode = value; }
    public bool TripleShotActive { get => tripleShotActive; set => tripleShotActive = value; }
    
    
    //------------------------------------------------------------------------------

    private DefaultInputActions defaultInputActions;
	private InputAction movementInput;
	private InputAction shootInput;
	private InputAction bulletTimeInput;

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

	private List<Image> heartFills = new List<Image>(); 
	
	private bool isBulletTimeActive = false;
	private float currentBulletTime;
	private Coroutine bulletTimeCoroutine;
	
	private PostProcessVolume playerPostProcessVolume;
	private ColorGrading colorGrading;
	
	
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

		InitializeHealthUI();
		
		currentBulletTime = bulletTimeDuration;
		
		bulletTimeSlider = Instantiate(powerUpSliderPrefab, StageLoop.Instance.powerUpPanel).GetComponentInChildren<Slider>(); 
		
		if (bulletTimeSlider != null)
		{
			bulletTimeSlider.maxValue = bulletTimeDuration;
			bulletTimeSlider.value = currentBulletTime;
		}

		playerPostProcessVolume = StageLoop.Instance.postProcessVolume;
		if (playerPostProcessVolume.profile.TryGetSettings(out colorGrading))
		{
			colorGrading.saturation.value = 0f;
		}
	}

	private void OnEnable()
	{
		movementInput = defaultInputActions.Player.Movement;
		movementInput.Enable();
		
		shootInput = defaultInputActions.Player.Shoot;
		shootInput.performed += OnShootPerformed;
		shootInput.canceled += OnShootCanceled;
		shootInput.Enable();
		
		bulletTimeInput = defaultInputActions.Player.BulletTime;
		bulletTimeInput.performed += OnBulletTimePerformed;
		bulletTimeInput.canceled += OnBulletTimeCanceled;
		bulletTimeInput.Enable();
	}

	private void OnDisable()
	{
		movementInput.Disable();
		shootInput.Disable();
		bulletTimeInput.Disable();
	}
	
	private void FixedUpdate()
	{
		//Debug.Log("Movement Values " + movementInput.ReadValue<Vector2>());

		inputDirection = movementInput.ReadValue<Vector2>();

        // Calculate the target velocity based on input
        Vector3 targetVelocity = new Vector3(inputDirection.x, inputDirection.y, 0.0f) * (maxSpeed / Time.timeScale);

        // Smoothly adjust current velocity toward target velocity
        Vector3 velocityDiff = targetVelocity - rigidBody.velocity;

        // Determine acceleration rate based on whether we're speeding up or slowing down
        float accelRate = (Mathf.Abs(targetVelocity.magnitude) > Mathf.Abs(rigidBody.velocity.magnitude))
            ? acceleration : deceleration;

        // Limit the velocity change to avoid abrupt movement
        velocityDiff.x = Mathf.Clamp(velocityDiff.x, -accelRate * Time.unscaledDeltaTime, accelRate * Time.unscaledDeltaTime);
        velocityDiff.y = Mathf.Clamp(velocityDiff.y, -accelRate * Time.unscaledDeltaTime, accelRate * Time.unscaledDeltaTime);

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
        rigidBody.rotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, rotationSmoothing * Time.unscaledDeltaTime);
        
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
		
		if (!isBulletTimeActive && currentBulletTime < bulletTimeDuration)
		{
			currentBulletTime += (bulletTimeDuration / bulletTimeCooldown) * Time.deltaTime;
			currentBulletTime = Mathf.Clamp(currentBulletTime, 0, bulletTimeDuration);
			UpdateBulletTimeUI();
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

	private void OnBulletTimePerformed(InputAction.CallbackContext context)
	{
		if (!isBulletTimeActive && currentBulletTime > 0)
		{
			bulletTimeCoroutine = StartCoroutine(ActivateBulletTime());
		}
	}
	
	private void OnBulletTimeCanceled(InputAction.CallbackContext context)
	{
		if (isBulletTimeActive)
		{
			DeactivateBulletTime();
		}
	}

	private void InitializeHealthUI()
	{
		for (int i = 0; i < maxLives; i++)
		{
			GameObject heart = Instantiate(heartPrefab, StageLoop.Instance.healthPanel);
			RectTransform heartRect = heart.GetComponent<RectTransform>();
			heartRect.anchoredPosition = new Vector2(i * heartRect.rect.width, 0);
			Image heartFill = heart.transform.Find("BG/FG").GetComponent<Image>();
			heartFills.Add(heartFill);
		}
	}

	private void UpdateHealthUI()
	{
		for (int i = 0; i < heartFills.Count; i++)
		{
			heartFills[i].enabled = i < currentLives;
		}
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

	private void OnTriggerEnter(Collider other)
	{
		PlayerBullet playerBullet = other.transform.GetComponent<PlayerBullet>();
		if (playerBullet && !playerBullet.isPlayerUsing)
		{
			TakeDamage();
		}
	}

	public void Heal(int heartsToHeal)
	{
		currentLives = Mathf.Min(currentLives + heartsToHeal, maxLives);
		UpdateHealthUI();
	}

	public void ToggleShield(bool active)
	{
		if(active) isInvincible = true;
		else isInvincible = false;
	}

	public void ModifyMovementSpeed(float newMaxSpeed, float newAcceleration, float newDeceleration)
	{
		maxSpeed = newMaxSpeed;
		acceleration = newAcceleration;
		deceleration = newDeceleration;
	}
	
	public void TakeDamage()
	{
		if (isInvincible) return;

		currentLives--;

		UpdateHealthUI();

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
		GameObject tempAudio = new GameObject("playerTempAudio");
		tempAudio.transform.SetParent(StageLoop.Instance.transform);
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
	
	private IEnumerator ActivateBulletTime()
	{
		isBulletTimeActive = true;
		Time.timeScale = bulletTimeScale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
		
		ModifyMovementSpeed(maxSpeed * (1/bulletTimeScale), acceleration * (1/bulletTimeScale), deceleration * (1/bulletTimeScale));

		StartCoroutine(TransitionSaturation(-100f));
		
		while (isBulletTimeActive && currentBulletTime > 0)
		{
			float deltaTime = Time.unscaledDeltaTime;
			currentBulletTime -= deltaTime;
			UpdateBulletTimeUI();

			if (currentBulletTime <= 0)
			{
				DeactivateBulletTime();
			}

			yield return null;
		}
		
	}
	private void DeactivateBulletTime()
	{
		if (bulletTimeCoroutine != null)
		{
			StopCoroutine(bulletTimeCoroutine);
			bulletTimeCoroutine = null;
		}

		isBulletTimeActive = false;
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;
		
		ModifyMovementSpeed(maxSpeed / (1/bulletTimeScale), acceleration / (1/bulletTimeScale), deceleration / (1/bulletTimeScale));

		StartCoroutine(TransitionSaturation(0));
	}

	private IEnumerator TransitionSaturation(float targetSaturation)
	{
		float startSaturation = colorGrading.saturation.value;
		float elapsedTime = 0f;

		while (elapsedTime < saturationTransitionDuration)
		{
			elapsedTime += Time.unscaledDeltaTime; // Use unscaled time to ignore time scale changes
			float blend = Mathf.Clamp01(elapsedTime / saturationTransitionDuration);
			colorGrading.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, blend);
			yield return null;
		}

		colorGrading.saturation.value = targetSaturation;
	}
	
	private void UpdateBulletTimeUI()
	{
		if (bulletTimeSlider != null)
		{
			bulletTimeSlider.value = currentBulletTime;
		}
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
