using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// Version of game
    /// </summary>
    string gameVersion = "0";
    RoomOptions roomOptions = new RoomOptions();
    static string gameplayLevel = "GameScene";

    //Singleton 
    public static PhotonManager Instance { get; private set; }

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

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, roomOptions);
        Debug.Log("[PhotonManager][CreateRoom] Creating room...");
    }

    public void JoinRandomRoom()
    {
        Debug.Log("[PhotonManager][JoinRandomRoom] Joining Random Room...");
        
        if (PhotonNetwork.JoinRandomRoom())
        {
            Debug.Log("[PhotonManager][JoinRandomRoom] No Rooms Found!");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("[PhotonManager][OnCreateRoom]");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("[PhotonManager][OnJoinedRoom] Joining Room...");

        PhotonNetwork.LoadLevel(gameplayLevel);
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
}
