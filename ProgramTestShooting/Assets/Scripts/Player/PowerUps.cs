using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUpType
{
    SpeedBoost,
    TripleShot,
    FullAuto,
    Health,
    Shield,
}
public class PowerUps : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float powerUpDuration;
    public AudioClip powerUpSound;
    public float powerUpMoveSpeed = 5f;
    
    //Speed Boost
    public float speedMultiplier;
    public float accelerationMultiplier;
    public float decelerationMultiplier;
    public Image speedBoostImage;
    private Coroutine speedBoostCoroutine;
    
    //Triple Shot
    public Image tripleShotImage;
    private Coroutine tripleShotCoroutine;
    
    //Full Auto
    public float fireRateMultiplier;
    public Image fullAutoImage;
    private Coroutine fullAutoCoroutine;
    
    //Health
    public int heartsRecovered = 1;
    
    //Shield
    public Image shieldImage;
    private Coroutine shieldCoroutine;
    
    private Player player;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform.GetComponentInParent<Player>();
            if (player)
            {
                ActivatePowerUp();
                PlayPowerUpSound();
                gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }

    private void Update()
    {
        rb.velocity = Vector3.down * powerUpMoveSpeed;
    }

    private void ActivatePowerUp()
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                if (speedBoostCoroutine != null)
                {
                    StopCoroutine(HandleSpeedBoost());
                }
                StartCoroutine(HandleSpeedBoost());
                break;
            case PowerUpType.TripleShot:
                if (tripleShotCoroutine != null)
                {
                    StopCoroutine(HandleTripleShot());
                }
                StartCoroutine(HandleTripleShot());
                break;
            case PowerUpType.FullAuto:
                if (fullAutoCoroutine != null)
                {
                    StopCoroutine(HandleFullAuto());
                }
                StartCoroutine(HandleFullAuto()); 
                break;
            case PowerUpType.Health:
                player.Heal(heartsRecovered);
                break;
            case PowerUpType.Shield:
                if (shieldCoroutine != null)
                {
                    StopCoroutine(HandleShield());
                }
                StartCoroutine(HandleShield());
                break;
            default:
                break;
        }
    }

    private void PlayPowerUpSound()
    {
        if (powerUpSound != null)
        {
            AudioSource.PlayClipAtPoint(powerUpSound, transform.position);
        }
    }
    
    private IEnumerator HandleSpeedBoost()
    {
        // Save current movement variables
        float originalMaxSpeed = player.MaxSpeed;
        float originalAcceleration = player.Acceleration;
        float originalDeceleration = player.Deceleration;

        // Apply speed boost modifiers
        player.MaxSpeed = speedMultiplier;
        player.Acceleration = accelerationMultiplier;
        player.Deceleration = decelerationMultiplier;

        // Activate UI indicator
        if (speedBoostImage != null)
        {
            speedBoostImage.gameObject.SetActive(true);
        }

        // Wait for the duration of the power-up
        yield return new WaitForSeconds(powerUpDuration);

        // Restore original movement variables
        player.MaxSpeed = originalMaxSpeed;
        player.Acceleration = originalAcceleration;
        player.Deceleration = originalDeceleration;

        // Deactivate UI indicator
        speedBoostImage.gameObject.SetActive(false);

    }
    
    private IEnumerator HandleTripleShot()
    {
        // Activate triple shot
        player.TripleShotActive = true; 

        // Activate UI indicator
        if (tripleShotImage != null)
        {
            tripleShotImage.gameObject.SetActive(true);
        }

        // Wait for the duration of the power-up
        yield return new WaitForSeconds(powerUpDuration);

        // Deactivate triple shot
        player.TripleShotActive = false;

        // Deactivate UI indicator
        tripleShotImage.gameObject.SetActive(false);

    }
    
    private IEnumerator HandleFullAuto()
    {
        // Activate full auto mode
        player.CurrentFireMode = Player.FireMode.FullAuto; 

        // Activate UI indicator
        if (fullAutoImage != null)
        {
            fullAutoImage.gameObject.SetActive(true);
        }

        // Wait for the duration of the power-up
        yield return new WaitForSeconds(powerUpDuration);

        // Revert to default fire mode
        player.CurrentFireMode = Player.FireMode.Single; 

        // Deactivate UI indicator
        fullAutoImage.gameObject.SetActive(false);

    }

    private IEnumerator HandleShield()
    {
        player.ToggleShield(true);

        if (shieldImage != null)
        {
            shieldImage.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(powerUpDuration);
        
        player.ToggleShield(false);
        
        shieldImage.gameObject.SetActive(false);
    }

}
