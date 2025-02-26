using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Stage main loop
/// </summary>
public class StageLoop : MonoBehaviour
{
	#region static 
	static public StageLoop Instance { get; private set; }
	#endregion

	//
	public TitleLoop titleLoop;

	[Header("Layout")]
	public Transform m_stage_transform;
	public Text m_stage_score_text;
	public GameObject restartMenu;
	
	[SerializeField] private Camera defaultCamera;

	[Header("Prefab")]
	public Player m_prefab_player;
	public EnemySpawner m_prefab_enemy_spawner;
	
	[Header("Boundary")]
	public BoxCollider topCollider;
	public BoxCollider bottomCollider;
	public BoxCollider leftCollider;
	public BoxCollider rightCollider;
	public BackgroundPanner panner;
	
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip gameAudioClip;
	[SerializeField] private AudioClip gameOverAudioClip;
	//
	int m_game_score = 0;

	private EnemySpawner spawner;
	//------------------------------------------------------------------------------
	
	#region loop
	public void StartStageLoop()
	{
		StartCoroutine(StageCoroutine());
	}

	/// <summary>
	/// stage loop
	/// </summary>
	private IEnumerator StageCoroutine()
	{
		Debug.Log("Start StageCoroutine");

		SetupStage();

		while (true)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				//exit stage
				CleanupStage();
				titleLoop.StartTitleLoop();
				yield break;
			}
			yield return null;
		}
	}
	#endregion


	void SetupStage()
	{
		Instance = this;

		m_game_score = 0;
		RefreshScore();
		
		restartMenu.SetActive(false);

		//create player
		{
			Player player = Instantiate(m_prefab_player, m_stage_transform);
			if (player)
			{
				player.transform.position = new Vector3(0, -4, 0);
				player.CalculateBounds(topCollider, bottomCollider, leftCollider, rightCollider);
			}
		}

		//create enemy spawner
		{
			{
				if (!spawner) spawner = Instantiate(m_prefab_enemy_spawner, m_stage_transform);
					spawner.transform.position = new Vector3(0, 5.5f, 0);
			}
		}

		if (audioSource != null)
		{
			audioSource.PlayOneShot(gameAudioClip);
			audioSource.loop = true;
		}
	}

	void CleanupStage()
	{
		restartMenu.SetActive(false);
		
		//delete all object in Stage
		{
			for (var n = 0; n < m_stage_transform.childCount; ++n)
			{
				Transform temp = m_stage_transform.GetChild(n);
				GameObject.Destroy(temp.gameObject);
			}
		}

		Instance = null;
		
		audioSource.Stop();
	}

	//------------------------------------------------------------------------------

	public void AddScore(int a_value)
	{
		m_game_score += a_value;
		RefreshScore();
	}

	void RefreshScore()
	{
		if (m_stage_score_text)
		{
			m_stage_score_text.text = $"Score {m_game_score:00000}";
		}
	}

	public void OnPlayerDeath()
	{
		OnDamage();
		
		if (audioSource != null)
		{
			audioSource.Stop();
			audioSource.PlayOneShot(gameOverAudioClip);
			audioSource.loop = false;
		}

		GameObject.Destroy(spawner);
		panner.SetActive(false);
		
		restartMenu.SetActive(true);
	}

	public void OnDamage()
	{
		if (defaultCamera != null)
		{
			StartCoroutine(CameraShake(0.2f, 0.3f));
		}
	}

	private IEnumerator CameraShake(float duration, float magnitude)
	{
		Vector3 originalPosition = defaultCamera.transform.localPosition;
		float elapsed = 0.0f;

		while (elapsed < duration)
		{
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;

			defaultCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);

			elapsed += Time.deltaTime;
			yield return null;
		}

		defaultCamera.transform.localPosition = originalPosition;
	}

	public void RestartGame()
	{
		CleanupStage();
		SetupStage();
	}

	public void MainMenu()
	{
		CleanupStage();
		titleLoop.SetupTitle();
	}

}
