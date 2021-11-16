using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAsteroid : MonoBehaviour
{
	public static int totalWeight; // The maximum weight of asteroids currently in the level
	[Tooltip("The designated weight of the asteroid. Dictates how many asteroids can be in the level at once with larger asteroids weighing more")]
	public int weight = 1;
	[Tooltip("The designated size of the asteroid. Dictates the movement and rotation speeds")]
	public int size = 1;

	SpriteRenderer spriteRenderer;
	ScreenWrap screenWrap;
	Rigidbody2D rb;
	Transform Sprite;
	public bool offScreen = true;

	[Tooltip("A modifier to change the speed of an asteroid sperate from its given size")]
	public int baseSpeedModifier = 10;

	public void Awake()
	{
		weight = Random.Range(1, 11);
		totalWeight += weight;
		Sprite = gameObject.transform.Find("SpriteRenderer");
		spriteRenderer = Sprite.GetComponent<SpriteRenderer>();
		screenWrap = GetComponent<ScreenWrap>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		rb.velocity = gameObject.transform.up * baseSpeedModifier * (250 / size) * Time.deltaTime;
	}

	public void Update()
	{
		Debug.DrawLine(transform.position, transform.position + (transform.up * 2), new Color(0, 0, 255, 255), 0f, false);
		if (Camera.main.ObjectIsVisible(gameObject) && offScreen)
		{
			offScreen = false;

			StartCoroutine(OnScreen());
		}
	}

	IEnumerator OnScreen()
	{
		yield return new WaitForSecondsRealtime(1);

		Sprite.gameObject.layer = 8;
		screenWrap.enabled = true;
	}

	private void OnDestroy()
	{
		totalWeight -= weight;
	}
}
