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
	public TMP_Dropdown cosmetics;

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

	public void JoinFPSLobby()
	{
		//PhotonManager.Instance.username = input.text;

		PlayerPrefs.SetString("Username", PhotonManager.Instance.username);
		PlayerPrefs.SetFloat("colorRed", PhotonManager.Instance.color.r);
		PlayerPrefs.SetFloat("colorGreen", PhotonManager.Instance.color.g);
		PlayerPrefs.SetFloat("colorBlue", PhotonManager.Instance.color.b);
		PlayerPrefs.SetFloat("colorAlpha", PhotonManager.Instance.color.a);

		PhotonManager.Instance.JoinFPSLobby();
	}

	public void OnEndEditUsername()
	{
		PhotonManager.Instance.username = input.text;
	}

	public void QuitApplication()
	{
		PlayerPrefs.SetString("Username", PhotonManager.Instance.username);
		PlayerPrefs.SetFloat("colorRed", PhotonManager.Instance.color.r);
		PlayerPrefs.SetFloat("colorGreen", PhotonManager.Instance.color.g);
		PlayerPrefs.SetFloat("colorBlue", PhotonManager.Instance.color.b);
		PlayerPrefs.SetFloat("colorAlpha", PhotonManager.Instance.color.a);

#if UNITY_EDITOR

		UnityEditor.EditorApplication.isPlaying = false;

#endif

		Application.Quit();
	}	
}
