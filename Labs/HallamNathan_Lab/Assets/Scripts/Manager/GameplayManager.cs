using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameplayManager : MonoBehaviourPunCallbacks
{
	public byte MaxPlayers = 4;
	PhotonView photonView;
	public TMP_Dropdown gamemodeDropdown;

	public static GameplayManager Instance { get; private set; }

	public void Awake()
	{
		if (Instance)
		{
			Debug.Log("GameplayManager already exists");
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		photonView = gameObject.GetPhotonView();

		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log("IsMasterClient");

			gamemodeDropdown.gameObject.SetActive(true);
		}
		else
		{
			//gamemodeDropdown.gameObject.SetActive(false);
		}
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		PhotonNetwork.CurrentRoom.MaxPlayers = MaxPlayers;

/*        SpawnPlayer(PlayerPrefs.GetString("Username"),
		PlayerPrefs.GetFloat("colorRed"),
		PlayerPrefs.GetFloat("colorGreen"),
		PlayerPrefs.GetFloat("colorBlue"),
		PlayerPrefs.GetFloat("colorAlpha"));*/
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
	}
}
