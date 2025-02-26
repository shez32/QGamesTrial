using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Title Screen Loop
/// </summary>
public class TitleLoop : MonoBehaviour
{
	public StageLoop m_stage_loop;

	[Header("Layout")]
	public Transform m_ui_title;
	public Transform mainMenu;
	
	[Header("Audio")]
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip menuAudioClip;

	//------------------------------------------------------------------------------

	private void Start()
	{
		//default start
		SetupTitle();
	}

	public void StartTitleLoop()
	{
		CleanupTitle();
		
		m_stage_loop.StartStageLoop();
	}
	
	public void SetupTitle()
	{
		m_ui_title.gameObject.SetActive(true);
		mainMenu.gameObject.SetActive(true);

		if (audioSource != null)
		{
			audioSource.PlayOneShot(menuAudioClip);
			audioSource.loop = true;
		}
	}

	void CleanupTitle()
	{
		m_ui_title.gameObject.SetActive(false);
		mainMenu.gameObject.SetActive(false);
		
		audioSource.Stop();
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
