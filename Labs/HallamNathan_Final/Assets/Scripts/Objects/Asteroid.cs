/************************************************************
 *  Bug List												*
 *															*
 *															*
************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
	public int size = 1;
	public float maxHP = 1;
	public float currentHP = 0;
	public int baseSpeedModifier = 10;
	public int damage = 5;
	public float lifetime = 0;

	public int splitCount = 3;

	public int pointsGivenOnDistruction = 1;

	int randomRotationValue;

	public GameObject SplitAsteroid;

	AudioSource soundEffect;
	public AudioClip soundHit;
	public AudioClip soundExplosion;

	public Transform spriteRotate;

	Rigidbody2D rigBod;

	public bool forceDestroy = false;
	public bool liftimeDestroy = false;

	GameObject MC;
	GameObject GM;
	Manager GameManager;

	public void Awake()
	{
		currentHP = maxHP;

		GM = GameObject.FindGameObjectWithTag("Manager");
		GameManager = GM.GetComponent<Manager>();
		MC = GameObject.FindGameObjectWithTag("MainCamera");
		soundEffect = MC.GetComponent<AudioSource>();
	}

	private void Start()
	{
		rigBod = gameObject.GetComponent<Rigidbody2D>();

		randomRotationValue = Random.Range(-150, 150);
	}

	public void Update()
	{
		if (size == 0)
		{
			Destroy(gameObject, lifetime);
		}
		else if (currentHP <= 0)
		{
			DestroyAsteroid();
		}
		else if (size >= 1 && liftimeDestroy == true)
		{
			Destroy(gameObject, lifetime);
		}
	}

	private void FixedUpdate()
	{
		Asteroid a = GetComponent<Asteroid>();

		if (size >= 1)
		{
			a.Move(baseSpeedModifier / size);
		}
		else
		{
			a.Move(baseSpeedModifier);
		}

		if (forceDestroy)
		{
			DestroyAsteroid();
		}

		SpriteRotate();
	}

	void SpriteRotate ()
	{
		if (size >= 1)
		{
			spriteRotate.transform.Rotate(Vector3.back * (randomRotationValue / (size * 2)) * Time.deltaTime);
		}
		else 
		{
		spriteRotate.transform.Rotate(Vector3.back * randomRotationValue * Time.deltaTime);
		}

	}

	void Move(float value)
	{
		if (size >= 1)
		{
			rigBod.velocity = gameObject.transform.up * value * (250 / size) * Time.deltaTime;
		}
		else
		{
			rigBod.velocity = gameObject.transform.up * value * 250 * Time.deltaTime;
		}
	}

	private void DestroyAsteroid()
	{
		soundEffect.PlayOneShot(soundExplosion);

		if (size == 1)
		{
			GameManager.IncreaseDestroy();
		}

		if (size >= 1)
		{
			SpawnSplits();
		}

		Destroy(gameObject);
		GameManager.GivePoints(pointsGivenOnDistruction);
	}

	private void SpawnSplits()
	{
		for (int count = splitCount; count > 0; count--)
		{
			GameObject go = Instantiate(SplitAsteroid, gameObject.transform.position, gameObject.transform.rotation);

			go.transform.position = gameObject.transform.position;
			go.transform.Rotate(0, 0, Random.Range(0f, 360f));
		}
		Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D Collision)
	{
		if (Collision.otherCollider)
		{
			if (Collision.gameObject.name == "Player")
			{
				GameObject player = Collision.gameObject;
				ShieldUI pm = player.GetComponentInParent<ShieldUI>();
				pm.TakeDamage(damage);
			}
			else if (Collision.gameObject.GetComponentInParent<BasicPew>().damage > 0)
			{
				TakeDamage(Collision.gameObject.GetComponentInParent<BasicPew>().damage);
			}
		}
	}

	public void TakeDamage(float playerDamage)
	{
		currentHP -= playerDamage;
	}
}

