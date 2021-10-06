using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class ButtonManager : MonoBehaviour
{

	//Singleton 
	public static ButtonManager Instance { get; private set; }

	public TMP_InputField input;

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
}
