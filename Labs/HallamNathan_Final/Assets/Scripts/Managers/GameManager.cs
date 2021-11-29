using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

[ExecuteInEditMode]
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
	}

	private void Update()
	{
		maxWeightForLevel = Mathf.RoundToInt(level * Mathf.Pow((1 + baseAsteroidWeight), asteroidWeightMultiplier));
		maxScoreForLevel = Mathf.RoundToInt(level * Mathf.Pow((1 + baseScoreForLevel), levelScoreMultiplier));

		currentWeight = TestAsteroid.totalWeight;

		availableWeight = maxWeightForLevel - currentWeight;
	}
}
