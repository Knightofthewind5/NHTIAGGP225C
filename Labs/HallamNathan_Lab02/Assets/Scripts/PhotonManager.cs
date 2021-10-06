using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
	#region Variables and Singletons
	/// <summary>
	/// Version of game
	/// </summary>
	string gameVersion = "0";
	RoomOptions roomOptions = new RoomOptions();
	static string gameplayLevel = "GameScene";
	static string chatLevel = "ChatRoom";
	private string username;
	PhotonView PV;

	//Singleton 
	public static PhotonManager Instance { get; private set; }

	#endregion Variables and Singletons

	#region Functions

	#region Start and Awake
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
			PV = gameObject.AddComponent<PhotonView>();
			PV.ViewID = 999;
			DontDestroyOnLoad(this);
		}

		PhotonNetwork.AutomaticallySyncScene = true;
		roomOptions.MaxPlayers = 4;
	}

	public void Start()
	{
		Connect();
	}
	#endregion Start and Awake

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
		PhotonNetwork.LoadLevel(gameplayLevel);
	}
	#endregion

	#region Join Rooms
	public void JoinRandomRoom()
	{
		Debug.Log("[PhotonManager][JoinRandomRoom] Joining Random Room...");

		PhotonNetwork.JoinRandomOrCreateRoom();
		
		PhotonNetwork.LoadLevel(gameplayLevel);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("[PhotonManager][OnJoinedRoom] Room " + PhotonNetwork.CurrentRoom.Name + " Joined!");
	}

	public void JoinChatroom()
	{
		if (ButtonManager.Instance)
		{
			if (!string.IsNullOrEmpty(ButtonManager.Instance.input.text) && PhotonNetwork.CountOfPlayersInRooms < 5)
			{
				if (Instance != null)
				{
					Debug.Log("[PhotonManager][JoinChatroom] Joining Chatroom...");

					username = ButtonManager.Instance.input.text;

					PlayerPrefs.SetString("Username", username);

					PhotonNetwork.JoinRandomOrCreateRoom();

					PhotonNetwork.LoadLevel(chatLevel);
				}
			}
			else
			{
				Debug.LogError("[PhotonManager][JoinChatroom] No username detected - Unable to join chatroom!");
			}
		}	
	}

	#endregion Join Rooms

	#region Disconnection and Failures
	public override void OnLeftRoom()
	{
		PhotonNetwork.LoadLevel("SampleScene");

		SceneManager.LoadScene("SampleScene");
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
	public void SetPlayerCount()
	{
		RPCChat.Instance.memberCounter.text = RPCChat.Instance.playerCount + "/4";
	}

	#endregion Chatrooms

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