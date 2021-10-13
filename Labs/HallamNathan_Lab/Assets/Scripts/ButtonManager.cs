using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;


public class ButtonManager : MonoBehaviour
{

	//Singleton 
	public static ButtonManager Instance { get; private set; }

	public TMP_InputField input;
	public Image chosenColor;

	public void Awake()
	{
		Instance = this;
	}

	public void JoinRoom()
	{
		PhotonManager.Instance.CreateRoom();
	}

	public void JoinRandom()
	{
		PhotonManager.Instance.JoinRandomRoom();
	}

	public void CreateFPSLobby()
	{
		PhotonManager.Instance.CreateFPSLobby();
	}

	public void JoinFPSLobby()
	{
		PhotonManager.Instance.JoinFPSLobby();
	}

	public void QuitApplication()
	{
#if UNITY_EDITOR

		UnityEditor.EditorApplication.isPlaying = false;

#endif

		Application.Quit();
	}	
}
