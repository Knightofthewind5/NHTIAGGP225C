using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

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
	public int scoreForLastLevel;
	int currentWeight;
	public float currentScore;
	public float currentScoreMX = 1f;
	public int availableWeight;
	public int livesRemaining;
	public GameObject baseGameObject;
	public List<AsteroidStats> asteroids = new List<AsteroidStats>();
	public List<WeaponStats> weapons = new List<WeaponStats>();

	public ShuttleInfo shuttleStats;
	public CanvasGroup DeathCanvas;
	public List<GameObject> AlivePlayers = new List<GameObject>();

	public Image scoreMXBar;
	public TMP_Text scoreText;
	public TMP_Text scoreMXText;
	public Image levelBar;
	public TMP_Text levelText;
	public TMP_Text livesText;
	public TMP_Text respawnCounter;
	public Button restartGame;
	public Button returnToLobby;
	float spawnTime = 0;
	IEnumerator spawnTimer;
	public bool localPlayerAlive = false;
	public float barFillStart = 0.24f;

	public CanvasGroup noShieldVignette;

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
		livesRemaining = GameSettingsManager.Instance.playerLives + PhotonNetwork.CurrentRoom.PlayerCount;

		spawnTimer = WaitTimer();
		StartCoroutine(spawnTimer);
	}

	private void Update()
	{
		scoreForLastLevel = 0;

		maxWeightForLevel = Mathf.RoundToInt(level * Mathf.Pow((1 + baseAsteroidWeight), asteroidWeightMultiplier));

		for (int i = level; i > 0; i--)
		{
			scoreForLastLevel += Mathf.RoundToInt((level - i) * Mathf.Pow((1 + baseScoreForLevel), levelScoreMultiplier));
		}

		maxScoreForLevel = Mathf.RoundToInt(level * Mathf.Pow((1 + baseScoreForLevel), levelScoreMultiplier) + scoreForLastLevel);

		currentWeight = Asteroid.totalWeight;

		availableWeight = maxWeightForLevel - currentWeight;

		UpdateInfoBars();

		if (PhotonNetwork.IsMasterClient && livesRemaining == 0 && !restartGame.gameObject.activeInHierarchy)
		{
			restartGame.gameObject.SetActive(true);
			returnToLobby.gameObject.SetActive(true);
		}
	}

	void UpdateInfoBars()
	{
		float levelPercentage = (currentScore - scoreForLastLevel) / (maxScoreForLevel - scoreForLastLevel);
		float levelBase = (1 / (1 - barFillStart));

		levelBar.fillAmount = (levelPercentage / levelBase) + barFillStart;

		float scorePercentage = currentScoreMX - (Mathf.Floor(currentScoreMX));
		float scoreBase = (1 / (1 - barFillStart));

		scoreMXBar.fillAmount = (scorePercentage / scoreBase) + barFillStart;



		if (currentScore >= maxScoreForLevel)
		{
			level++;
		}

		levelText.text = level.ToString();
		scoreText.text = currentScore.ToString("F1");
		scoreMXText.text = "X" + currentScoreMX.ToString("F1");

		livesText.text = livesRemaining.ToString();

		if (spawnTimer == null && !localPlayerAlive && livesRemaining > 0)
		{
			spawnTimer = SpawnTimer(5f);
			StartCoroutine(spawnTimer);
		}
		else if (spawnTimer == null && !localPlayerAlive && livesRemaining == 0)
		{
			respawnCounter.text = "No lives remaining!";
		}
	}

	IEnumerator SpawnTimer(float time)
	{
		spawnTime = time;

		do
		{
			spawnTime -= 1;
			respawnCounter.text = "Respawning in: " + spawnTime;
			yield return new WaitForSeconds(1f);
		}
		while (spawnTime > 0);

		yield return new WaitForSeconds(1f);

		SpawnPlayer();
		DeathCanvas.SetActive(false);

		spawnTimer = null;
	}

	IEnumerator WaitTimer()
	{
		yield return new WaitForSecondsRealtime(GameSettingsManager.Instance.playerSpawnWaitTime);

		SpawnPlayer();

		spawnTimer = null;
	}

	void SpawnPlayer()
	{
		foreach (var shuttle in shuttleStats.shuttles)
		{
			if (shuttle.shuttleName == PhotonNetwork.LocalPlayer.CustomProperties["shuttleName"].ToString())
			{
				GameObject player = PhotonNetwork.Instantiate(shuttle.shuttlePrefab.name, Vector2.zero, Quaternion.identity);

				PV.RPC("AddAlivePlayerRPC", RpcTarget.All, player.GetPhotonView().ViewID);

				localPlayerAlive = true;
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

		livesRemaining -= 1;
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

	[PunRPC]
	public void UpdateInfoBarValuesRPC(int score)
	{
		currentScore += (score * currentScoreMX);
		float addScoreMX = score;
		currentScoreMX += addScoreMX / 100;
	}
}
