﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
	#region Variables and Singletons
	/// <summary>
	/// Version of game
	/// </summary>
	string gameVersion = "0";
	RoomOptions roomOptions = new RoomOptions();
	static string fpsLobby = "FPSLobby";
	private string username;
	PhotonView photonView;

	Button StartButton;

	//Singleton 
	public static PhotonManager Instance { get; private set; }

	#endregion Variables and Singletons

	#region Functions

	#region Start Awake
	public void Awake()
	{	
		if (Instance)
		{
			Debug.Log("Already have a photon view");
			Destroy(GetComponent<PhotonView>());
		}
		else
		{
			Instance = this;
			photonView = gameObject.AddComponent<PhotonView>();
			photonView.ViewID = 999;
			DontDestroyOnLoad(this);
		}

		PhotonNetwork.AutomaticallySyncScene = true;
		roomOptions.MaxPlayers = 4;

		StartButton = GameObject.FindGameObjectWithTag("StartButton").GetComponent<Button>();
	}

	public void Start()
	{
		Connect();
	}
	#endregion Start Awake

	#region Connections
	/// <summary>
	/// Connect user to Master server.
	/// </summary>
	public void Connect()
	{
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = gameVersion;
			PhotonNetwork.AutomaticallySyncScene = true;
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("[PhotonManager][OnConnectedToMaster]");
		StartButton.interactable = true;

		if (ButtonManager.Instance)
		{
			ButtonManager.Instance.chosenColor.color = new Color(PlayerPrefs.GetFloat("colorRed"), PlayerPrefs.GetFloat("colorGreen"), PlayerPrefs.GetFloat("colorBlue"), PlayerPrefs.GetFloat("colorAlpha"));
			ButtonManager.Instance.input.text = PlayerPrefs.GetString("Username");
		}
	}
	#endregion Connections

	#region Create Rooms
	public void CreateRoom()
	{
		PhotonNetwork.CreateRoom(null, roomOptions);
		Debug.Log("[PhotonManager][CreateRoom] Creating room...");
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("[PhotonManager][OnCreateRoom]");
	}
	#endregion

	#region Join Rooms
	public void JoinRandomRoom()
	{
		Debug.Log("[PhotonManager][JoinRandomRoom] Joining Random Room...");

		PhotonNetwork.JoinRandomOrCreateRoom();
		
		PhotonNetwork.LoadLevel(fpsLobby);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("[PhotonManager][OnJoinedRoom] Room " + PhotonNetwork.CurrentRoom.Name + " Joined!");
	}

	public void JoinFPSLobby()
	{
		if (ButtonManager.Instance)
		{
			if (!string.IsNullOrEmpty(ButtonManager.Instance.input.text) && PhotonNetwork.CountOfPlayersInRooms < 5)
			{
				if (Instance != null)
				{
					Debug.Log("[PhotonManager][JoinFPSLobby] Joining Lobby for FPS...");

					username = ButtonManager.Instance.input.text;

					PlayerPrefs.SetString("Username", username);

					PhotonNetwork.JoinRandomOrCreateRoom();

					if (PhotonNetwork.IsMasterClient)
					{
						Debug.Log("is master");
						SceneManager.LoadScene(fpsLobby);
					}
					else
					{
						PhotonNetwork.LoadLevel(fpsLobby);
					}
				}
			}
			else
			{
				Debug.LogError("[PhotonManager][JoinFPSLobby] No username detected - Unable to join lobby!");
			}
		}
	}

	#endregion Join Rooms

	#region Disconnection and Failures
	public override void OnLeftRoom()
	{
		PhotonNetwork.LoadLevel("MainMenu");

		SceneManager.LoadScene("MainMenu");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("[PhotonManager][OnDisconnect] Disconnected from Room due to " + cause);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("[PhotonManager][OnCreateRoomFailed] " + message);
		OnJoinedRoom();
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("[PhotonManager][OnCreateRoomFailed] " + message);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("[PhotonManager][OnJoinRandomFailed] " + message);
	}
	#endregion Disconnection and Failures

	#region Chatrooms

	[PunRPC]
	public void UsernameRPC(string _username, string _chat)
	{
		RPCChat.Instance.chatRoomString.text += "\n" + _username + ": " + _chat;
		Debug.Log(string.Format("ChatMessage {0} {1}", _username, _chat));
	}


	[PunRPC]
	public void SetUserList(int playerCount)
	{
		//RPCChat.Instance.memberCounter.text = playerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

		RPCChat chat = RPCChat.Instance;

		chat.memberCounter.text = playerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
		//PhotonNetwork.CurrentRoom.MaxPlayers = 4;
	}

	[PunRPC]
	public void SetGameTimer(float timer)
	{
		RPCChat chat = RPCChat.Instance;

		if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
		{
			chat.timerText.text = "Game Start in: " + timer.ToString("F2");
		}
		else
		{
			chat.timerText.text = "Waiting for Players...";
		}
	}

	[PunRPC]
	public void JoinGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			SceneManager.LoadScene("FPSScene");
		}
		else
		{
			PhotonNetwork.LoadLevel("FPSScene");
		}
	}

	#endregion Chatrooms

	[PunRPC]
	public void SetGameHUDTimer(float timer)
	{
		GameManager3 GM = FindObjectOfType<GameManager3>();

		GM.GameTime.text = "Time Remaining: " + timer.ToString("F2");
	}

	[PunRPC]
	public void RemoveFromAlivePlayers()
	{
		GameManager3 GM = FindObjectOfType<GameManager3>();

		GM.playersAlive--;
	}

	[PunRPC]
	public void DestroyObject(int id)
	{
		GameObject[] objs = FindObjectsOfType<GameObject>();

		foreach (GameObject go in objs)
		{
			if (go.GetInstanceID() == id)
			{
				Destroy(go);
				return;
			}
		}
	}


	#endregion Functions
}