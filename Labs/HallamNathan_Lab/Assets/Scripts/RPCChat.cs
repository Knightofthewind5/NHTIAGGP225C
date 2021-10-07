using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RPCChat : MonoBehaviourPunCallbacks
{
	public TMP_Text username;
	public TMP_Text chatRoomString;
	public TMP_InputField inputString;
	public TMP_Text memberCounter;
	public int playerCount;


	public static RPCChat Instance { get; private set; }

	private void Awake()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}

		//username1.text = PlayerPrefs.GetString("Username");
		//username.text = PhotonNetwork.PlayerList[0].NickName;
	}

	public void Update()
	{
		if (Instance && PhotonNetwork.InRoom)
		{
			playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
			PhotonManager.Instance.gameObject.GetPhotonView().RPC("SetUserList", RpcTarget.All, playerCount);
		}
	}

	public void SendMessage()
	{
		PhotonManager.Instance.gameObject.GetPhotonView()
		.RPC("UsernameRPC", RpcTarget.AllBuffered, PlayerPrefs.GetString("Username"), inputString.text);

		inputString.text = "";
	}

	public void LeaveRoom()
	{
		if (PhotonNetwork.CountOfPlayers <= 1)
		{
			PhotonNetwork.DestroyAll();
		}
		PhotonNetwork.LeaveRoom();
	}
}
