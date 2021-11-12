using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

[ExecuteInEditMode]
[RequireComponent(typeof(PhotonView), typeof(Rigidbody2D), typeof(EdgeCollider2D))]
[RequireComponent(typeof(SpriteRenderer), typeof(ScreenWrap))]
public class PlayerController : MonoBehaviour
{
	PhotonView photonView;
	Rigidbody2D rb;
	EdgeCollider2D edgeCollider;

	[Tooltip("How fast will the player accelerate")]
	[SerializeField] float moveSpeed = 15f;
	[Tooltip("How fast will the player rotate")]
	[SerializeField] float rotationSpeed = 145f;
	[Tooltip("How long it takes for the player to stabilize. 0 is no stabilizing")]
	[Range(0f, 1f)]
	[SerializeField] float driftAmount = 0.15f;
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

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody2D>();
		edgeCollider = GetComponent<EdgeCollider2D>();
	}

	//Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;
	public bool canThrustBack = false;

	public ParticleSystem ps;

	private void FixedUpdate()
	{
		if (photonView.IsMine)
		{
			GetInputPlayer1();

			if (forward)
			{
				Move(1);
				ps.Play();
			}
			else
			{
				rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, driftAmount * (rb.velocity.magnitude / 10));
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
		rb.AddForce(transform.up * moveSpeed * value * Time.deltaTime, ForceMode2D.Impulse);
	}

	void Rotate(float value)
	{
		gameObject.transform.Rotate(Vector3.back * value * rotationSpeed * Time.deltaTime);
	}
}
