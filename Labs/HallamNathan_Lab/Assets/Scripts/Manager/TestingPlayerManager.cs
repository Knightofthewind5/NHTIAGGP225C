using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class TestingPlayerManager : MonoBehaviour
{
	CharacterController controller;
	public GameObject projectileSpawn;
	public GameObject pistolBullet;
	public GameObject rifleBullet;
	public GameObject shotgunBullet;
	public float currentPlayerSpeed = 5;
	public float defaultPlayerSpeed = 5;
	public float jumpHeight = 2;
	float playerVelocity;
	public float gravityValue = 9.81f;
	public TMP_Text user;
	[HideInInspector] public PhotonView photonView;
	public float pistolProjectileSpeed = 1000f;
	public float rifleProjectileSpeed = 1000f;
	public float shotgunProjectileSpeed = 1000f;
	public float projectileLifetime = 5f;
	public float verticalSpeed = 1f;
	public float horizontalSpeed = 1f;
	private float xRotation = 0.0f;
	private float yRotation = 0.0f;
	public Camera cam;
	public bool pistol = true;
	public bool rifle = false;
	public bool shotgun = false;
	public bool holdFire = false;
	public float pistolFireRate = 1f;
	public float rifleFireRate = 0.5f;
	public float shotgunFireRate = 2f;
	float fireCooldown = 0f;
	bool canFireForever = false;

	float boostSpeedTime = 0f;
	float boostSpeed = 10f;


	public Image healthBar;
	DataSync DS;
	BulletRoundManager GM;

	public GameObject playerInfo;
	private Animator anim;

	public AudioSource audioSource;
	public AudioClip shoot;
	public AudioClip walk;
	public float volume;
	public Transform sourceTransform;

	void Awake()
	{
		currentPlayerSpeed = defaultPlayerSpeed;

		GM = FindObjectOfType<BulletRoundManager>();
		healthBar = GM.HealthBar;
		controller = gameObject.GetComponent<CharacterController>();
		photonView = GetComponent<PhotonView>();

		if (photonView.IsMine)
		{
			cam.GetComponent<Camera>().enabled = true;
		}
		else
		{
			cam.GetComponent<Camera>().enabled = false;
		}

		anim = gameObject.GetComponent<Animator>();
		DS = GetComponent<DataSync>();
	}

	//http://gyanendushekhar.com/2020/02/06/first-person-movement-in-unity-3d/
	void Update()
	{
		fireCooldown -= Time.deltaTime;

		if (fireCooldown <= -10 && canFireForever)
		{
			canFireForever = false;
		}

		if (boostSpeedTime > 0)
		{
			boostSpeedTime -= Time.deltaTime;
			currentPlayerSpeed = boostSpeed;
		}
		else
		{
			currentPlayerSpeed = defaultPlayerSpeed;
		}

		// Gravity
		if (controller.isGrounded)
		{
			playerVelocity = 0;
		}
		else
		{
			playerVelocity -= gravityValue * Time.deltaTime;
			controller.Move(new Vector3(0, playerVelocity, 0));
		}

		//healthBar.fillAmount = (DS.Health / DS.MaxHealth);

		float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
		float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

		yRotation += mouseX;
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90, 90);

		cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
		gameObject.transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);

		float horizontal = Input.GetAxis("Horizontal") * currentPlayerSpeed;
		float vertical = Input.GetAxis("Vertical") * currentPlayerSpeed;
		controller.Move((cam.transform.right * horizontal + cam.transform.forward * vertical) * Time.deltaTime);

		float move = Mathf.Abs(horizontal + vertical);


		anim.SetFloat("Move", move);

		if (Input.GetButtonDown("Fire1") && !holdFire && fireCooldown <= 0)
		{
			Fire();
		}
		else if (Input.GetButton("Fire1") && holdFire && fireCooldown <= 0)
		{
			Fire();
		}

		if (move == 0)
		{
			anim.SetFloat("Move", 0);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Debug.Log("1 Pressed");

			pistol = true;
			rifle = false;
			shotgun = false;
			holdFire = false;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Debug.Log("2 Pressed");
			pistol = false;
			rifle = true;
			shotgun = false;
			holdFire = true;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Debug.Log("3 Pressed");
			pistol = false;
			rifle = false;
			shotgun = true;
			holdFire = false;
		}
	}
	

	public void Fire()
	{
		if (pistol)
		{
			if (!canFireForever)
			{
				fireCooldown = pistolFireRate;
			}

			RPCFire(projectileSpawn.transform.forward * pistolProjectileSpeed);
		}
		else if (rifle)
		{
			if (!canFireForever)
			{
				fireCooldown = rifleFireRate;
			}

			RPCFire(projectileSpawn.transform.forward * rifleProjectileSpeed);
		}
		else if (shotgun)
		{
			if (!canFireForever)
			{
				fireCooldown = shotgunFireRate;
			}

			RPCFire(projectileSpawn.transform.forward * shotgunProjectileSpeed);
		}
	}


	[PunRPC]
	public void RPCFire(Vector3 velocity)
	{
		GameObject proj;

		if (pistol)
		{
			proj = Instantiate(pistolBullet, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			rb.velocity = velocity;
			Destroy(proj, projectileLifetime);

			audioSource.PlayOneShot(shoot);
		}
		else if (rifle)
		{
			proj = Instantiate(rifleBullet, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			rb.velocity = velocity;
			Destroy(proj, projectileLifetime);

			audioSource.PlayOneShot(shoot);
		}
		else if (shotgun)
		{
			proj = Instantiate(shotgunBullet, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			rb.velocity = velocity;
			Destroy(proj, projectileLifetime);

			audioSource.PlayOneShot(shoot);
		}
	}

	public void WalkSound()
	{
		//Debug.Log("Play Walk");

		if (photonView.IsMine)
		{
			audioSource.Play();
		}
	}
}