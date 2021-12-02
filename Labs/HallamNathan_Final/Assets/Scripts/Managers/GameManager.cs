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
	PhotonView PV;

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
	public float gameStartWait = 5f;
	public List<WeaponStats> weapons = new List<WeaponStats>();

	public ShuttleInfo shuttleStats;

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
		yield return new WaitForSecondsRealtime(2);

		SpawnPlayer();
	}

	void SpawnPlayer()
	{
		PV.RPC("SpawnPlayerRPC", RpcTarget.All);
	}

	[PunRPC]
	public void SpawnPlayerRPC()
	{
		foreach (var shuttle in shuttleStats.shuttles)
		{
			if (shuttle.shuttleName == PhotonManager.Instance.properties["shuttleName"].ToString())
			{
				PhotonNetwork.Instantiate(shuttle.shuttlePrefab.name, Vector2.zero, Quaternion.identity);
			}
		}
	}
}
