using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	static string gameplayLevel = "GameScene";

	//Singleton 
	public static PhotonManager Instance { get; private set; }
	#endregion

	#region Start and Awake
	public void Awake()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}

		PhotonNetwork.AutomaticallySyncScene = true;
		roomOptions.MaxPlayers = 4;
	}

	public void Start()
	{
		Connect();
	}
	#endregion

	#region Functions

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
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("[PhotonManager][OnConnectedToMaster]");
	}
	#endregion

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
		PhotonNetwork.JoinRandomOrCreateRoom();
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("[PhotonManager][OnJoinedRoom] Joining Room...");

		PhotonNetwork.LoadLevel(gameplayLevel);
	}
	#endregion

	#region Disconnection and Failures
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
	#endregion

	#endregion
}
