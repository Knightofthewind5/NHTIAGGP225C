using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneButtonManager : MonoBehaviour
{
	public void ReturnToMain()
	{
		StopAllCoroutines();
		PhotonNetwork.LeaveRoom();
	}
}
