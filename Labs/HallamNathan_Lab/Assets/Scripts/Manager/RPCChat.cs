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
	public bool lobby;
	public TMP_Dropdown gamemodeDropdown;
	public byte MaxPlayers = 4;

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
			playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

			PhotonManager.Instance.gameObject.GetPhotonView().RPC("SetUserList", RpcTarget.All, playerCount);

			if (PhotonNetwork.IsMasterClient)
			{
				if (lobby)
				{
					if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
					{
						timerToStart -= Time.deltaTime;

						PhotonManager.Instance.photonView.RPC("SetGameTimer", RpcTarget.All, timerToStart);

						if (timerToStart <= 0)
						{
							timerToStart = 0;

							if (!joinGame)
							{
								joinGame = true;

								if (Instance.gamemodeDropdown.GetCurrentSelectionText() == "Bullet Round Elimination")
								{

									PhotonNetwork.LoadLevel("BREScene");
								}
								else if (Instance.gamemodeDropdown.GetCurrentSelectionText() == "Deathmatch")
								{
									PhotonNetwork.LoadLevel("DMScene");
								}
								else if (Instance.gamemodeDropdown.GetCurrentSelectionText() == "One in the Chamber")
								{
									PhotonNetwork.LoadLevel("OITCScene");
								}
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
		.RPC("UsernameRPC", RpcTarget.AllBuffered, PhotonManager.Instance.username, inputString.text);

		inputString.text = "";
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		PhotonNetwork.CurrentRoom.MaxPlayers = MaxPlayers;
	}

	public void LeaveRoom()
	{
		if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
		{
			PhotonNetwork.DestroyAll();
		}

		Debug.Log("[RPCChat][LeaveRoom]");
		PhotonNetwork.LeaveRoom();
	}
}

public static class DropDownExtensions
{
	public static string GetCurrentSelectionText(this TMP_Dropdown dropdown)
	{
		return dropdown.options[dropdown.value].text;
	}
}
