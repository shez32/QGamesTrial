using UnityEngine;
using UnityEngine.Serialization;

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
			Instantiate(deathParticles, transform.position, Quaternion.identity);
			DestroyByPlayer();
		}
		else
		{
			Instantiate(hitParticles, transform.position, Quaternion.identity);
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
		AudioSource tempSource = tempAudioSource.AddComponent<AudioSource>();
		tempSource.clip = deathAudio;
		tempSource.Play();
		
		Destroy(tempAudioSource, deathAudio.length);
		
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
