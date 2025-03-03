using UnityEngine;

//This class is being used to notify the player script when it takes damage from enemy bullets
//The reason for creating a separate script is due to the fact that the rigidbody and collider are children of the empty gameobject where the player script resides
public class NotifyDamage : MonoBehaviour
{
    private Player player;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBullet playerBullet = other.transform.GetComponent<PlayerBullet>();
        
        //i.e it is the enemy bullet
        if (playerBullet && !playerBullet.isPlayerUsing)
        {
            player.TakeDamage();
            GameObject.Destroy(playerBullet.gameObject);
        }
    }
}
