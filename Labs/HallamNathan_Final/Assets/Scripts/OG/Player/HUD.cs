using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
	public Image MultiplierBar;
	public Image LevelBar;
	public TextMeshProUGUI MultiplierText;
	public TextMeshProUGUI ScoreText;
	public TextMeshProUGUI LevelText;

	GameObject GM;
	Manager GameManager;

	double levelPercentage;
	float multipPercentage;

	public void Awake()
	{
		GM = GameObject.FindGameObjectWithTag("Manager");
		GameManager = GM.GetComponent<Manager>();
	}

	public void Update()
	{
		SetField();
	}

	public void FixedUpdate()
	{
		multipPercentage = (GameManager.MPPointsToNextM[GameManager.level - 1] - GameManager.currentPointsToNextMlt) / GameManager.MPPointsToNextM[GameManager.level - 1];

		if (GameManager.level == 1)
		{
			levelPercentage = (GameManager.pointToNextLvl[GameManager.level - 1] - GameManager.currentPointsToNextLvl) / GameManager.pointToNextLvl[GameManager.level - 1];
		}
		else
		{
			levelPercentage = ((GameManager.pointToNextLvl[GameManager.level - 1] - GameManager.pointToNextLvl[GameManager.level - 2]) - GameManager.currentPointsToNextLvl) / (GameManager.pointToNextLvl[GameManager.level - 1] - GameManager.pointToNextLvl[GameManager.level - 2]);
		}

		MultiplierBar.fillAmount = multipPercentage;
		LevelBar.fillAmount = (float)levelPercentage;
	}

	public void SetField()
	{
		float floScore = (float)GameManager.score;
		floScore = Mathf.Round(floScore * 10f) * 0.1f;

		MultiplierText.text = GameManager.currentMultiplier.ToString();
		ScoreText.text = floScore.ToString();
		LevelText.text = GameManager.level.ToString();
	}
}

