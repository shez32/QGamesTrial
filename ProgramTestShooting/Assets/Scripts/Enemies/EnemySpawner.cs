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
	public int chance;
}
public class EnemySpawner : MonoBehaviour
{
	[Header("Prefab")] public EnemySpawnEntry[] EnemySpawnEntries;

	[Header("Parameter")]
	public float minSpawnInterval = 2;
	public float maxSpawnInterval = 5;
	public float spawnRangeMin;
	public float spawnRangeMax;
	public int maxActiveEnemies = 10;
	
	private float spawnTimer;

	//------------------------------------------------------------------------------

	private void Update()
	{
		spawnTimer -= Time.deltaTime;

		if (spawnTimer <= 0 && EnemyCount() < maxActiveEnemies)
		{
			SpawnEnemy();
			spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
		}
	}

	private void SpawnEnemy()
	{
		int totalChance = 0;
		foreach (var entry in EnemySpawnEntries)
		{
			totalChance += entry.chance;
		}

		if (totalChance != 100)
		{
			Debug.LogWarning("Total spawn chance is not 100. It is " + totalChance, this);
		}
		
		int randomValue = Random.Range(0, totalChance);
		int cumulative = 0;
		GameObject selectedEnemy = null;

		foreach (var entry in EnemySpawnEntries)
		{
			cumulative += entry.chance;
			if (randomValue < cumulative)
			{
				selectedEnemy = entry.enemyPrefab;
				break;
			}
		}

		if (selectedEnemy != null)
		{
			float spawnX = Random.Range(spawnRangeMin, spawnRangeMax);
			Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, transform.position.z);
			Instantiate(selectedEnemy, spawnPosition, Quaternion.identity, transform);
		}
	}

	//------------------------------------------------------------------------------

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
