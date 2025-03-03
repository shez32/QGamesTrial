using UnityEngine;

/// <summary>
/// Enemy SpawnPoint
/// </summary>
///

[System.Serializable]
public class EnemySpawnEntry
{
	public GameObject enemyPrefab;

	[Range(0f, 100)] 
	public int chance; // The probability of this enemy spawning (should sum to 100 across all entries)
}
public class EnemySpawner : MonoBehaviour
{
	[Header("Prefab")] 
	public EnemySpawnEntry[] enemySpawnEntries; // Array of enemy types and their spawn chances

	[Header("Parameter")]
	public float minSpawnInterval = 2;
	public float maxSpawnInterval = 5;
	public float spawnRangeMin;
	public float spawnRangeMax;
	public int maxActiveEnemies = 10; // Maximum number of active enemies allowed at once
	
	private float spawnTimer;
	
	private void Update()
	{
		spawnTimer -= Time.deltaTime;

		// If the timer reaches zero and the enemy limit is not exceeded, spawn an enemy
		if (spawnTimer <= 0 && EnemyCount() < maxActiveEnemies)
		{
			SpawnEnemy();
			// Reset the timer to a random value within the spawn interval range
			spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
		}
	}

	private void SpawnEnemy()
	{
		int totalChance = 0;
		
		// Calculate the total spawn chance (should ideally sum to 100)
		foreach (var entry in enemySpawnEntries)
		{
			totalChance += entry.chance;
		}

		// Warn if the total spawn chance isn't exactly 100, as this may cause imbalance
		if (totalChance != 100)
		{
			Debug.LogWarning("Total spawn chance is not 100. It is " + totalChance, this);
		}
		
		// Generate a random number to determine which enemy type will spawn
		int randomValue = Random.Range(0, totalChance);
		int cumulative = 0;
		GameObject selectedEnemy = null;

		// Iterate through the enemy list, adding up their chances until the random value falls within a range
		foreach (var entry in enemySpawnEntries)
		{
			cumulative += entry.chance;
			if (randomValue < cumulative)
			{
				selectedEnemy = entry.enemyPrefab;
				break;
			}
		}

		// If a valid enemy is selected, instantiate it at a random x-position within the spawn range
		if (selectedEnemy != null)
		{
			float spawnX = Random.Range(spawnRangeMin, spawnRangeMax);
			Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, transform.position.z);
			Instantiate(selectedEnemy, spawnPosition, Quaternion.identity, transform);
		}
	}

	//------------------------------------------------------------------------------

	// Counts the number of active enemies in the scene.
	// This is an expensive operation and needs to changed in the future
	private int EnemyCount()
	{
		return FindObjectsOfType<Enemy>().Length;
	}
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(Mathf.Abs(spawnRangeMin) + Mathf.Abs(spawnRangeMax), 1, 1));
	}
}
