using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody2D), typeof(EdgeCollider2D))]
[RequireComponent(typeof(SpriteRenderer), typeof(ScreenWrap))]
public class PlayerController : MonoBehaviour, IPunObservable
{
	PhotonView photonView;
	Rigidbody2D rb;
	EdgeCollider2D edgeCollider;

	#region Movement Variables
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

	#region Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;
	bool fire1 = false;
	public bool canThrustBack = false;
	#endregion Input Variables

	#region Weapon Variables
	bool fireOneIsEnabled = true;
	float fireOneCooldown = 0;
	public bool fireOneOnCooldown = false;
	public float fireOneCooldownTime = 6;  //in Seconds.
	public float cooldownOneSpeed = 1; //For Fire1 - 1 is normal speed : 2 is 2X faster : 0.5 is 2X slower. If there were to be powerups - this would change the "reload" speed.
									   //Projectile Instantiate Variables
	public GameObject PewPrefab;
	public GameObject SpawnLocation;
	[SerializeField] float pewLifetime = 1f;
	[SerializeField] float pewDamage = 1f;
	[SerializeField] float pewSpeed = 50f;
	#endregion Weapon Variables

	#region Sync Variables
	Vector3 networkPosition;
	float networkRotation;
	[SerializeField] Color color;

	public float LerpSpeed = 0.1f;

	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
	#endregion Sync Variables

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody2D>();
		edgeCollider = GetComponent<EdgeCollider2D>();
		ps = GetComponent<ParticleSystem>();


		properties = photonView.Controller.CustomProperties;


		color = new Color((float)properties["colorRed"], (float)properties["colorGreen"], (float)properties["colorBlue"], (float)properties["colorAlpha"]);
	}

	private void Start()
	{
		psMain = ps.main;
		psMain.startColor = color;

		GetComponent<SpriteRenderer>().color = color;
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
			UpdateTransform();
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

	IEnumerator FireCooldown()
	{
		yield return new WaitForSecondsRealtime(fireOneCooldown);

		fireOneOnCooldown = false;
	}

	private void Fire1() //Checks if player has used ability and if fire1 is on cooldown, if available then call FireProjectile.
	{
		if (fireOneIsEnabled)   //If the player can fire
		{
			if (!fireOneOnCooldown)   //If the ability is not on cooldown
			{
				fireOneCooldown = fireOneCooldownTime;
				FireProjectile();
			}
		}
	}

	private void FireProjectile()    //Spawns Projectile when Fire1 is active.
	{
		fireOneOnCooldown = true;
		StartCoroutine(FireCooldown());

		GameObject pew = Instantiate(PewPrefab,                    //Object to Spawn
				SpawnLocation.transform.position,   //Position to Spawn Object
				SpawnLocation.transform.rotation);  //Rotation to Spawn Object

		Rigidbody2D pewRB = pew.GetComponent<Rigidbody2D>();
		pewRB.velocity = gameObject.transform.up * (pewSpeed + rb.velocity.magnitude);

		Destroy(pew, pewLifetime);
	}
	
	public void UpdateTransform()
	{
		rb.position = Vector3.MoveTowards(rb.position, networkPosition, Time.fixedDeltaTime);
		rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(rb.position);
			stream.SendNext(rb.rotation);
			stream.SendNext(rb.velocity);
			stream.SendNext(forward);
			stream.SendNext(color.r);
			stream.SendNext(color.g);
			stream.SendNext(color.b);
			stream.SendNext(color.a);
		}
		else if (stream.IsReading)
		{
			networkPosition = (Vector2)stream.ReceiveNext();
			networkRotation = (float)stream.ReceiveNext();
			rb.velocity = (Vector2)stream.ReceiveNext();
			forward = (bool)stream.ReceiveNext();
			color.r = (float)stream.ReceiveNext();
			color.g = (float)stream.ReceiveNext();
			color.b = (float)stream.ReceiveNext();
			color.a = (float)stream.ReceiveNext();
		}

		/*   if (stream.IsWriting)
   {
	   stream.SendNext(rigidbody.position);
	   stream.SendNext(rigidbody.rotation);
	   stream.SendNext(rigidbody.velocity);
   }
   else
   {
	   networkPosition = (Vector3)stream.ReceiveNext();
	   networkRotation = (Quaternion)stream.ReceiveNext();
	   rigidbody.velocity = (Vector3)stream.ReceiveNext();

	   float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
	   networkPosition += (rigidbody.velocity * lag);
   }
		 * 
		 */





	}
}
