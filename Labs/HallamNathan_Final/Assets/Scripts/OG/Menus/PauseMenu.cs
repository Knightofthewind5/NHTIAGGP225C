using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static bool GameIsPaused = false;
	public static bool QuitIsOpen = false;

	public GameObject PauseMenuUI;
	public GameObject QuitMenu;
	public GameObject HUD;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused == true)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}

	public void Resume()
	{
		PauseMenuUI.SetActive(false);
		HUD.SetActive(true);
		Time.timeScale = 1f;
		GameIsPaused = false;
	}

	void Pause()
	{
		PauseMenuUI.SetActive(true);
		HUD.SetActive(false);
		Time.timeScale = 0f;
		GameIsPaused = true;
	}

	public void LoadMenu()
	{
		SceneManager.LoadScene("Main");
		GameIsPaused = false;
		Time.timeScale = 1f;
	}

	public void QuitGame()
	{
		Debug.Log("Quit App");
		Application.Quit();

#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlaying)
		{
			UnityEditor.EditorApplication.isPlaying = false;
		}
#endif
	}

	public void ToggleQuitMenu()
	{
		if (QuitIsOpen == true)
		{
			QuitMenu.SetActive(false);
			QuitIsOpen = false;
		}
		else
		{
			QuitMenu.SetActive(true);
			QuitIsOpen = true;
		}
	}
}
