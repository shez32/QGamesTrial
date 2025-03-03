using UnityEngine;

/// <summary>
/// Title Screen Loop
/// </summary>
public class TitleLoop : MonoBehaviour
{
	public StageLoop stageLoop;

	[Header("Layout")]
	public Transform uiTitle;
	public Transform mainMenu;
	
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip menuAudioClip;
	
	private void Start()
	{
		//default start
		SetupTitle();
	}

	public void StartTitleLoop()
	{
		CleanupTitle();
		
		stageLoop.StartStageLoop();
	}
	
	public void SetupTitle()
	{
		uiTitle.gameObject.SetActive(true);
		mainMenu.gameObject.SetActive(true);

		if (audioSource != null)
		{
			audioSource.PlayOneShot(menuAudioClip);
			audioSource.loop = true;
		}
	}
	
	void CleanupTitle()
	{
		uiTitle.gameObject.SetActive(false);
		mainMenu.gameObject.SetActive(false);
		
		audioSource.Stop();
	}

	//Function used by quit button in Main menu
	public void QuitGame()
	{
		Application.Quit();
	}
}
