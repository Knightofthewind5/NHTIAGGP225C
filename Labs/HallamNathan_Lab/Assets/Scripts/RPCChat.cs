using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RPCChat : MonoBehaviour
{
	public TMP_Text username;
	public TMP_Text chatRoomString;
	public TMP_InputField inputString;
	public TMP_Text memberCounter;
	public int playerCount;
	public float updateTick;


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

		username.text = PlayerPrefs.GetString("Username");
	}

	public void Update()
	{
		updateTick++;


		if (Instance)
		{
			if (playerCount <= 4 && updateTick > 20) // UpdateTick is needed to prevent noinstance error occuring. Error did not cause problems but do not like the red!
			{
				playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
				memberCounter.text = playerCount + "/4";
				PhotonNetwork.CurrentRoom.MaxPlayers = 4;
				updateTick = 0;
			}
			else if (playerCount > 4)
			{

			}
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
