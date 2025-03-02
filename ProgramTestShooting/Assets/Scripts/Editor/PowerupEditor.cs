using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(PowerUps))]
public class PowerupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PowerUps powerUp = (PowerUps)target;

        powerUp.powerUpType = (PowerUpType)EditorGUILayout.EnumPopup("Power-Up Type", powerUp.powerUpType);
        powerUp.powerUpDuration = EditorGUILayout.FloatField("Duration", powerUp.powerUpDuration);
        powerUp.powerUpMoveSpeed = EditorGUILayout.FloatField("PowerUp Move-Speed", powerUp.powerUpMoveSpeed);
        powerUp.powerUpSound = EditorGUILayout.ObjectField("Power-Up Sound", powerUp.powerUpSound, typeof(AudioClip), false) as AudioClip;

        // Display fields based on the selected power-up type
        switch (powerUp.powerUpType)
        {
            case PowerUpType.SpeedBoost:
                powerUp.speedMultiplier = EditorGUILayout.FloatField("Speed Multiplier", powerUp.speedMultiplier);
                powerUp.accelerationMultiplier = EditorGUILayout.FloatField("Acceleration Multiplier", powerUp.accelerationMultiplier);
                powerUp.decelerationMultiplier = EditorGUILayout.FloatField("Deceleration Multiplier", powerUp.decelerationMultiplier);
                powerUp.speedBoostImage = EditorGUILayout.ObjectField("Speed Boost", powerUp.speedBoostImage, typeof(Image), true) as Image;
                break;
            case PowerUpType.FullAuto:
                powerUp.fireRateMultiplier = EditorGUILayout.FloatField("Fire Rate", powerUp.fireRateMultiplier);
                powerUp.fullAutoImage = EditorGUILayout.ObjectField("Full Auto Image", powerUp.fullAutoImage, typeof(Image), true) as Image;
                break;
            case PowerUpType.TripleShot:
                powerUp.tripleShotImage = EditorGUILayout.ObjectField("Triple Shot", powerUp.tripleShotImage, typeof(Image), true) as Image;
                break;
            case PowerUpType.Health:
                powerUp.heartsRecovered = EditorGUILayout.IntField("Heart Recovered", powerUp.heartsRecovered);
                break;
            case PowerUpType.Shield:
                powerUp.shieldImage = EditorGUILayout.ObjectField("Shield", powerUp.shieldImage, typeof(Image), true) as Image;
                break;
        }

        // Apply changes to the serialized object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(powerUp);
        }
    }
}
