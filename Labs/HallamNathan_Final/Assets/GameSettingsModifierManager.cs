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
	}

	public void SaveSettings()
	{
		if (playerSpawnWait.text.Length > 0) GameSettingsManager.Instance.playerSpawnWaitTime = float.Parse(playerSpawnWait.text);

		GameSettingsManager.Instance.playerMaxSpeedMultiplier = float.Parse(playerMaxSpeedMX.text);
		GameSettingsManager.Instance.playerAccelerationMultiplier = float.Parse(playerAccelerationMX.text);
		GameSettingsManager.Instance.playerRotationMultiplier = float.Parse(playerRotationMX.text);
		GameSettingsManager.Instance.playerBrakingpowerMultiplier = float.Parse(playerBrakingPowerMX.text);
		GameSettingsManager.Instance.playerHealthMultiplier = float.Parse(playerHealthMX.text);
		GameSettingsManager.Instance.playerGraceMultiplier = float.Parse(playerGraceMX.text);
		GameSettingsManager.Instance.invulnerability = Convert.ToBoolean(playerInvulnerability.value);
		GameSettingsManager.Instance.noShields = Convert.ToBoolean(playerNoShields.value);
		GameSettingsManager.Instance.playerLives = int.Parse(playerLives.text);
	}

	public void DefaultSettings()
	{
		GameSettingsManager.Instance.SetDefault();
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
