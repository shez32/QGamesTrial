using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[Header("Enemy Parameters")]
	public float moveSpeed;
	public int scoreValue;
	public float health;
	public float lifeTime = 20f;

	[Header("Effects")]
	public GameObject deathParticles;
	public GameObject hitParticles;
	
	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip deathAudio;
	public AudioClip hitAudio;

	[Header("Powerups")]
	public GameObject[] powerUps;
	[Range(0, 100)] public int powerUpSpawnChance = 20; // Spawn chance percentage for powerups 
	
	protected Rigidbody rb;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		
		// Ensure there's an AudioSource component on the enemy
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	protected virtual void Start()
	{
		//Empty function......Not best practice but I'm leaving it here
	}

	protected virtual void Update()
	{
		Move();
		
		// Countdown life timer
		lifeTime -= Time.deltaTime;
		if(lifeTime <= 0) Die();
	}
	
	// Defines enemy movement. Must be implemented by child classes
	protected abstract void Move();

	// Applies damage to the enemy. If health reaches zero, the enemy dies
	public virtual void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Instantiate(deathParticles, transform.position, Quaternion.identity, StageLoop.Instance.stageTransform);
			DestroyByPlayer();
		}
		else
		{
			Instantiate(hitParticles, transform.position, Quaternion.identity, StageLoop.Instance.stageTransform);
			PlaySound(hitAudio);
		}
	}

	// Handles the enemy's destruction when killed by the player
	protected virtual void DestroyByPlayer()
	{
		StageLoop.Instance.AddScore(scoreValue);
		Die();
	}
	
	// Handles the enemy's destruction process
	protected virtual void Die()
	{
		//we are creating a temporary audio source to play the given death sound whilst the enemy is being destroyed
		GameObject tempAudioSource = new GameObject("TempAudio");
		tempAudioSource.transform.SetParent(StageLoop.Instance.transform);
		AudioSource tempSource = tempAudioSource.AddComponent<AudioSource>();
		tempSource.clip = deathAudio;
		tempSource.Play();
		
		//destroy the temp audio source after the death sound is played
		Destroy(tempAudioSource, deathAudio.length);

		// Spawn power-up if conditions are met
		
		if (powerUps != null && powerUps.Length > 0)
		{
			if (Random.Range(0, 100) <= powerUpSpawnChance)
			{
				Instantiate(powerUps[Random.Range(0, powerUps.Length)], transform.position, Quaternion.identity,
					StageLoop.Instance.stageTransform);
			}
		}
		
		GameObject.Destroy(gameObject);
	}
	
	//------------------------------------------------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		// Detect bullet collision
		PlayerBullet playerBullet = other.transform.GetComponent<PlayerBullet>();
		if (playerBullet && playerBullet.isPlayerUsing)
		{
			TakeDamage(1);
			GameObject.Destroy(playerBullet.gameObject);
		}

		// Detect player collision
		if (other.CompareTag("Player"))
		{
			Player player = other.transform.GetComponentInParent<Player>();
			player?.TakeDamage(); //Null Safe call
			Instantiate(deathParticles, transform.position, Quaternion.identity, StageLoop.Instance.stageTransform);
			Die();
		}
		//Debug.Log("Hit by: " + other.gameObject.name);
	}

	protected virtual void PlaySound(AudioClip clip)
	{
		if (audioSource != null && clip != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}
	
}
