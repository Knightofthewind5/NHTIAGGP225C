using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
	public int[] MPPointsToNextM;
	public double[] baseMultiplierPerLevel;
	public float[] timeBtwSpawnsPerLevel;
	public int[] maxAsteroidsPerLevel;
	public double[] pointToNextLvl;

	[HideInInspector] public int level = 1;
	[HideInInspector] public double score;
	[HideInInspector] public double currentMultiplier = 1.0;
	[HideInInspector] public int currentMPPoints = 0;
	[HideInInspector] public float currentTimeBtwSpawns;
	[HideInInspector] public int maxAsteroids;
	[HideInInspector] public double currentPointsToNextLvl;
	[HideInInspector] public float currentPointsToNextMlt;
	[HideInInspector] public int currentDestroy;
	[HideInInspector] public float currentTime;
	[HideInInspector] public int currentFire;
	[HideInInspector] GameObject[] Targets;
	[HideInInspector] public int currentCount = 0;
	[HideInInspector] int currentMPTier = 1;

	public int highestLevel;
	public double highestScore;
	public double highestMultiplier;
	public double highestCurrentMultiplier;
	public int highestDestroy;
	public float highestTime;
	public int highestFire;
	public TextMeshProUGUI CurrentScoreText;
	public TextMeshProUGUI CurrentMultiplierText;
	public TextMeshProUGUI CurrentLevelText;
	public TextMeshProUGUI CurrentDestroyCount;
	public TextMeshProUGUI CurrentTime;
	public TextMeshProUGUI CurrentFire;
	public TextMeshProUGUI HighScoreText;
	public TextMeshProUGUI HighMultiplierText;
	public TextMeshProUGUI HighLevelText;
	public TextMeshProUGUI HighDestroyCount;
	public TextMeshProUGUI BestTime;
	public TextMeshProUGUI HighestFire;
	public GameObject DeathScreen;
	public GameObject HUD;
	public float deathTime = 0;

	float tempMin;
	float tempHour;
	float highTempMin;
	float highTempHour;
	public bool resetBoard = false;

	public void Awake()
	{
		deathTime = 0;
		level = 1;
		currentMultiplier = 1;
		currentMPPoints = 0;
		currentMPTier = 1;
		score = 0;
		currentDestroy = 0;
		currentTime = 0;
		currentFire = 0;

		HUD.SetActive(true);

		highestScore = PlayerPrefs.GetFloat("High Score");
		highestLevel = PlayerPrefs.GetInt("High Level");
		highestMultiplier = Mathf.Round(PlayerPrefs.GetFloat("High Multiplier") * 10f) * 0.1;
		highestDestroy = PlayerPrefs.GetInt("High Destroy");
		highestTime = PlayerPrefs.GetFloat("High Time");
		highestFire = PlayerPrefs.GetInt("High Fire");
	}

	public void Start()
	{
		level = 1;
		currentMultiplier = baseMultiplierPerLevel[0];
		currentTimeBtwSpawns = timeBtwSpawnsPerLevel[0];
		maxAsteroids = maxAsteroidsPerLevel[0];
		this.gameObject.tag = ("Manager");

		if (resetBoard == true)
		{
			ResetBoard();
			resetBoard = false;
		}
	}

	// Update is called once per frame
	public void Update()
	{
		Targets = GameObject.FindGameObjectsWithTag("Target");
		currentCount = Targets.Length;

		currentPointsToNextLvl = Mathf.Abs((float)score - (float)pointToNextLvl[level - 1]);
		currentPointsToNextMlt =  MPPointsToNextM[level - 1] - currentMPPoints;

		if (deathTime == 0)
		{
			currentTime += Time.deltaTime;
		}

		if (currentMPPoints >= MPPointsToNextM[currentMPTier - 1])
		{
			IncreaseMultiplier();
		}
		if (currentMPPoints < 0)
		{
			DecreaseMultiplier();
		}

		if (score >= pointToNextLvl[level - 1])
		{
			level++;
			
			if (currentMultiplier <= baseMultiplierPerLevel[level - 1])
			{
				currentMultiplier = baseMultiplierPerLevel[level - 1];
			}

			maxAsteroids = maxAsteroidsPerLevel[level - 1];
			currentTimeBtwSpawns = timeBtwSpawnsPerLevel[level - 1];
		}

		if (currentMultiplier > highestCurrentMultiplier)
		{
			highestCurrentMultiplier = currentMultiplier;
		}

		if (deathTime > 0)
		{
			deathTime += Time.deltaTime;

			if (currentTime > highestTime)
			{
				highestTime = currentTime;
			}

			PlayerPrefs.SetFloat("High Time", highestTime);

			while (currentTime >= 60)
			{
				tempMin++;
				currentTime -= 60;
			}

			while (tempMin >= 60)
			{
				tempHour++;
				tempMin -= 60;
			}

			while (highestTime >= 60)
			{
				highTempMin++;
				highestTime -= 60;
			}

			while (highTempMin >= 60)
			{
				highTempHour++;
				highTempMin -= 60;
			}

			float floTime = currentTime;
			floTime = Mathf.Round(floTime * 10f) * 0.1f;

			float highFloTime = highestTime;
			highFloTime = Mathf.Round(highFloTime * 10f) * 0.1f;

			CurrentTime.text = tempHour.ToString() + ":" + tempMin.ToString() + ":" + floTime.ToString();
			BestTime.text = highTempHour.ToString() + ":" + highTempMin + ":" + highFloTime.ToString();
		}

		if (deathTime > 5)
		{
			deathTime = 0;
			DeathScreen.SetActive(true);

			HUD.SetActive(false);
			UpdateBoard();
		}
	}

	public void ScoreMultiplierReset()
	{
		currentMultiplier = baseMultiplierPerLevel[level - 1];
		currentMPPoints = 0;
		currentMPTier = level;
	}

	public void GivePoints(int basePoints)
	{
		score += basePoints * currentMultiplier;
		currentMPPoints += basePoints;
	}

	public void IncreaseMultiplier()
	{
		currentMultiplier += 0.1;
		currentMPPoints = 0;
		currentMPTier += 1;
	}

	public void DecreaseMultiplier()
	{
		if (currentMPTier > 1)
		{
			currentMultiplier -= 0.1;
			currentMPPoints = MPPointsToNextM[currentMPTier - 1];
			currentMPTier -= 1;
		}
	}

	public void IncreaseDestroy()
	{
		currentDestroy++;
	}

	public void IncreaseFire()
	{
		currentFire++;
	}

	public void UpdateBoard ()
	{
		float floScore = (float)score;
		floScore = Mathf.Round(floScore * 10f) * 0.1f;

		float highFloScore = (float)highestScore;
		highFloScore = Mathf.Round(highFloScore * 10f) * 0.1f;


		if (level > highestLevel)
		{
			highestLevel = level;
		}

		if (currentMultiplier > highestMultiplier)
		{
			highestMultiplier = currentMultiplier;
		}

		if (score > highestScore)
		{
			highestScore = score;
		}

		if (currentDestroy > highestDestroy)
		{
			highestDestroy = currentDestroy;
		}

		if (currentFire > highestFire)
		{
			highestFire = currentFire;
		}

		CurrentMultiplierText.text = highestCurrentMultiplier.ToString();
		CurrentScoreText.text = floScore.ToString();
		CurrentLevelText.text = level.ToString();
		CurrentDestroyCount.text = currentDestroy.ToString();
		CurrentFire.text = currentFire.ToString();

		HighMultiplierText.text = highestMultiplier.ToString();
		HighScoreText.text = highFloScore.ToString();
		HighLevelText.text = highestLevel.ToString();
		HighDestroyCount.text = highestDestroy.ToString();
		HighestFire.text = highestFire.ToString();

		PlayerPrefs.SetFloat("High Multiplier", (float)highestMultiplier);
		PlayerPrefs.SetFloat("High Score", (float)highestScore);
		PlayerPrefs.SetInt("High Level", highestLevel);
		PlayerPrefs.SetInt("High Destroy", highestDestroy);
		PlayerPrefs.SetInt("High Fire", highestFire);
		PlayerPrefs.Save();
	}

	public void ResetBoard()
	{
		PlayerPrefs.SetFloat("High Multiplier", 1);
		PlayerPrefs.SetFloat("High Score", 0);
		PlayerPrefs.SetInt("High Level", 1);
		PlayerPrefs.SetInt("High Destroy", 0);
		PlayerPrefs.SetFloat("High Time", 0);
		PlayerPrefs.SetInt("High Fire", 0);
		PlayerPrefs.Save();

		HighMultiplierText.text = PlayerPrefs.GetFloat("High Multiplier").ToString();
		HighScoreText.text = PlayerPrefs.GetFloat("High Score").ToString();
		HighLevelText.text = PlayerPrefs.GetInt("High Level").ToString();
		HighDestroyCount.text = PlayerPrefs.GetInt("High Destroy").ToString();
		BestTime.text = PlayerPrefs.GetFloat("High Time").ToString();
		HighestFire.text = PlayerPrefs.GetFloat("High Fire").ToString();
	}
}
