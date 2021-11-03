using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class BulletRoundManager : GeneralGameManager
{
	bool gameStart = false;

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();

		if (!gameStart && playersAlive == PhotonNetwork.CurrentRoom.PlayerCount)
		{
			gameStart = true;
		}

		if (playersAlive == 1 && !gameDone && gameStart)
		{
			gameDone = true;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			DataSync DS = player.GetComponent<DataSync>();
			WinText.text = "Winner: " + DS.username.text;
			WinCanvas.SetActive(true);
			timer = 5f;
		}			
	}	
}