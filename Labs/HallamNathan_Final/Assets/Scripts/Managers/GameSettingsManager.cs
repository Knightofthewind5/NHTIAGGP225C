using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
	public static GameSettingsManager Instance { get; private set; }

	[Header("Player")]
	[Tooltip("The amount of time it takes to spawn in the players. \n Default 2")]
	public float playerSpawnWaitTime = 2f;
	[Tooltip("Multiplies the max speed of each shuttle by the given value. \n Default 1")]
	public float playerMaxSpeedMultiplier = 1f;
	[Tooltip("Multiplies the acceleration of each shuttle by the given value. \n Default 1")]
	public float playerAccelerationMultiplier = 1f;
	[Tooltip("Multiplies the rotation speed of each shuttle by the given value. \n Default 1")]
	public float playerRotationMultiplier = 1f;
	[Tooltip("Multiplies the braking power of each shuttle by the given value. \n Default 1")]
	public float playerBrakingpowerMultiplier = 1f;
	[Tooltip("Multiplies the health of each shuttle by the given value. \n Default 1")]
	public float playerHealthMultiplier = 1f;
	[Tooltip("Multiplies the grace period of each shuttle by the given value. \n Default 1")]
	public float playerGraceMultiplier = 1f;
	[Tooltip("Increases the overall amount of lives for the entire game across all players. \n Default 0")]
	public int playerLives = 0;
	[Tooltip("Shuttles will no longer take damage from any source. \n Default False")]
	public bool invulnerability = false;
	[Tooltip("Shuttles will no longer have shields to prevent damage. \n Default False")]
	public bool noShields = false;

	[Header("Asteroids")]
	[Tooltip("The amount of time it takes to spawn in the asteroids. \n Default 5")]
	public float asteroidSpawnWaitTime = 5f;
	[Tooltip("Multiplies the max speed of each asteroid by the given value. \n Default 1")]
	public float asteroidSpeedMultiplier = 1f;
	[Tooltip("Multiplies the health of each asteroid by the given value. \n Default 1")]
	public float asteroidHealthMultiplier = 1f;
	[Tooltip("Multiplies the damage dealt by each asteroid by the given value. \n Default 1")]
	public float asteroidDamageMultiplier = 1f;
	[Tooltip("Opperated with the asteroidWeightMultiplier to get the max weight of asteroids per level. \n Default 22")]
	public int baseAsteroidWeight = 22;
	[Tooltip("Opperated with the baseAsteroidWeight to get the max weight of asteroids per level. \n Default 1.25")]
	public float asteroidWeightMultiplier = 1.25f;

	[Header("Score")]
	[Tooltip("Opperated with the levelScoreMultiplier to get the max score for each level. \n Default 1")]
	public int baseScorePerLevel = 1;
	[Tooltip("Opperated with the baseScorePerLevel to get the max score for each level. \n Defualt 5.63")]
	public float levelScoreMultiplier = 5.63f;

	public bool setDefault = false;

	Dictionary<string, object> defaults = new Dictionary<string, object>();

	private void Awake()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		defaults.Add(nameof(playerSpawnWaitTime), playerSpawnWaitTime);
		defaults.Add(nameof(playerMaxSpeedMultiplier), playerMaxSpeedMultiplier);
		defaults.Add(nameof(playerAccelerationMultiplier), playerAccelerationMultiplier);
		defaults.Add(nameof(playerRotationMultiplier), playerRotationMultiplier);
		defaults.Add(nameof(playerBrakingpowerMultiplier), playerBrakingpowerMultiplier);
		defaults.Add(nameof(playerHealthMultiplier), playerHealthMultiplier);
		defaults.Add(nameof(playerGraceMultiplier), playerGraceMultiplier);
		defaults.Add(nameof(playerLives), playerLives);
		defaults.Add(nameof(invulnerability), invulnerability);
		defaults.Add(nameof(noShields), noShields);

		defaults.Add(nameof(asteroidSpawnWaitTime), asteroidSpawnWaitTime);
		defaults.Add(nameof(asteroidSpeedMultiplier), asteroidSpeedMultiplier);
		defaults.Add(nameof(asteroidHealthMultiplier), asteroidHealthMultiplier);
		defaults.Add(nameof(baseAsteroidWeight), baseAsteroidWeight);
		defaults.Add(nameof(asteroidWeightMultiplier), asteroidWeightMultiplier);

		defaults.Add(nameof(baseScorePerLevel), baseScorePerLevel);
		defaults.Add(nameof(levelScoreMultiplier), levelScoreMultiplier);
	}

	public void Update()
	{
		if (setDefault)
		{
			SetDefault();
			setDefault = false;
		}
	}

	public void SetDefault()
	{
		playerSpawnWaitTime = (float)defaults[nameof(playerSpawnWaitTime)];
		playerMaxSpeedMultiplier = (float)defaults[nameof(playerMaxSpeedMultiplier)];
		playerAccelerationMultiplier = (float)defaults[nameof(playerAccelerationMultiplier)];
		playerRotationMultiplier = (float)defaults[nameof(playerRotationMultiplier)];
		playerBrakingpowerMultiplier = (float)defaults[nameof(playerBrakingpowerMultiplier)];
		playerHealthMultiplier = (float)defaults[nameof(playerHealthMultiplier)];
		playerGraceMultiplier = (float)defaults[nameof(playerGraceMultiplier)];
		playerLives = (int)defaults[nameof(playerLives)];
		invulnerability = (bool)defaults[nameof(invulnerability)];
		noShields = (bool)defaults[nameof(noShields)];

		asteroidSpawnWaitTime = (float)defaults[nameof(asteroidSpawnWaitTime)];
		asteroidSpeedMultiplier = (float)defaults[nameof(asteroidSpeedMultiplier)];
		asteroidHealthMultiplier = (float)defaults[nameof(asteroidHealthMultiplier)];	
		baseAsteroidWeight = (int)defaults[nameof(baseAsteroidWeight)];
		asteroidWeightMultiplier = (float)defaults[nameof(asteroidWeightMultiplier)];

		baseScorePerLevel = (int)defaults[nameof(baseScorePerLevel)];
		levelScoreMultiplier = (float)defaults[nameof(levelScoreMultiplier)];
	}
}
