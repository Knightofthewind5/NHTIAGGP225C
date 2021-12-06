using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
	//Singleton 
	public static GameManager Instance { get; private set; }
	public PhotonView PV { get; private set; }

	[Range(1, 100)]
	public int level = 1;
	public int baseScoreForLevel = 65;
	public float levelScoreMultiplier = 1.1f;

	public int baseAsteroidWeight = 11;
	public float asteroidWeightMultiplier = 1.3f;

	public int maxWeightForLevel;
	public int maxScoreForLevel;
	int currentWeight;
	public int availableWeight;
	public GameObject baseGameObject;
	public List<AsteroidStats> asteroids = new List<AsteroidStats>();
	public List<WeaponStats> weapons = new List<WeaponStats>();

	public ShuttleInfo shuttleStats;
	public CanvasGroup DeathCanvas;
	public List<GameObject> AlivePlayers = new List<GameObject>();

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			PV = gameObject.GetPhotonView();
		}

		baseAsteroidWeight = GameSettingsManager.Instance.baseAsteroidWeight;
		asteroidWeightMultiplier = GameSettingsManager.Instance.asteroidWeightMultiplier;
		baseScoreForLevel = GameSettingsManager.Instance.baseScorePerLevel;
		levelScoreMultiplier = GameSettingsManager.Instance.levelScoreMultiplier;

		StartCoroutine(WaitTimer());
	}

	private void Update()
	{
		maxWeightForLevel = Mathf.RoundToInt(level * Mathf.Pow((1 + baseAsteroidWeight), asteroidWeightMultiplier));
		maxScoreForLevel = Mathf.RoundToInt(level * Mathf.Pow((1 + baseScoreForLevel), levelScoreMultiplier));

		currentWeight = Asteroid.totalWeight;

		availableWeight = maxWeightForLevel - currentWeight;
	}

	IEnumerator WaitTimer()
	{
		yield return new WaitForSecondsRealtime(GameSettingsManager.Instance.playerSpawnWaitTime);

		SpawnPlayer();
	}

	void SpawnPlayer()
	{
		foreach (var shuttle in shuttleStats.shuttles)
		{
			if (shuttle.shuttleName == PhotonNetwork.LocalPlayer.CustomProperties["shuttleName"].ToString())
			{
				GameObject player = PhotonNetwork.Instantiate(shuttle.shuttlePrefab.name, Vector2.zero, Quaternion.identity);

				PV.RPC("AddAlivePlayerRPC", RpcTarget.All, player.GetPhotonView().ViewID);
			}
		}
	}

	[PunRPC]
	public void AddAlivePlayerRPC(int ViewID)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			GameObject player = PhotonNetwork.GetPhotonView(ViewID).gameObject;

			AlivePlayers.Add(player);
		}
	}

	[PunRPC]
	public void RemoveAlivePlayerRPC(int ViewID)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			GameObject player = PhotonNetwork.GetPhotonView(ViewID).gameObject;

			AlivePlayers.Remove(player);
		}
	}
}
