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
	public GameObject playerPrefab;
	[HideInInspector] public GameObject playerListHolder;
	PhotonView photonView;
	bool playerSpawned = false;

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
		playerListHolder = GameObject.FindGameObjectWithTag("LobbyPlayerHolder");
	}

    public void Update()
    {
        if (!playerSpawned && PhotonNetwork.IsConnectedAndReady)
        {
			Debug.Log("Calling Spawn");

			if (PhotonNetwork.LocalPlayer.IsLocal)
            {
				string username = PlayerPrefs.GetString("Username");

				PhotonNetwork.LocalPlayer.NickName = username;

				Color color = new Color(PlayerPrefs.GetFloat("colorRed"), PlayerPrefs.GetFloat("colorGreen"), PlayerPrefs.GetFloat("colorBlue"), PlayerPrefs.GetFloat("colorAlpha"));

				Instance.gameObject.GetPhotonView().RPC("PUNSpawnPlayer", RpcTarget.AllBuffered,
					username, color.r, color.g, color.b, color.a);
			}				


			

			/*			SpawnPlayer(PlayerPrefs.GetString("Username"),
										PlayerPrefs.GetFloat("colorRed"),
										PlayerPrefs.GetFloat("colorGreen"),
										PlayerPrefs.GetFloat("colorBlue"),
										PlayerPrefs.GetFloat("colorAlpha"));*/
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
	
	[PunRPC]
	void PUNSpawnPlayer(string username, float colorR, float colorG, float colorB, float colorA)
    {
		SpawnPlayer(username, colorR, colorG, colorB, colorA);
    }

	public void SpawnPlayer(string _username, float _colorR, float _colorG, float _colorB, float _colorA)
	{
		if (playerPrefab)
		{
			if (!playerSpawned)
            {
				playerSpawned = true;

				GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, playerListHolder.transform.position, playerListHolder.transform.rotation);

				if (player)
				{
					player.transform.SetParent(playerListHolder.transform);

					player.gameObject.name = _username;

					player.GetComponent<TMP_Text>().text = _username;
					player.GetComponent<TMP_Text>().color = new Color(_colorR, _colorG, _colorB, _colorA);
				}
				else
				{
					Debug.Log("No Player Instantaited");
				}
			}
			else
            {
				Debug.LogWarning("Player already spawned on " + PhotonNetwork.LocalPlayer);
            }
		}
		else
		{
			Debug.Log("[GameplayManager][Start](playerPrefab) No prefab set");
		}
	}
}
