using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ChatManager : MonoBehaviourPunCallbacks
{
	public TMP_Text chatRoomString;
	public TMP_InputField inputString;
	public Button enterButton;

	public static ChatManager Instance { get; private set; }

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
	}

	public void Update()
	{
		if (inputString.text.Length > 0)
		{
			enterButton.interactable = true;

			if (Input.GetKeyDown(KeyCode.Return))
			{
				SendMessage();
			}
		}
		else
		{
			enterButton.interactable = false;
		}
	}

	public void SendMessage()
	{
		if (inputString.text.Length != 0)
		{
			// Get player name from properties
			string playerName = PhotonNetwork.LocalPlayer.NickName;
			
			//Get player color from properties
			Color32 playerColor = new Color((float)PhotonNetwork.LocalPlayer.CustomProperties["colorRed"], (float)PhotonNetwork.LocalPlayer.CustomProperties["colorGreen"], (float)PhotonNetwork.LocalPlayer.CustomProperties["colorBlue"], (float)PhotonNetwork.LocalPlayer.CustomProperties["colorAlpha"]);
			
			//Change player name color in chat to the chosen color
			playerName = "<color=#" + playerColor.ColorToHex() + ">" + playerName + "</color>";

			RPCManager.Instance.gameObject.GetPhotonView()
			.RPC("UsernameRPC", RpcTarget.AllBuffered, 
				playerName, inputString.text);

			inputString.text = null;
		}
	}

	public void ChatOnConnectDisconnect(string username = null, string message = null)
	{
		RPCManager.Instance.gameObject.GetPhotonView()
		.RPC("UsernameRPC", RpcTarget.AllBuffered,
			message, username);
	}
}
