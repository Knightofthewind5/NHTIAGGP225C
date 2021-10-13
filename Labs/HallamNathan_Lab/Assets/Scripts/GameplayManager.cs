using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameplayManager : MonoBehaviourPunCallbacks
{
	public byte MaxPlayers = 4;
	public GameObject playerPrefab;
	[HideInInspector] public GameObject playerListHolder;
	PhotonView photonView;

	public void Awake()
	{
		photonView = gameObject.GetPhotonView();
		playerListHolder = GameObject.FindGameObjectWithTag("LobbyPlayerHolder");
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		PhotonNetwork.CurrentRoom.MaxPlayers = MaxPlayers;
		PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("Username");

		SpawnPlayer(PlayerPrefs.GetString("Username"), 
		PlayerPrefs.GetFloat("colorRed"), 
		PlayerPrefs.GetFloat("colorGreen"), 
		PlayerPrefs.GetFloat("colorBlue"), 
		PlayerPrefs.GetFloat("colorAlpha"));
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
	}

	[PunRPC]
	public void SpawnPlayer(string username, float colorR, float colorG, float colorB, float colorA)
	{
		if (playerPrefab)
		{
			GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, playerListHolder.transform.position, playerListHolder.transform.rotation);

			if (player)
			{
				player.transform.SetParent(playerListHolder.transform);

				player.gameObject.name = username;

				player.GetComponent<TMP_Text>().text = username;
				player.GetComponent<TMP_Text>().color = new Color(colorR, colorG, colorB, colorA);
			}
			else
			{
				Debug.Log("No Player Instantaited");
			}
		}
		else
		{
			Debug.Log("[GameplayManager][Start](playerPrefab) No prefab set");
		}
	}
}
