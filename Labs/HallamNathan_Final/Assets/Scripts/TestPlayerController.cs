using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
	Rigidbody2D rb;
	EdgeCollider2D edgeCollider;

	[Tooltip("How fast the player is capable of moving")]
	[SerializeField] float maxSpeed = 10f;
	[Tooltip("How fast the player will accelerate")]
	[SerializeField] float acceleration = 10f;
	[Tooltip("How fast the player will rotate")]
	[SerializeField] float rotationSpeed = 145f;
	[Tooltip("How long it takes for the player to stop moving when not thrusting. 0 is no stopping.")]
	[Range(0f, 1f)]
	[SerializeField] float brakingPower = 0.15f;
	[Tooltip("The maximum HP the shields have")]
	[SerializeField] float maxShieldStrength = 10f;
	[Tooltip("How fast do the shields regenerate after taking damage")]
	[SerializeField] float shieldRegenRate = 10f;
	[Tooltip("How fast do the shields recharge after breaking")]
	[SerializeField] float shieldRechargeRate = 20f;
	[Tooltip("How long it takes for the shields to start regenerating")]
	[SerializeField] float shieldRegenWait = 5f;
	[Tooltip("How long it takes for the shields to start recharging")]
	[SerializeField] float shieldRechargeWait = 100f;
	[Tooltip("How many seconds of invulnerability does the player have when their sheilds break")]
	[SerializeField] float gracePeriod = 1.5f;

	ParticleSystem ps;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		edgeCollider = GetComponent<EdgeCollider2D>();
		ps = GetComponent<ParticleSystem>();
	}

	//Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;
	public bool canThrustBack = false;

	private void FixedUpdate()
	{
		GetInputPlayer1();

		if (forward && rb.velocity.magnitude < maxSpeed)
		{
			Move(1);
			ps.Play();
		}
		else
		{
			rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, brakingPower * (rb.velocity.magnitude / 10));

			ps.Stop();
		}
		if (backward)
		{
			if (canThrustBack == true)
			{
				Move(-1);
			}
		}
		if (right)
		{
			Rotate(1);
		}
		if (left)
		{
			Rotate(-1);
		}		
	}

	void GetInputPlayer1()
	{
		forward = Input.GetKey(KeyCode.W);
		backward = Input.GetKey(KeyCode.S);
		left = Input.GetKey(KeyCode.A);
		right = Input.GetKey(KeyCode.D);
	}

	void Move(float value)
	{
		// Apply force behind ship to propel forward
		rb.AddForce(transform.up * acceleration * value * Time.deltaTime);
	}

	void Rotate(float value)
	{
		gameObject.transform.Rotate(Vector3.back * value * rotationSpeed * Time.deltaTime);
	}
}
