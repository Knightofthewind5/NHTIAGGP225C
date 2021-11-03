using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//***************************************
//Current Bugs							*
//-										*
//										*
//										*
//***************************************

public class PlayerMovement2d : MonoBehaviour
{
	//Input Variables
	public int playerId = 1;
	public float moveSpeed = 10;
	public float rotationSpeed = 120;
	public bool useGamePad = false;

	//Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;
	public bool canThrustBack = false;

	public bool isMoving = false;

	Rigidbody2D rigBod;

	public ParticleSystem ps;

	void Start()
	{
		rigBod = gameObject.GetComponent<Rigidbody2D>();
		rigBod.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void FixedUpdate()
	{
		if (playerId == 1)
		{
			GetInputPlayer1();

			if (forward)
			{
				Move(1);
				ps.Play();
			}
			else
			{
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

			if (this.GetComponent<Rigidbody2D>() != null)
			{
				if (this.GetComponent<Rigidbody2D>().velocity.magnitude > 0.01)
				{
					isMoving = true;
				}
				if (this.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.01)
				{
					isMoving = false;
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
				rigBod.AddForce(transform.up * moveSpeed * value * Time.deltaTime);
			}

			void Rotate(float value)
			{
				gameObject.transform.Rotate(Vector3.back * value * rotationSpeed * Time.deltaTime);
			}
		}
	}
}
