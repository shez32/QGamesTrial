using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[Header("Parameter")]
	public float moveSpeed;
	public int scoreValue;
	public float health;
	public float lifeTime = 20f;

	public GameObject deathParticles;
	public GameObject hitParticles;
	
	public AudioSource audioSource;
	public AudioClip deathAudio;
	public AudioClip hitAudio;

	public GameObject[] powerUps;
	public int powerUpSpawnChance = 20;
	
	protected Rigidbody rb;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	protected virtual void Start()
	{

	}

	protected virtual void Update()
	{
		Move();
		
		lifeTime -= Time.deltaTime;
		if(lifeTime <= 0) Die();
	}
	
	protected abstract void Move();

	public virtual void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Instantiate(deathParticles, transform.position, Quaternion.identity, StageLoop.Instance.m_stage_transform);
			DestroyByPlayer();
		}
		else
		{
			Instantiate(hitParticles, transform.position, Quaternion.identity, StageLoop.Instance.m_stage_transform);
			PlaySound(hitAudio);
		}
	}

	protected virtual void DestroyByPlayer()
	{
		StageLoop.Instance.AddScore(scoreValue);
		Die();
	}
	
	protected virtual void Die()
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		tempAudioSource.transform.SetParent(StageLoop.Instance.transform);
		AudioSource tempSource = tempAudioSource.AddComponent<AudioSource>();
		tempSource.clip = deathAudio;
		tempSource.Play();
		
		Destroy(tempAudioSource, deathAudio.length);

		if (powerUps != null && powerUps.Length > 0)
		{
			if (Random.Range(0, 100) <= powerUpSpawnChance)
			{
				Instantiate(powerUps[Random.Range(0, powerUps.Length)], transform.position, Quaternion.identity,
					StageLoop.Instance.m_stage_transform);
			}
		}
		
		GameObject.Destroy(gameObject);
	}
	
	//------------------------------------------------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		PlayerBullet playerBullet = other.transform.GetComponent<PlayerBullet>();
		if (playerBullet && playerBullet.isPlayerUsing)
		{
			TakeDamage(1);
			GameObject.Destroy(playerBullet.gameObject);
		}

		if (other.CompareTag("Player"))
		{
			Player player = other.transform.GetComponentInParent<Player>();
			if(player) player.TakeDamage();
			Instantiate(deathParticles, transform.position, Quaternion.identity, StageLoop.Instance.m_stage_transform);
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
