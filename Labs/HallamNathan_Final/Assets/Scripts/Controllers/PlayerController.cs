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
	SpriteRenderer SR;

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
	// Recharging - The process of a shield filling broken portions of its array when damaged
	// Regenerate - The process of a shield rebuilding the entire shield array when broken
	[Header("Shields")]
	[Tooltip("The maximum HP the shields have")]
	[SerializeField] float maxShieldStrength = 10f;

	[Tooltip("How many points does the shields regenerate per second when damaged")]
	[SerializeField] float shieldRechargeRate = 10f;

	[Tooltip("How long in seconds the player has to wait for shields to begin recharge")]
	[SerializeField] float shieldRechargeWait = 3f;

	[Tooltip("How many points does the shields regenerate per second when broken")]
	[SerializeField] float shieldRegenRate = 2f;

	[Tooltip("How long in seconds it takes for the shields regeneration timer to start after the shields are broken")]
	[SerializeField] float shieldRegenPause = 5f;

	[Tooltip("The total time in seconds the player has to wait for shields to begin regenerate")]
	[SerializeField] float shieldRegenWait = 10f;

	[Tooltip("How many seconds of invulnerability does the player have when their sheilds break")]
	[SerializeField] float gracePeriod = 1.5f;

	bool hasGraced = false;

	public Image ShieldBar;
	public Image ShieldRecharge;
	public CanvasGroup Shields;
	public Color ShieldNormalColor = new Color(1f, 1f, 1f, 1f);
	public Color LowShieldBlink1 = new Color(1f, 0f, 0f, 1f);
	public Color LowShieldBlink2 = new Color(1f, 0f, 0f, 0.5f);
	public float shieldBlinkInterval1 = 0.25f;
	public float shieldBlinkInterval2 = 0.25f;
	private float currentShieldStrength = 0f;
	IEnumerator _shieldTimer;
	IEnumerator _graceTimer;
	IEnumerator _shieldBlinkTimer;
	#endregion Shield Variables

	ParticleSystem JetTrailPS;
	ParticleSystem ShieldsDestroyedPS;
	ParticleSystem.MainModule psMain;

	public AudioSource ASgeneral { get; private set; }
	public AudioSource ASthrust { get; private set; }
	public AudioClip DeathSound;

	#region Input Variables
	bool forward = false;
	bool backward = false;
	bool left = false;
	bool right = false;
	bool fire1 = false;
	public bool canThrustBack = false;
	#endregion Input Variables

	#region Weapon Variables
	public int primaryWeaponIndex = 0;
	public WeaponStats PrimaryWeapon { get; private set; }
	float primaryWeaponCooldown = 0;
	public bool primaryWeaponOnCooldown = false;
	public GameObject SpawnLocation;
	#endregion Weapon Variables

	#region Sync Variables
	[SerializeField] Color color;

	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

	string shuttleName;
	#endregion Sync Variables

	CanvasGroup chat;

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;

		photonView = GetComponent<PhotonView>();
		rb = GetComponent<Rigidbody2D>();
		polyCollider = GetComponent<PolygonCollider2D>();
		JetTrailPS = transform.GetComponent<ParticleSystem>();
		ASgeneral = GetComponent<AudioSource>();
		ASthrust = transform.GetChild(3).GetChild(0).GetComponent<AudioSource>();
		SR = transform.GetChild(0).GetComponent<SpriteRenderer>();

		properties = photonView.Controller.CustomProperties;

		gameObject.name = "(Player) " + photonView.Controller;
		color = new Color((float)properties["colorRed"], (float)properties["colorGreen"], (float)properties["colorBlue"], (float)properties["colorAlpha"]);

		shuttleName = (string)properties["shuttleName"];

		foreach (var shuttle in GameManager.Instance.shuttleStats.shuttles)
		{
			if (shuttle.shuttleName == shuttleName)
			{
				acceleration = shuttle.acceleration;
				brakingPower = shuttle.brakingPower;
				gracePeriod = shuttle.gracePeriod;
				maxShieldStrength = shuttle.maxShieldStrength;
				maxSpeed = shuttle.maxSpeed;
				rotationSpeed = shuttle.rotationSpeed;
				shieldRechargeRate = shuttle.shieldRechargeRate;
				shieldRechargeWait = shuttle.shieldRechargeWait;
				shieldRegenPause = shuttle.shieldRegenPause;
				shieldRegenRate = shuttle.shieldRegenRate;
				shieldRegenWait = shuttle.shieldRegenWait;
				gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = shuttle.shuttleSprite;
			}
		}

		chat = ChatManager.Instance.gameObject.GetComponentInParent<CanvasGroup>();
	}

	private void Start()
	{
		psMain = JetTrailPS.main;
		psMain.startColor = color;

		GetComponentInChildren<SpriteRenderer>().color = color;
		ShieldsDestroyedPS = transform.GetChild(0).GetComponentInChildren<ParticleSystem>();

		PrimaryWeapon = new WeaponStats(GameManager.Instance.weapons[primaryWeaponIndex]);

		if (!hasGraced)
		{
			_graceTimer = GraceTimer(5f);
			StartCoroutine(_graceTimer);
		}

		_shieldTimer = ShieldRechargeTimer();
		StartCoroutine(_shieldTimer);
	}

	private void FixedUpdate()
	{
		if (!GameSettingsManager.Instance.noShields)
		{
			UpdateHUD();
		}
		else if (GameSettingsManager.Instance.noShields && Shields.activeInHierarchy())
		{
			Shields.SetActive(false);
		}

		if (photonView.IsMine)
		{
			GetInput();

			if (forward && rb.velocity.magnitude < (maxSpeed * GameSettingsManager.Instance.playerMaxSpeedMultiplier) && !chat.activeInHierarchy())
			{
				Move(GameSettingsManager.Instance.playerAccelerationMultiplier);
			}
			else 
			{
				rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, (brakingPower * GameSettingsManager.Instance.playerBrakingpowerMultiplier) * (rb.velocity.magnitude / 10));				
			}

			if (forward && !chat.activeInHierarchy())
			{
				JetTrailPS.Play(false);

				if (!ASthrust.isPlaying)
				{
					ASthrust.Play();
				}

			}
			else if (!forward)
			{
				JetTrailPS.Stop(false);
				ASthrust.Stop();
			}

			if (backward && !chat.activeInHierarchy())
			{
				if (canThrustBack == true)
				{
					Move(-GameSettingsManager.Instance.playerAccelerationMultiplier);
				}
			}
			if (right && !chat.activeInHierarchy())
			{
				Rotate(GameSettingsManager.Instance.playerRotationMultiplier);
			}
			if (left && !chat.activeInHierarchy())
			{
				Rotate(-GameSettingsManager.Instance.playerRotationMultiplier);
			}

			if (fire1 && !chat.activeInHierarchy())
			{
				Fire1();
			}
				
		}
		else if (!photonView.IsMine)
		{
			if (forward)
			{
				JetTrailPS.Play(false);
			}
			else
			{
				JetTrailPS.Stop(false);
			}

			if (psMain.startColor.color != color)
			{
				psMain.startColor = color;
				GetComponent<SpriteRenderer>().color = color;
			}

			if (shuttleName != properties["shuttleName"].ToString())
			{
				foreach (var shuttle in GameManager.Instance.shuttleStats.shuttles)
				{
					if (shuttle.shuttleName == shuttleName)
					{
						acceleration = shuttle.acceleration;
						brakingPower = shuttle.brakingPower;
						gracePeriod = shuttle.gracePeriod;
						maxShieldStrength = shuttle.maxShieldStrength;
						maxSpeed = shuttle.maxSpeed;
						rotationSpeed = shuttle.rotationSpeed;
						shieldRechargeRate = shuttle.shieldRechargeRate;
						shieldRechargeWait = shuttle.shieldRechargeWait;
						shieldRegenPause = shuttle.shieldRegenPause;
						shieldRegenRate = shuttle.shieldRegenRate;
						shieldRegenWait = shuttle.shieldRegenWait;
						gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = shuttle.shuttleSprite;
					}
				}
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

	void UpdateHUD()
	{
		float shieldPercentage = currentShieldStrength / (maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier);

		ShieldBar.fillAmount = shieldPercentage;

		if (_shieldBlinkTimer == null && currentShieldStrength < ((maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier) / 3) && currentShieldStrength > 0)
		{
			_shieldBlinkTimer = ShieldLowBlink();
			StartCoroutine(_shieldBlinkTimer);
		}
		else if (_shieldBlinkTimer == null && currentShieldStrength == 0)
		{
			_shieldBlinkTimer = ShieldBrokenBlink();
			StartCoroutine(_shieldBlinkTimer);
		}
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

	IEnumerator ShieldRechargeTimer()
	{
		float currentShieldEnergy = 0;

		do
		{
			currentShieldEnergy += Time.fixedDeltaTime;

			float shieldRechargePercentage = currentShieldEnergy / shieldRechargeWait;

			ShieldRecharge.fillAmount = shieldRechargePercentage;

			yield return new WaitForFixedUpdate();
		}
		while (currentShieldEnergy < shieldRechargeWait);

		yield return new WaitForEndOfFrame();

		do
		{
			currentShieldStrength += (shieldRechargeRate * Time.fixedDeltaTime);

			yield return new WaitForFixedUpdate();
		}
		while (currentShieldStrength < (maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier));

		currentShieldStrength = (maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier);

		_shieldTimer = null;

		yield return new WaitForSeconds(1f);

		Shields.Fade(false, 1f);
	}

	IEnumerator ShieldRegenTimer()
	{	
		if (photonView.IsMine)
		{
			GameManager.Instance.noShieldVignette.Fade(true, 0.5f);
			ShieldsDestroyedPS.Play();
		}

		if (!hasGraced)
		{
			_graceTimer = GraceTimer(gracePeriod);
			StartCoroutine(_graceTimer);
		}

		yield return new WaitForSeconds(shieldRegenPause);

		float currentShieldEnergy = 0;

		do
		{
			currentShieldEnergy += Time.fixedDeltaTime;

			float shieldRegenPercentage = currentShieldEnergy / shieldRegenWait;

			ShieldRecharge.fillAmount = shieldRegenPercentage;

			yield return new WaitForFixedUpdate();
		}
		while (currentShieldEnergy < shieldRegenWait);

		yield return new WaitForEndOfFrame();

		if (photonView.IsMine)
		{
			GameManager.Instance.noShieldVignette.Fade(false, 0.5f);
		}

		do
		{
			currentShieldStrength += (shieldRegenRate * Time.fixedDeltaTime);

			yield return new WaitForFixedUpdate();
		}
		while (currentShieldStrength < (maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier));

		currentShieldStrength = (maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier);

		_shieldTimer = null;

		yield return new WaitForSeconds(1f);

		Shields.Fade(false, 1f);
	}

	IEnumerator GraceTimer(float time)
	{
		hasGraced = true;
		polyCollider.enabled = false;
		Color spriteColor = SR.color;
		spriteColor.a = 0.5f;
		SR.color = spriteColor;
		ParticleSystem.EmissionModule psem = JetTrailPS.emission;
		psem.enabled = false;

		yield return new WaitForSeconds(time);

		psem.enabled = true;
		polyCollider.enabled = true;
		spriteColor.a = (float)properties["colorAlpha"];
		SR.color = spriteColor;

		yield return new WaitUntil(() => _shieldTimer == null);

		_graceTimer = null;
		hasGraced = false;
	}

	IEnumerator ShieldBrokenBlink()
	{
		do
		{
			Shields.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = LowShieldBlink1;
			Shields.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = LowShieldBlink1;

			yield return new WaitForSeconds(shieldBlinkInterval1);

			Shields.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = LowShieldBlink2;
			Shields.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = LowShieldBlink2;

			yield return new WaitForSeconds(shieldBlinkInterval2);
		}
		while (currentShieldStrength <= 0);		

		Shields.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = ShieldNormalColor;
		Shields.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = ShieldNormalColor;

		_shieldBlinkTimer = null;
	}

	IEnumerator ShieldLowBlink()
	{
		do
		{
			Shields.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = LowShieldBlink1;

			yield return new WaitForSeconds(shieldBlinkInterval1 * 1.5f);

			Shields.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = ShieldNormalColor;
			Shields.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = ShieldNormalColor;

			yield return new WaitForSeconds(shieldBlinkInterval2 * 1.5f);
		}
		while (currentShieldStrength < ((maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier) / 3) && currentShieldStrength > 0);

		Shields.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = ShieldNormalColor;
		Shields.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = ShieldNormalColor;

		_shieldBlinkTimer = null;
	}

	public void ModifyHealth(float amount)
	{
		if (!GameSettingsManager.Instance.invulnerability)
		{
			photonView.RPC("ModifyHealthRPC", RpcTarget.All, amount);
		}
	}

	[PunRPC]
	public void ModifyHealthRPC(float amount)
	{
		if (!GameSettingsManager.Instance.noShields)
		{
			GameManager.Instance.currentScoreMX = 1f;

			currentShieldStrength += amount;

			ShieldRecharge.fillAmount = 0f;

			if (currentShieldStrength != (maxShieldStrength * GameSettingsManager.Instance.playerHealthMultiplier) && Shields.alpha != 1)
			{
				Shields.StopFade(false);
			}

			if (currentShieldStrength <= 0 && hasGraced) // If the shield is broken and the player has entered their grace period
			{
				if (PhotonNetwork.LocalPlayer.IsLocal && photonView.IsMine)
				{
					currentShieldStrength = 0;

					GameManager.Instance.PV.RPC("RemoveAlivePlayerRPC", RpcTarget.MasterClient, photonView.ViewID);

					PhotonNetwork.Destroy(this.gameObject);
				}
			}
			else if (currentShieldStrength <= 0)
			{
				currentShieldStrength = 0;

				if (_shieldTimer == null) // If the coroutine is not running
				{
					_shieldTimer = ShieldRegenTimer();
					StartCoroutine(_shieldTimer);
				}
				else
				{
					//Stop the coroutine and restart it as the proper coroutine
					StopCoroutine(_shieldTimer);
					_shieldTimer = ShieldRegenTimer();
					StartCoroutine(_shieldTimer);
				}
			}
			else if (amount < 0) // If not healing
			{
				if (_shieldTimer == null) // If the coroutine is not running
				{
					//Start normally
					_shieldTimer = ShieldRechargeTimer();
					StartCoroutine(_shieldTimer);
				}
				else
				{
					//Stop the coroutine and restart it as the proper coroutine
					StopCoroutine(_shieldTimer);
					_shieldTimer = ShieldRechargeTimer();
					StartCoroutine(_shieldTimer);
				}
			}
		}
		else
		{
			if (PhotonNetwork.LocalPlayer.IsLocal && photonView.IsMine)
			{
				GameManager.Instance.PV.RPC("RemoveAlivePlayerRPC", RpcTarget.MasterClient, photonView.ViewID);

				PhotonNetwork.Destroy(this.gameObject);
			}
		}
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
			stream.SendNext(currentShieldStrength);
			stream.SendNext(shuttleName);
			stream.SendNext(polyCollider.enabled);
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
			currentShieldStrength = (float)stream.ReceiveNext();
			shuttleName = (string)stream.ReceiveNext();
			polyCollider.enabled = (bool)stream.ReceiveNext();
		}
	}

	public void OnDestroy()
	{
		Cursor.lockState = CursorLockMode.None;

		GameManager.Instance.noShieldVignette.Fade(false, 0.5f);
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(DeathSound);

		if (photonView.IsMine)
		{
			GameManager.Instance.DeathCanvas.Fade(true, 5f);
			GameManager.Instance.DeathCanvas.interactable = true;
			GameManager.Instance.DeathCanvas.blocksRaycasts = true;
			GameManager.Instance.localPlayerAlive = false;
		}

		for (int count = 8; count > 0; count--)
		{
			float rotation = Random.Range(0f, 360f);

			RPCManager.Instance.PV.RPC("SpawnAsteroidsRPC", RpcTarget.All, 0, gameObject.transform.position, rotation, Random.Range(100000, 999999));
		}
	}
}
