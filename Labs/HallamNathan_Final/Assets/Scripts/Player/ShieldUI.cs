using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
	public GameObject Destruction;

	//Image Variables
	public Image ShieldBar;
	public Image ShieldBarOutline;
	public Image ShieldRechargeBar;
	public Image ShieldRechargeOutline;

	//Shield Modifiable Variables
	public float maxShield = 10;
	public float shieldRechargeRate = 20;           //The rate at which shields are regenerated once the cooldown period is over
	public float shieldRechargeCooldownRate = 20;   //The rate at which the shield cooldown progresses
	public float beginShieldRegenAt = 100;          //The total time it takes for the shield cooldown to begin shield regeneration
	public float beginCooldownAfterBreak = 3;       //The time in seconds for how long the cooldown takes to start after breaking shields

	//Shield Bools
	bool isShieldCharged = true;     //Is the shield fully charged?
	bool isShieldBroken = false;     //Is the shield broken?
	bool isShieldRecharging = false; //Is the shield recharging?

	//Grace Period Variables
	bool inGrace = false;    //Is the player in a grace period?
	public float graceTime = 1.5f;  //The time in seconds for how long the grace periods lasts

	//Counters
	float currentShield = 0;
	float blinkInterval;			//When shields are damaged and broken, change color of bar on interval
	float shieldBrokenTime = 0;		//How long the shields are completly broken
	float shieldCurrentCharge = 0;  //The current cooldown charge


	GameObject GM;
	Manager GameManager;

	public AudioSource audioSource;
	public AudioClip PlayerDeath;
	public AudioClip ShieldDown;

	public void Awake()
	{
		currentShield = maxShield;

		GM = GameObject.FindGameObjectWithTag("Manager");
		GameManager = GM.GetComponent<Manager>();
	}

	public void FixedUpdate()
	{
		float shieldPercentage = currentShield / maxShield;
		float shieldRePercentage = shieldCurrentCharge / beginShieldRegenAt;


		ShieldBar.fillAmount = shieldPercentage;
		ShieldRechargeBar.fillAmount = shieldRePercentage;

		//Shields are charged
		if (currentShield >= maxShield)
		{
			shieldCurrentCharge = 0;
			isShieldRecharging = false;
			isShieldCharged = true;

			gameObject.GetComponentInChildren<Canvas>().enabled = false;

			currentShield = maxShield;
		}
		else
		{
			isShieldCharged = false;
		}

		//Shields are damaged
		if (currentShield < maxShield)
		{
			gameObject.GetComponentInChildren<Canvas>().enabled = true;
		}

		//Start Shield Regen Cooldown
		if (isShieldCharged == false && isShieldRecharging == false && shieldCurrentCharge < beginShieldRegenAt)
		{
			//If player is in grace
			if (isShieldBroken == true && shieldBrokenTime > beginCooldownAfterBreak)
			{
				shieldCurrentCharge += shieldRechargeCooldownRate / 2 * Time.deltaTime;
			}
			else if (isShieldBroken == false)
			{
				shieldCurrentCharge += shieldRechargeCooldownRate * Time.deltaTime;
			}
		}

		//Set Shield Regen Status
		if (shieldCurrentCharge >= beginShieldRegenAt)
		{
			isShieldRecharging = true;
		}
		else
		{
			isShieldRecharging = false;
		}

		//Start Shield Regen
		if (isShieldRecharging == true)
		{
			currentShield += shieldRechargeRate * Time.deltaTime;
			isShieldBroken = false;
			inGrace = false;
			shieldBrokenTime = 0;
		}

		//If shield is under 1/3 maximum
		if (currentShield <= maxShield / 3 && currentShield > 0)
		{
			blinkInterval += 10 * Time.deltaTime;

			if (blinkInterval >= 2)
			{
				ShieldBarOutline.color = new Color(1, 0, 0, 1);
				if (blinkInterval >= 3)
				{
					blinkInterval = 0;
				}
			}
			else
			{
				ShieldBarOutline.color = new Color(1, 1, 1, 0.5f);
			}
		}
		else
		{
			ShieldBarOutline.color = new Color(1, 1, 1, 0.5f);
		}

		//If shields are empty
		if (currentShield <= 0f)
		{
			blinkInterval += 15 * Time.deltaTime;
			shieldBrokenTime += Time.deltaTime;

			if (shieldBrokenTime <= 0.5)
			{
				isShieldBroken = true;
				inGrace = true;
				currentShield = 0;
			}

			if (blinkInterval >= 2)
			{
				ShieldBarOutline.color = new Color(1, 0, 0, 1);
				if (blinkInterval >= 3)
				{
					blinkInterval = 0;
					audioSource.PlayOneShot(ShieldDown);
				}
			}
			else
			{
				ShieldBarOutline.color = new Color(1f, 0.75f, 0.75f, 0.5f);
			}
		}

		if (shieldBrokenTime >= graceTime)
		{
			inGrace = false;
		}

		//If in grace period, turn off collider
		if (inGrace == true)
		{
			gameObject.GetComponentInParent<BoxCollider2D>().enabled = false;
			gameObject.GetComponentInParent<SpriteRenderer>().color = new Color (1f,0f,1f,0.75f);
		}
		else
		{
			gameObject.GetComponentInParent<BoxCollider2D>().enabled = true;
			gameObject.GetComponentInParent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
		}
	}

	public void TakeDamage(int damage)
	{
		currentShield -= damage;
		shieldCurrentCharge = 0;
		GameManager.ScoreMultiplierReset();

		if (isShieldBroken == true && currentShield < 0)
		{
			Debug.LogWarning("Player Dead");
			Death();
		}
	}

	public void Death()
	{
		audioSource.PlayOneShot(PlayerDeath);

		for (int count = 10; count > 0; count--)
		{
			GameObject ds = Instantiate(Destruction, gameObject.transform.position, gameObject.transform.rotation);

			ds.transform.position = gameObject.transform.position;
			ds.transform.Rotate(0, 0, UnityEngine.Random.Range(0f, 360f));
		}

		GameManager.deathTime = 0.1f;

		Destroy(gameObject);
	}
}
