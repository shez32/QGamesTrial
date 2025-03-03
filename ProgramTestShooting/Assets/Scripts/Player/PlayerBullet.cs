using UnityEngine;

/// <summary>
/// Player Bullet
/// </summary>
public class PlayerBullet : MonoBehaviour
{
	[Header("Parameter")]
	public float moveSpeed = 5;
	public float lifeTime = 2;

	public bool isPlayerUsing;
	void Update()
	{
		if (isPlayerUsing)
		{
			// we are dividing movespeed by time.timescale to counter-effect the bullet-time effect 
			// this ensures that the bullet speed is constant irrespective of the time scale
			transform.position += transform.up * (moveSpeed / Time.timeScale) * Time.deltaTime;
		}
		else
		{
			transform.position += -transform.up * moveSpeed * Time.deltaTime;
		}

		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0)
		{
			DeleteObject();
		}
	}

	public void DeleteObject()
	{
		GameObject.Destroy(gameObject);
	}
}
