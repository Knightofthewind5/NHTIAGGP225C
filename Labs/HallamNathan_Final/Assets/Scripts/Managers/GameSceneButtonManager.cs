using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameSceneButtonManager : MonoBehaviour
{
	public void ReturnToLobby()
	{
		PhotonNetwork.LoadLevel("GameLobby");
	}

	public void RestartGame()
	{
		RPCManager.Instance.PV.RPC("RestartGameRPC", RpcTarget.All);
	}

	public void ReturnToMain()
	{
		StopAllCoroutines();
		PhotonNetwork.LeaveRoom();
	}
}
