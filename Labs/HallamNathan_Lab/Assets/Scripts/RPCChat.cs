using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RPCChat : MonoBehaviourPunCallbacks
{
	public TMP_Text chatRoomString;
	public TMP_InputField inputString;
	public TMP_Text memberCounter;
	public TMP_Text timerText;
	public int playerCount;
	public float gameStartTimer = 5f;
	public float timerToStart = 0f;
	bool joinGame = false;

	public static RPCChat Instance { get; private set; }

	private void Awake()
	{
		timerToStart = gameStartTimer;

		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void Update()
	{
		if (Instance && PhotonNetwork.InRoom)
		{
			if (memberCounter != null)
			{
				playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

				PhotonManager.Instance.gameObject.GetPhotonView().RPC("SetUserList", RpcTarget.All, playerCount);

				if (PhotonNetwork.IsMasterClient)
				{
					if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
					{
						timerToStart -= Time.deltaTime;

						PhotonManager.Instance.photonView.RPC("SetGameTimer", RpcTarget.All, timerToStart);

						if (timerToStart <= 0)
						{
							timerToStart = 0;
							Debug.Log("Max players in lobby, loading game...");

							if (!joinGame)
							{
								joinGame = true;
								PhotonManager.Instance.photonView.RPC("JoinGame", RpcTarget.All);
							}
						}
					}
					else
					{
						timerToStart = gameStartTimer;
						PhotonManager.Instance.photonView.RPC("SetGameTimer", RpcTarget.All, timerToStart);
					}
				}
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
		if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
		{
			//Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
			PhotonNetwork.DestroyAll();
		}
		PhotonNetwork.LeaveRoom();
	}
}
