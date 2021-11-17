using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAsteroid : MonoBehaviour
{
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

	public void Awake()
	{
		Sprite = gameObject.transform.Find("SpriteRenderer");
		spriteRenderer = Sprite.GetComponent<SpriteRenderer>();
		edgeCollider = GetComponent<EdgeCollider2D>();
		screenWrap = GetComponent<ScreenWrap>();
		screenWrap.enabled = false;
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		totalWeight += ASs.weight;

		spriteRenderer.sprite = ASs.sprite;
		spriteRenderer.color = ASs.color;
		spriteColor = spriteRenderer.color;
		transform.localScale = ASs.scale;
		HP = ASs.HP;
		gameObject.name = ASs.name;

		if (ASs.lifetime > 0)
		{
			Destroy(gameObject, ASs.lifetime);
		}

		if (ASs.size == 0)
		{
			edgeCollider.enabled = false;
		}
	}

	public void Update()
	{
		Move(ASs.speed);

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

	private void SpawnSplits()
	{
		for (int count = ASs.splitCount; count > 0; count--)
		{
			GameObject go = Instantiate(GameManager.Instance.baseGameObject, gameObject.transform.position, gameObject.transform.rotation);
			go.GetComponent<TestAsteroid>().ASs = new AsteroidStats(GameManager.Instance.asteroids[ASs.splitIndex]);

			go.transform.position = gameObject.transform.position;
			go.transform.Rotate(0, 0, Random.Range(0f, 360f));
		}
		Destroy(gameObject);
	}

	public void ModifyHealth(float value)
	{
		HP += value;

		if (HP <= 0)
		{
			SpawnSplits();
		}
	}

	private void OnCollisionEnter2D(Collision2D Collision)
	{
		if (Collision.otherCollider)
		{
			if (Collision.gameObject.name.Contains("Player"))
			{
				GameObject player = Collision.gameObject;
				PlayerController pc = player.GetComponent<PlayerController>();
				pc.ModifyHealth(-ASs.damage);
			}
			else if (Collision.gameObject.transform.TryGetComponent(out Projectile proj))
			{
				ModifyHealth(-proj.damage);
				Destroy(proj.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		totalWeight -= ASs.weight;
	}
}
