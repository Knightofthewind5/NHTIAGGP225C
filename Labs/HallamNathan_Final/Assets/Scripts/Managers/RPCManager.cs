using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class RPCManager : MonoBehaviour
{
	public static RPCManager Instance;

	void Awake()
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

	[PunRPC]
	public void UsernameRPC(string _username, string _chat)
	{
		ChatManager.Instance.chatRoomString.text += "\n" + _username + ": " + _chat;
		//Debug.Log(string.Format("ChatMessage {0} {1}", _username, _chat));
	}

	[PunRPC]
	public void SetLobbyTimerRPC(string _value)
	{
		GameLobbyManager.Instance._lobbyStatus.text = _value;
	}
}
