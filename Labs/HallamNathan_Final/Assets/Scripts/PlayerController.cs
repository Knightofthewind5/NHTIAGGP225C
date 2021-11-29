using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody2D), typeof(PolygonCollider2D))]
[RequireComponent(typeof(ScreenWrap))]
public class PlayerController : MonoBehaviour, IPunObservable
{
	public PhotonView photonView { get; private set; }
	public Rigidbody2D rb { get; private set; }
	PolygonCollider2D polyCollider;

	#region Movement Variables
	[Header("Movement")]
	[Tooltip("How fast the player is capable of moving")]
	[SerializeField] float maxSpeed = 71f;
	[Tooltip("How fast the player will accelerate")]
	[SerializeField] float acceleration = 10f;
	[Tooltip("How fast the player will rotate")]
	[SerializeField] float rotationSpeed = 145f;
	[Tooltip("How long it takes for the player to stop moving when not thrusting. 0 is no stopping.")]
	[Range(0f, 1f)]
	[SerializeField] float brakingPower = 0.15f;
	#endregion Movement Variables

	#region Shield Variables
	[Header("Shields")]
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
	#endregion Shield Variables

	ParticleSystem ps;
	ParticleSystem.MainModule psMain;

	public AudioSource AS { get; private set; }
	public AudioListener AL { get; private set; }

	#region Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;
	bool fire1 = false;
	public bool canThrustBack = false;
	#endregion Input Variables

	#region Weapon Variables
	int primaryWeaponIndex = 0;
	public WeaponStats PrimaryWeapon { get; private set; }
	float primaryWeaponCooldown = 0;
	public bool primaryWeaponOnCooldown = false;
	public GameObject SpawnLocation;
	#endregion Weapon Variables

	#region Sync Variables
	Vector3 networkPosition;
	float networkRotation;
	[SerializeField] Color color;

	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
	#endregion Sync Variables

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody2D>();
		polyCollider = GetComponent<PolygonCollider2D>();
		ps = GetComponent<ParticleSystem>();
		AS = GetComponent<AudioSource>();
		AL = GetComponent<AudioListener>();

		properties = photonView.Controller.CustomProperties;

		gameObject.name = "(Player) " + photonView.Controller;
		color = new Color((float)properties["colorRed"], (float)properties["colorGreen"], (float)properties["colorBlue"], (float)properties["colorAlpha"]);
	}

	private void Start()
	{
		psMain = ps.main;
		psMain.startColor = color;

		GetComponentInChildren<SpriteRenderer>().color = color;

		PrimaryWeapon = new WeaponStats(GameManager.Instance.weapons[primaryWeaponIndex]);
	}

	private void FixedUpdate()
	{
		if (photonView.IsMine)
		{
			GetInput();

			if (forward && rb.velocity.magnitude < maxSpeed)
			{
				Move(1);
			}
			else 
			{
				rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, brakingPower * (rb.velocity.magnitude / 10));				
			}

			if (forward)
			{
				ps.Play();
			}
			else if (!forward)
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

			if (fire1)
			{
				Fire1();
			}
		}
		else if (!photonView.IsMine)
		{
			AL.enabled = false;

			if (forward)
			{
				ps.Play();
			}
			else
			{
				ps.Stop();
			}

			if (psMain.startColor.color != color)
			{
				psMain.startColor = color;
				GetComponent<SpriteRenderer>().color = color;
			}
		}
	}

	void GetInput()
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
		rb.AddForce(transform.up * acceleration * value * Time.deltaTime);
	}

	void Rotate(float value)
	{
		gameObject.transform.Rotate(Vector3.back * value * rotationSpeed * Time.deltaTime);
	}

	IEnumerator PrimaryWeaponCooldown()
	{
		yield return new WaitForSecondsRealtime(primaryWeaponCooldown);

		primaryWeaponOnCooldown = false;
	}

	private void Fire1() //Checks if player has used ability and if fire1 is on cooldown, if available then call FireProjectile.
	{
		if (!primaryWeaponOnCooldown)   //If the ability is not on cooldown
		{
			primaryWeaponCooldown = PrimaryWeapon.fireRate;
			FireProjectile();
		}	
	}

	private void FireProjectile()    //Spawns Projectile when Fire1 is active.
	{
		primaryWeaponOnCooldown = true;

		StartCoroutine(PrimaryWeaponCooldown());

		RPCManager.Instance.PV.RPC("ShootProjectile", RpcTarget.AllBuffered, photonView.ViewID, SpawnLocation.transform.position, SpawnLocation.transform.rotation, Random.Range(100000, 999999));
	}

	public void ModifyHealth(float value)
	{

	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(photonView.Controller.NickName);
			stream.SendNext(rb.velocity);
			stream.SendNext(forward);
			stream.SendNext(color.r);
			stream.SendNext(color.g);
			stream.SendNext(color.b);
			stream.SendNext(color.a);
		}
		else if (stream.IsReading)
		{
			gameObject.name = "(Player) " + (string)stream.ReceiveNext();
			rb.velocity = (Vector2)stream.ReceiveNext();
			forward = (bool)stream.ReceiveNext();
			color.r = (float)stream.ReceiveNext();
			color.g = (float)stream.ReceiveNext();
			color.b = (float)stream.ReceiveNext();
			color.a = (float)stream.ReceiveNext();
		}
	}
}
