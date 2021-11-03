using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{

	public void Replay()
	{
		SceneManager.LoadScene("Game");
		Time.timeScale = 1f;
	}

	public void LoadMenu()
	{
		SceneManager.LoadScene("Main");
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
}