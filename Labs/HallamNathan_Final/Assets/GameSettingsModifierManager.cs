using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameSettingsModifierManager : MonoBehaviour
{
	CanvasGroup GameSettingsCanvasGroup;
	public CanvasGroup ChatCanvasGroup;
	public CanvasGroup ShuttleSelectGroup;
	public CanvasGroup ShuttleStatsCanvasGroup;
	public CanvasGroup ShuttleStatInfoCanvasGroup;

	public TMP_InputField playerSpawnWait;
	public TMP_InputField playerMaxSpeedMX;
	public TMP_InputField playerAccelerationMX;
	public TMP_InputField playerRotationMX;
	public TMP_InputField playerBrakingPowerMX;
	public TMP_InputField playerHealthMX;
	public TMP_InputField playerGraceMX;
	public TMP_InputField playerLives;
	public TMP_Dropdown playerInvulnerability;
	public TMP_Dropdown playerNoShields;

	public TMP_InputField asteroidSpawnWait;
	public TMP_InputField asteroidSpeedMX;
	public TMP_InputField asteroidHealthMX;
	public TMP_InputField asteroidDamageMX;
	public TMP_InputField asteroidBaseWeight;
	public TMP_InputField asteroidWeightMX;

	public TMP_InputField scoreBasePerLevel;
	public TMP_InputField scoreLevelMX;

	private void Awake()
	{
		GameSettingsCanvasGroup = GetComponent<CanvasGroup>();
	}

	private void Start()
	{
		SetDefaultSettingsInfo();
	}

	public void SaveSettings()
	{
		if (playerSpawnWait.text.Length > 0) GameSettingsManager.Instance.playerSpawnWaitTime = float.Parse(playerSpawnWait.text);
		if (playerMaxSpeedMX.text.Length > 0) GameSettingsManager.Instance.playerMaxSpeedMultiplier = float.Parse(playerMaxSpeedMX.text);
		if (playerAccelerationMX.text.Length > 0) GameSettingsManager.Instance.playerAccelerationMultiplier = float.Parse(playerAccelerationMX.text);
		if (playerRotationMX.text.Length > 0) GameSettingsManager.Instance.playerRotationMultiplier = float.Parse(playerRotationMX.text);
		if (playerBrakingPowerMX.text.Length > 0) GameSettingsManager.Instance.playerBrakingpowerMultiplier = float.Parse(playerBrakingPowerMX.text);
		if (playerHealthMX.text.Length > 0) GameSettingsManager.Instance.playerHealthMultiplier = float.Parse(playerHealthMX.text);
		if (playerGraceMX.text.Length > 0) GameSettingsManager.Instance.playerGraceMultiplier = float.Parse(playerGraceMX.text);
		if (playerLives.text.Length > 0) GameSettingsManager.Instance.playerLives = int.Parse(playerLives.text);
		GameSettingsManager.Instance.invulnerability = Convert.ToBoolean(playerInvulnerability.value);
		GameSettingsManager.Instance.noShields = Convert.ToBoolean(playerNoShields.value);

		if (asteroidSpawnWait.text.Length > 0) GameSettingsManager.Instance.asteroidSpawnWaitTime = float.Parse(asteroidSpawnWait.text);
		if (asteroidSpeedMX.text.Length > 0) GameSettingsManager.Instance.asteroidSpeedMultiplier = float.Parse(asteroidSpeedMX.text);
		if (asteroidHealthMX.text.Length > 0) GameSettingsManager.Instance.asteroidHealthMultiplier = float.Parse(asteroidHealthMX.text);
		if (asteroidDamageMX.text.Length > 0) GameSettingsManager.Instance.asteroidDamageMultiplier = float.Parse(asteroidDamageMX.text);
		if (asteroidBaseWeight.text.Length > 0) GameSettingsManager.Instance.baseAsteroidWeight = int.Parse(asteroidBaseWeight.text);
		if (asteroidWeightMX.text.Length > 0) GameSettingsManager.Instance.asteroidWeightMultiplier = float.Parse(asteroidWeightMX.text);

		if (scoreBasePerLevel.text.Length > 0) GameSettingsManager.Instance.baseScorePerLevel = int.Parse(scoreBasePerLevel.text);
		if (scoreLevelMX.text.Length > 0) GameSettingsManager.Instance.levelScoreMultiplier = float.Parse(scoreLevelMX.text);
	}

	public void DefaultSettings()
	{
		GameSettingsManager.Instance.SetDefault();

		SetDefaultSettingsInfo();
	}

	public void RetroSettings()
	{
		playerSpawnWait.text = GameSettingsManager.Instance.playerSpawnWaitTime.ToString();
		playerMaxSpeedMX.text = GameSettingsManager.Instance.playerMaxSpeedMultiplier.ToString();
		playerAccelerationMX.text = GameSettingsManager.Instance.playerAccelerationMultiplier.ToString();
		playerRotationMX.text = GameSettingsManager.Instance.playerRotationMultiplier.ToString();
		playerBrakingPowerMX.text = "0";
		playerHealthMX.text = "0";
		playerGraceMX.text = "0";
		playerLives.text = "3";
		playerInvulnerability.value = 0;
		playerNoShields.value = 1;

		asteroidSpawnWait.text = GameSettingsManager.Instance.asteroidSpawnWaitTime.ToString();
		asteroidSpeedMX.text ="2";
		asteroidHealthMX.text = "0.1";
		asteroidDamageMX.text = GameSettingsManager.Instance.asteroidDamageMultiplier.ToString();
		asteroidBaseWeight.text = GameSettingsManager.Instance.baseAsteroidWeight.ToString();
		asteroidWeightMX.text = GameSettingsManager.Instance.asteroidWeightMultiplier.ToString();

		scoreBasePerLevel.text = GameSettingsManager.Instance.baseScorePerLevel.ToString();
		scoreLevelMX.text = GameSettingsManager.Instance.levelScoreMultiplier.ToString();
	}

	public void NoLimitsSettings()
	{
		playerSpawnWait.text = "0.1";
		playerMaxSpeedMX.text = "10";
		playerAccelerationMX.text = "1.5";
		playerRotationMX.text = "1.25";
		playerBrakingPowerMX.text = "1";
		playerHealthMX.text = "1.5";
		playerGraceMX.text = "1";
		playerLives.text = "9";
		playerInvulnerability.value = 0;
		playerNoShields.value = 0;

		asteroidSpawnWait.text = GameSettingsManager.Instance.asteroidSpawnWaitTime.ToString();
		asteroidSpeedMX.text = "0.5";
		asteroidHealthMX.text = "0.1";
		asteroidDamageMX.text = GameSettingsManager.Instance.asteroidDamageMultiplier.ToString();
		asteroidBaseWeight.text = GameSettingsManager.Instance.baseAsteroidWeight.ToString();
		asteroidWeightMX.text = GameSettingsManager.Instance.asteroidWeightMultiplier.ToString();

		scoreBasePerLevel.text = GameSettingsManager.Instance.baseScorePerLevel.ToString();
		scoreLevelMX.text = "2.5";
	}

	public void SetDefaultSettingsInfo()
	{
		playerSpawnWait.text = GameSettingsManager.Instance.playerSpawnWaitTime.ToString();
		playerMaxSpeedMX.text = GameSettingsManager.Instance.playerMaxSpeedMultiplier.ToString();
		playerAccelerationMX.text = GameSettingsManager.Instance.playerAccelerationMultiplier.ToString();
		playerRotationMX.text = GameSettingsManager.Instance.playerRotationMultiplier.ToString();
		playerBrakingPowerMX.text = GameSettingsManager.Instance.playerBrakingpowerMultiplier.ToString();
		playerHealthMX.text = GameSettingsManager.Instance.playerHealthMultiplier.ToString();
		playerGraceMX.text = GameSettingsManager.Instance.playerGraceMultiplier.ToString();
		playerLives.text = GameSettingsManager.Instance.playerLives.ToString();
		playerInvulnerability.value = Convert.ToInt32(GameSettingsManager.Instance.invulnerability);
		playerNoShields.value = Convert.ToInt32(GameSettingsManager.Instance.noShields);

		asteroidSpawnWait.text = GameSettingsManager.Instance.asteroidSpawnWaitTime.ToString();
		asteroidSpeedMX.text = GameSettingsManager.Instance.asteroidSpeedMultiplier.ToString();
		asteroidHealthMX.text = GameSettingsManager.Instance.asteroidHealthMultiplier.ToString();
		asteroidDamageMX.text = GameSettingsManager.Instance.asteroidDamageMultiplier.ToString();
		asteroidBaseWeight.text = GameSettingsManager.Instance.baseAsteroidWeight.ToString();
		asteroidWeightMX.text = GameSettingsManager.Instance.asteroidWeightMultiplier.ToString();

		scoreBasePerLevel.text = GameSettingsManager.Instance.baseScorePerLevel.ToString();
		scoreLevelMX.text = GameSettingsManager.Instance.levelScoreMultiplier.ToString();
	}

	public void ToggleGameSettingsCanvasGroup()
	{
		if (GameSettingsCanvasGroup.activeInHierarchy())
		{
			GameSettingsCanvasGroup.SetActive(false);
			ChatCanvasGroup.SetActive(true);
		}
		else
		{
			ChatCanvasGroup.SetActive(false);
			ShuttleSelectGroup.SetActive(false);
			ShuttleStatInfoCanvasGroup.SetActive(false);
			ShuttleStatsCanvasGroup.SetActive(false);
			GameSettingsCanvasGroup.SetActive(true);
		}
	}
}
