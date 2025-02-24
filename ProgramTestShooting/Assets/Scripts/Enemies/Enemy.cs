using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
	[Header("Parameter")]
	public float moveSpeed;
	public int scoreValue;
	public float health;

	protected Rigidbody rb;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	protected virtual void Start()
	{

	}

	protected virtual void Update()
	{
		Move();
	}
	
	protected abstract void Move();

	public virtual void TakeDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
		{
			Die();
		}
	}

	protected virtual void Die()
	{
		StageLoop.Instance.AddScore(scoreValue);
		
		GameObject.Destroy(gameObject);
	}
	
	//------------------------------------------------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		PlayerBullet playerBullet = other.transform.GetComponent<PlayerBullet>();
		if (playerBullet && playerBullet.isPlayerUsing)
		{
			DestroyByPlayer(playerBullet);
		}

		if (other.CompareTag("Player"))
		{
			Player player = other.transform.GetComponentInParent<Player>();
			if(player) player.TakeDamage();
			Die();
		}
		
		//Debug.Log("Hit by: " + other.gameObject.name);
	}
	
	void DestroyByPlayer(PlayerBullet playerBullet)
	{
		//delete bullet
		if (playerBullet)
		{
			playerBullet.DeleteObject();
		}

		Die();
	}
}
