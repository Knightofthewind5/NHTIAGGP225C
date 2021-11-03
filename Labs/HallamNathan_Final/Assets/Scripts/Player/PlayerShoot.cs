using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
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

	PlayerMovement2d PM2D;
	public GameObject Player;

	public bool debugMode = false;

	public Manager GameManager;

	// Start is called before the first frame update
	void Start()
	{
		PM2D = Player.GetComponent<PlayerMovement2d>();
	}

	private void FixedUpdate()
	{
		GetInput();

		if (fire1)
		{
			Fire1();
		}
	}

	void GetInput()
	{

		if (PM2D.useGamePad == true)
		{
			bool gamepadIsConnected = GetInputGamePad(Gamepad.current);
			if (gamepadIsConnected)
			{
				return;
			}
		}

		if (PM2D.playerId == 1)
		{
			GetInputPlayer1();
		}
	}
	bool GetInputGamePad(Gamepad gpad)
	{
		if (gpad == null)
		{
			if (debugMode == true)
			{
				Debug.LogWarning("Warning: No gamepad connected - Swapping to Default Controls");
			}
			return false;
		}

		Vector2 LStick = gpad.leftStick.ReadValue();
		Vector2 RStick = gpad.rightStick.ReadValue();

		fire1 = gpad.buttonSouth.wasPressedThisFrame;

		return true;
	}

	void GetInputPlayer1()
	{
		fire1 = Input.GetKey(KeyCode.Space);
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
		GameManager.IncreaseFire();

		float audioShift = Random.Range(-.15f, .15f);
		soundEffect.pitch = 1 + audioShift;
		soundEffect.PlayOneShot(soundFire1);

		Instantiate(PewPrefab,                    //Object to Spawn
				SpawnLocation.transform.position,   //Position to Spawn Object
				SpawnLocation.transform.rotation);  //Rotation to Spawn Object

		if (debugMode == true)
		{
			Debug.LogWarning("Projectile Launched");
		}

	}
}
