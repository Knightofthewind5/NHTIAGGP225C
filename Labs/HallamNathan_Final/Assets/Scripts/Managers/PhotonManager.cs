using System.Collections;
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
	new PhotonView photonView;

	public ExitGames.Client.Photon.Hashtable properties { get; private set; } = new ExitGames.Client.Photon.Hashtable();

	public string username;
	public Color color = new Color(1f, 1f, 1f, 1f);

	//Singleton 
	public static PhotonManager Instance { get; private set; }

	#endregion Variables and Singletons

	#region Functions

	#region Start Awake
	public void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			photonView = gameObject.AddComponent<PhotonView>();
			photonView.ViewID = 199;
			DontDestroyOnLoad(this);
		}
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
			username = PlayerPrefs.GetString("Username");
			color = new Color(PlayerPrefs.GetFloat("colorRed"), PlayerPrefs.GetFloat("colorGreen"), PlayerPrefs.GetFloat("colorBlue"), PlayerPrefs.GetFloat("colorAlpha"));

			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = gameVersion;
			PhotonNetwork.AutomaticallySyncScene = true;
		}
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("[PhotonManager][OnConnectedToMaster]");
		MainMenuButtonManager.Instance.MultiplayerButton.interactable = true;

		if (!PhotonNetwork.OfflineMode)
		{
			PhotonNetwork.JoinLobby(TypedLobby.Default);
		}
	}
	#endregion Connections

	#region Create Rooms
	public void CreateRoom(string roomName = "Player Room", byte maxPlayers = 4)
	{
		if (roomName == "Player Room")
		{
			roomName = Random.Range(100000000, 999999999).ToString();
		}


		roomOptions.MaxPlayers = maxPlayers;
		roomOptions.IsVisible = true;

		SetCustomProperties();

		PhotonNetwork.CreateRoom(roomName, roomOptions);
		//Debug.Log("[PhotonManager][CreateRoom] Creating " + roomName + " for " + maxPlayers + " Players");
	}

	public override void OnCreatedRoom()
	{
		//Debug.Log("[PhotonManager][OnCreateRoom]");
	}
	#endregion

	#region Join Rooms
	public void JoinRoom(string roomName)
	{
		SetCustomProperties();

		PhotonNetwork.JoinRoom(roomName);
	}

	public void JoinRandomRoom()
	{
		Debug.Log("[PhotonManager][JoinRandomRoom] Joining Random Room...");

		PhotonNetwork.JoinRandomOrCreateRoom();

		PhotonNetwork.LoadLevel("TestScene");
	}

	public override void OnJoinedRoom()
	{
		//Debug.Log("[PhotonManager][OnJoinedRoom] " + PhotonNetwork.CurrentRoom.Name + " Joined!");	
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
		Debug.Log("[PhotonManager][OnJoinRoomFailed] " + message);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("[PhotonManager][OnJoinRandomFailed] " + message);
	}
	#endregion Disconnection and Failures

	public void SetCustomProperties()
	{
		PhotonNetwork.LocalPlayer.NickName = username;

		properties["colorRed"] = color.r;
		properties["colorGreen"] = color.g;
		properties["colorBlue"] = color.b;
		properties["colorAlpha"] = color.a;

		PlayerPrefs.SetString("Username", username);
		PlayerPrefs.SetFloat("colorRed", color.r);
		PlayerPrefs.SetFloat("colorGreen", color.g);
		PlayerPrefs.SetFloat("colorBlue", color.b);
		PlayerPrefs.SetFloat("colorAlpha", color.a);

		PhotonNetwork.LocalPlayer.CustomProperties = properties;
	}

	#endregion Functions
}