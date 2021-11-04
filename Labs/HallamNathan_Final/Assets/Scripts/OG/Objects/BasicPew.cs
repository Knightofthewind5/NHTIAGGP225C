using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPew : MonoBehaviour
{
	public float damage = 10;
	public float lifetime = 8;
	public float projectileSpeed = 25f;
	GameObject GM;
	Manager GameManager;
	float currentLife = 0;

	private void Awake()
	{
		GM = GameObject.FindGameObjectWithTag("Manager");
		GameManager = GM.GetComponent<Manager>();
	}

	void Update()
	{
		currentLife += Time.deltaTime;

		Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
		rb.velocity = gameObject.transform.up * projectileSpeed;
		
	if (currentLife >= lifetime)
		{
			Destroy(gameObject);
			GameManager.currentMPPoints -= 1;
		}
	}

	private void OnCollisionEnter2D(Collision2D Collision)
	{
		if (Collision.gameObject.tag == "Target")
		{
			GameObject target = Collision.gameObject;
			Asteroid a = target.GetComponentInParent<Asteroid>();
			a.TakeDamage(damage);
			Destroy(gameObject);
		}
	}
}
