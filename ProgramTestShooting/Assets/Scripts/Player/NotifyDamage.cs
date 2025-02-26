using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (playerBullet && !playerBullet.isPlayerUsing)
        {
            player.TakeDamage();
            GameObject.Destroy(playerBullet.gameObject);
        }
    }
}
