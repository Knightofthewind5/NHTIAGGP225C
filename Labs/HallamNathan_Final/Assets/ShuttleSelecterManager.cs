using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ShuttleSelecterManager : MonoBehaviour
{
	public GameObject Content;
	public GameObject Prefab;
	public ShuttleInfo shuttleStats;
	public TMP_Text currentShuttleText;
	public Image currentShuttleImage;
	public CanvasGroup ShuttleSelectGroup;
	public CanvasGroup ShuttleStatsCanvasGroup;
	public CanvasGroup ShuttleStatInfoCanvasGroup;
	public CanvasGroup GameSettingsCanvasGroup;
	public CanvasGroup ChatCanvasGroup;
	public Button SelectShuttleButton;

	public TMP_Text shuttleNameText;
	public TMP_Text maxSpeedText;
	public TMP_Text accelerationText;
	public TMP_Text rotationSpeedText;
	public TMP_Text brakingPowerText;
	public TMP_Text maxShieldText;
	public TMP_Text shieldRechargeRateText;
	public TMP_Text shieldRechargeWaitText;
	public TMP_Text shieldRegenRateText;
	public TMP_Text shieldRegenPauseText;
	public TMP_Text shieldRegenWaitText;
	public TMP_Text gracePeriodText;

	public TMP_Text statDescriptionText;

	void Start()
	{
		foreach(var shuttle in shuttleStats.shuttles)
		{
			GameObject shuttlePrefab = Instantiate(Prefab, Content.transform);
			shuttlePrefab.GetComponent<Image>().sprite = shuttle.shuttleSprite;
			shuttlePrefab.GetComponent<Button>().onClick.AddListener(() => ShowShuttleStats(shuttle.shuttleName));
		}
	}

	public void ToggleShuttleSelectPanel()
	{
		if (ShuttleSelectGroup.activeInHierarchy())
		{
			ShuttleSelectGroup.SetActive(false);
			ShuttleStatsCanvasGroup.SetActive(false);
			ShuttleStatInfoCanvasGroup.SetActive(false);
			ChatCanvasGroup.SetActive(true);
		}
		else
		{
			ChatCanvasGroup.SetActive(false);
			GameSettingsCanvasGroup.SetActive(false);
			ShuttleSelectGroup.SetActive(true);
		}
	}

	public void ShowShuttleStats(string name)
	{
		foreach (var shuttle in shuttleStats.shuttles)
		{
			if (shuttle.shuttleName == name)
			{
				ShuttleStatsCanvasGroup.SetActive(true);
				SelectShuttleButton.onClick.AddListener(() => SelectShuttle(shuttle.shuttleName));
				SelectShuttleButton.onClick.AddListener(() => ShuttleStatInfoCanvasGroup.SetActive(false));

				shuttleNameText.text = "Shuttle: " + shuttle.shuttleName;
				maxSpeedText.text = shuttle.maxSpeed.ToString();
				accelerationText.text = shuttle.acceleration.ToString();
				rotationSpeedText.text = shuttle.rotationSpeed.ToString();
				brakingPowerText.text = shuttle.brakingPower.ToString();
				maxShieldText.text = shuttle.maxShieldStrength.ToString();
				shieldRechargeRateText.text = shuttle.shieldRechargeRate.ToString();
				shieldRechargeWaitText.text = shuttle.shieldRechargeWait.ToString();
				shieldRegenRateText.text = shuttle.shieldRegenRate.ToString();
				shieldRegenPauseText.text = shuttle.shieldRegenPause.ToString();
				shieldRegenWaitText.text = shuttle.shieldRegenWait.ToString();
				gracePeriodText.text = shuttle.gracePeriod.ToString();
			}
		}
	}

	public void SelectShuttle(string name)
	{
		foreach (var shuttle in shuttleStats.shuttles)
		{
			if (shuttle.shuttleName == name)
			{
				currentShuttleText.text = name;
				currentShuttleImage.sprite = shuttle.shuttleSprite;
				ShuttleSelectGroup.SetActive(false);
				ShuttleStatsCanvasGroup.SetActive(false);
				ChatCanvasGroup.SetActive(true);

				foreach (var listing in GameLobbyManager.Instance.lobbyListing)
				{
					if (listing.username == PhotonNetwork.LocalPlayer.NickName)
					{
						listing.Player.CustomProperties["shuttleName"] = name;
					}
				}
			}
		}
	}

	public void UpdateStatDescriptionText(string text)
	{
		statDescriptionText.text = text;
	}
}
