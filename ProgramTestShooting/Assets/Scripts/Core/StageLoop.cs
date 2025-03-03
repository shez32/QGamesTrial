using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

/// <summary>
/// Stage main loop
/// </summary>
public class StageLoop : MonoBehaviour
{
	#region static 
	static public StageLoop Instance { get; private set; }
	#endregion
	
	#region public or editor visible variables
	public TitleLoop titleLoop;

	[Header("Layout")]
	public Transform stageTransform;
	public Text stageScoreText;
	public GameObject restartMenu;
	public Transform healthPanel;
	public Transform powerUpPanel;
	public PostProcessVolume postProcessVolume;
	
	[SerializeField] private Camera defaultCamera;

	[Header("Prefab")]
	public Player prefabPlayer;
	public EnemySpawner prefabEnemySpawner;
	
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
	#endregion
	
	int gameScore = 0;

	private EnemySpawner spawner;
	
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
		//Debug.Log("Start StageCoroutine");

		SetupStage();

		while (true)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				//exit stage - have not implemented this
			}
			yield return null;
		}
	}
	#endregion
	
	void SetupStage()
	{
		Instance = this;

		gameScore = 0;
		RefreshScore();
		
		//Deactivate the restart menu and Display the Player HUD
		restartMenu.SetActive(false);
		healthPanel.gameObject.SetActive(true);
		powerUpPanel.gameObject.SetActive(true);

		//create player
		{
			Player player = Instantiate(prefabPlayer, stageTransform);
			if (player)
			{
				player.transform.position = new Vector3(0, -4, 0);
				
				//Calculate the linear limits the player can move in
				player.CalculateBounds(topCollider, bottomCollider, leftCollider, rightCollider);
			}
		}

		//create enemy spawner
		{
			{
				if (!spawner) spawner = Instantiate(prefabEnemySpawner, stageTransform);
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
		//Deactivate all UI elements
		restartMenu.SetActive(false);
		healthPanel.gameObject.SetActive(false);
		powerUpPanel.gameObject.SetActive(false);
		
		//delete all object in Stage
		{
			for (var n = 0; n < stageTransform.childCount; ++n)
			{
				Transform temp = stageTransform.GetChild(n);
				GameObject.Destroy(temp.gameObject);
			}
		}

		Instance = null;
		
		audioSource.Stop();
	}

	public void AddScore(int a_value)
	{
		gameScore += a_value;
		RefreshScore();
	}

	void RefreshScore()
	{
		if (stageScoreText)
		{
			stageScoreText.text = $"Score {gameScore:00000}";
		}
	}

	//function is called when player has lost all lives
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

	//function is called when player takes damage
	public void OnDamage()
	{
		if (defaultCamera != null)
		{
			StartCoroutine(CameraShake(0.2f, 0.3f));
		}
	}

	//Self Explanatory
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

	//Function used by restart button in restart menu
	public void RestartGame()
	{
		CleanupStage();
		SetupStage();
	}

	//Function used by main menu button in restart menu
	public void MainMenu()
	{
		CleanupStage();
		titleLoop.SetupTitle();
	}
	
}
