using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class DeathmatchManager : GeneralGameManager
{
	public Dictionary<string, int> playerScores = new Dictionary<string, int>();

	bool isSpawning;

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		StartCoroutine(WaitForSpawn());
	}

	IEnumerator WaitForSpawn()
	{
		yield return new WaitUntil(() => players.Count == PhotonNetwork.CurrentRoom.PlayerCount);

		foreach(GameObject player in players)
		{
			playerScores.Add(player.GetPhotonView().Controller.NickName, 0);
			Debug.Log("Adding " + player.GetPhotonView().Controller.NickName);
		}
	}

	public override void LateUpdate()
	{
		base.LateUpdate();

		if (!hasSpawned && !isSpawning)
		{
			Debug.Log("Respawning Player");

			isSpawning = true;

			StartCoroutine(SpawnWaitTimer());
		}
		else if (hasSpawned)
		{
			isSpawning = false;
		}

		if (timer <= 0 && !gameDone)
		{
			gameDone = true;

			int highestScore = playerScores.Values.Max();
			string winnerName = "";

			foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
			{
				Debug.Log(player.NickName + " SCORE: " + playerScores[player.NickName]);

				if (playerScores[player.NickName] == highestScore)
				{
					winnerName = player.NickName;
				}
			}

			WinText.text = "Winner: " + winnerName;
			WinCanvas.SetActive(true);
			timer = 5f;
		}
	}

	[PunRPC]
	public void RPCAddToScore(string name)
	{
		if (name != null)
		{
			playerScores[name]++;
		}
		else
		{
			Debug.LogError("[DeathmatchManager][RPCAddToScore] given name is NULL");
		}
	}
}
