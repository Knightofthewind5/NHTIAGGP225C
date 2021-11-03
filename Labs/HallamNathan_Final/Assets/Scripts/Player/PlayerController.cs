using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	//Input Variables
	public int playerId = 1;
	public float moveSpeed = 20;
	public float rotationSpeed = 145;
	public bool useGamePad = false;

	//Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;

	public bool isMoving = false;

	Rigidbody2D rigBod;

	//Fire Variables
	bool fireOneIsEnabled = true;
	double fireOneCooldown = 0;
	public bool fireOneOnCooldown = false;
	public double fireOneCooldownTime = 6;  //in Seconds.
	public double cooldownOneSpeed = 1; //For Fire1 - 1 is normal speed : 2 is 2X faster : 0.5 is 2X slower. If there were to be powerups - this would change the "reload" speed.
	bool fire1 = false;

	//Projectile Instantiate Variables
	public GameObject PewPrefab;
	public GameObject SpawnLocation;

	public AudioSource soundEffect;
	public AudioClip soundFire1;

	public GameObject Player;

	[HideInInspector]
	public int toolbarTop;
	public int toolbarBottom;
	public string currentTab;

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
			}
			if (backward)
			{
				Move(-1);
			}
			if (right)
			{
				Rotate(1);
			}
			if (left)
			{
				Rotate(-1);
			}
			if (fire1)
			{
				Fire1();
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
				fire1 = Input.GetKey(KeyCode.Space);
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
	private void Update()   //Update fire cooldown time and availability.
	{
		//Fire 1
		if (fireOneCooldown > 0)    //If ability cooldown is over 0 seconds.
		{
			fireOneCooldown -= cooldownOneSpeed * Time.deltaTime;   //Countdown cooldown.
			fireOneOnCooldown = true;   //Disable Fire.
		}
		else //If ability is not on cooldown.
		{
			fireOneOnCooldown = false;  //Enable fire.
			fireOneCooldown = 0;    //Set cooldown to zero.
		}
	}

	private void Fire1() //Checks if player has used ability and if fire1 is on cooldown, if available then call FireProjectile.
	{
		if (fireOneIsEnabled)   //If the player can fire
		{
			if (fireOneCooldown <= 0)   //If the ability is not on cooldown
			{
				fireOneCooldown = fireOneCooldownTime;
				FireProjectile();
			}
		}
	}

	private void FireProjectile()    //Spawns Projectile when Fire1 is active.
	{
		float audioShift = Random.Range(-.25f, .25f);
		soundEffect.pitch = 1 + audioShift;
		soundEffect.PlayOneShot(soundFire1);

		//Debug.LogWarning("Projectile Launched");
		Instantiate(PewPrefab,                    //Object to Spawn
				SpawnLocation.transform.position,   //Position to Spawn Object
				SpawnLocation.transform.rotation);  //Rotation to Spawn Object
	}
}
