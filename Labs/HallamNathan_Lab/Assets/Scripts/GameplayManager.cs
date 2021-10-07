using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameplayManager : MonoBehaviourPunCallbacks
{
	public GameObject playerPrefab;

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		
		if (playerPrefab)
		{
			GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);

			PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("Username");
		}
		else
		{
			Debug.Log("[GameplayManager][Start](playerPrefab) No prefab set");
		}
	}
}
