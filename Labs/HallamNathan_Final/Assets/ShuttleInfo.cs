using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ShuttleStats", menuName = "ScriptableObjects/ShuttleStats")]
public class ShuttleInfo : ScriptableObject
{
	public List<ShuttleStats> shuttles = new List<ShuttleStats>();

	public void CopyInfo()
	{

	}
}

[System.Serializable]
public class ShuttleStats
{
	public string shuttleName;
	public Sprite shuttleSprite;
	public GameObject shuttlePrefab;

	[Header("Movement")]
	[Tooltip("How fast the player is capable of moving")]
	public float maxSpeed = 71f;
	[Tooltip("How fast the player will accelerate")]
	public float acceleration = 10f;
	[Tooltip("How fast the player will rotate")]
	public float rotationSpeed = 145f;
	[Tooltip("How long it takes for the player to stop moving when not thrusting. 0 is no stopping.")]
	[Range(0f, 1f)]
	public float brakingPower = 0.15f;

	// Recharging - The process of a shield filling broken portions of its array when damaged
	// Regenerate - The process of a shield rebuilding the entire shield array when broken
	[Header("Shields")]
	[Tooltip("The maximum HP the shields have")]
	public float maxShieldStrength = 10f;

	[Tooltip("How many points does the shields regenerate per second when damaged")]
	public float shieldRechargeRate = 10f;

	[Tooltip("How long in seconds the player has to wait for shields to begin recharge")]
	public float shieldRechargeWait = 3f;

	[Tooltip("How many points does the shields regenerate per second when broken")]
	public float shieldRegenRate = 2f;

	[Tooltip("How long in seconds it takes for the shields regeneration timer to start after the shields are broken")]
	public float shieldRegenPause = 5f;

	[Tooltip("The total time in seconds the player has to wait for shields to begin regenerate")]
	public float shieldRegenWait = 10f;

	[Tooltip("How many seconds of invulnerability does the player have when their sheilds break")]
	public float gracePeriod = 1.5f;

	public void CopyStats()
	{

	}
}
