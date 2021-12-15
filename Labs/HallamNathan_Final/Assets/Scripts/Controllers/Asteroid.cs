using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Asteroid : MonoBehaviour
{
	private const int UPDATE_HEALTH = 1;
	public string ID;

	public static int totalWeight; // The maximum weight of asteroids currently in the level

	SpriteRenderer spriteRenderer;
	ScreenWrap screenWrap;
	Rigidbody2D rb;
	Transform Sprite;
	EdgeCollider2D edgeCollider;
	public bool offScreen = true;

	public AsteroidStats ASs;
	float HP;
	Color spriteColor;
	float randomRotationValue;

	public AudioClip deathSound;
	public void Awake()
	{
		Sprite = gameObject.transform.Find("SpriteRenderer");
		spriteRenderer = Sprite.GetComponent<SpriteRenderer>();
		edgeCollider = GetComponent<EdgeCollider2D>();
		screenWrap = GetComponent<ScreenWrap>();
		screenWrap.enabled = false;
		rb = GetComponent<Rigidbody2D>();

		randomRotationValue = Random.Range(-179, 179);
	}

	private void Start()
	{
		totalWeight += ASs.weight;

		spriteRenderer.sprite = ASs.sprite;
		spriteRenderer.color = ASs.color;
		spriteColor = spriteRenderer.color;
		transform.localScale = ASs.scale;
		HP = ASs.HP;
		HP *= GameSettingsManager.Instance.asteroidHealthMultiplier;
		gameObject.name = ASs.name;

		if (ASs.lifetime > 0)
		{
			Destroy(gameObject, ASs.lifetime);
		}

		if (ASs.size == 0)
		{
			edgeCollider.enabled = false;
		}

		PhotonNetwork.NetworkingClient.EventReceived += this.UpdateAsteroidHealth;
	}

	public void Update()
	{
		Move(ASs.speed * GameSettingsManager.Instance.asteroidSpeedMultiplier);
		SpriteRotate();

		Debug.DrawLine(transform.position, transform.position + (transform.up * 2), new Color(0, 0, 255, 255), 0f, false);
		if (Camera.main.ObjectIsVisible(gameObject) && offScreen)
		{
			offScreen = false;

			StartCoroutine(OnScreen());
		}

		if (ASs.size == 0)
		{
			ASs.lifetime -= Time.deltaTime;

			spriteColor.a = Mathf.Lerp(0, 1, ASs.lifetime);

			spriteRenderer.color = spriteColor;
		}
	}

	private void UpdateAsteroidHealth(EventData obj)
	{
		if (obj.Code == UPDATE_HEALTH)
		{
			object[] datas = (object[])obj.CustomData;

			if (datas[0].ToString() == ID)
			{
				HP = (float)datas[1];

				if (HP <= 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}

	IEnumerator OnScreen()
	{
		yield return new WaitForSecondsRealtime(ASs.size);

		Sprite.gameObject.layer = 6;
		screenWrap.enabled = true;
	}

	void Move(float value)
	{
		if (ASs.size >= 1)
		{
			rb.velocity = gameObject.transform.up * value * 250 * Time.deltaTime;
		}
		else
		{
			rb.velocity = gameObject.transform.up * value * 250 * Time.deltaTime;
		}
	}

	void SpriteRotate()
	{
		if (ASs.size >= 1)
		{
			spriteRenderer.transform.Rotate(Vector3.back * (randomRotationValue / (ASs.size * 2)) * Time.deltaTime);
		}
		else
		{
			spriteRenderer.transform.Rotate(Vector3.back * randomRotationValue * Time.deltaTime);
		}
	}

	private void SpawnSplits()
	{
		for (int count = ASs.splitCount; count > 0; count--)
		{
			float rotation = Random.Range(0f, 360f);

			RPCManager.Instance.PV.RPC("SpawnAsteroidsRPC", RpcTarget.All, ASs.splitIndex, gameObject.transform.position, rotation, Random.Range(100000, 999999));
		}

		gameObject.name = gameObject.GetInstanceID().ToString();
		RPCManager.Instance.PV.RPC("DestroyGameObject", RpcTarget.All, gameObject.GetInstanceID());
	}

	public void ModifyHealth(float value)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			HP += value;

			if (HP <= 0)
			{
				SpawnSplits();
			}
			object[] datas = new object[] { ID, HP };

			RaiseEventOptions REO = new RaiseEventOptions { Receivers = ReceiverGroup.All };

			PhotonNetwork.RaiseEvent(UPDATE_HEALTH, datas, REO, SendOptions.SendReliable);
		}
	}

	private void OnCollisionEnter2D(Collision2D Collision)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (Collision.otherCollider)
			{
				if (Collision.gameObject.transform.TryGetComponent(out Projectile proj))
				{
					ModifyHealth(-proj.HP - proj.damage);
				}
			}
		}	
		
		if (Collision.otherCollider)
		{
			if (Collision.gameObject.transform.TryGetComponent(out PlayerController PC))
			{
				if (PC.photonView.IsMine)
				{
					PC.ModifyHealth(-ASs.damage * GameSettingsManager.Instance.asteroidDamageMultiplier);
				}
			}
		}
	}

	private void OnDestroy()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.UpdateAsteroidHealth;

		if (PhotonNetwork.IsMasterClient && ASs.weight > 0)
		{
			if (Camera.main)
			{
				Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(deathSound);
			}

			totalWeight -= ASs.weight;
			GameManager.Instance.PV.RPC("UpdateInfoBarValuesRPC", RpcTarget.AllBuffered, ASs.score);
		}
	}
}
