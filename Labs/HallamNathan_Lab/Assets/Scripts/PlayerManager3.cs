using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager3 : MonoBehaviour
{
	GameObject Player;
	CharacterController controller;
	public GameObject projectileSpawn;
	public GameObject projectile;
	public float playerSpeed = 2;
	public float jumpHeight = 2;
	float playerVelocity;
	public float gravityValue = 9.81f;
	bool groundedPlayer = true;
	public TMP_Text user;
	PhotonView photonView;
	public float projectileSpeed = 1000f;
	public float projectileLifetime = 5f;
	public float verticalSpeed = 1f;
	public float horizontalSpeed = 1f;
	private float xRotation = 0.0f;
	private float yRotation = 0.0f;
	public Camera cam;

	public GameObject playerInfo;
	private Animator anim;

	public AudioSource audioSource;
	public AudioClip shoot;
	public AudioClip walk;
	public float volume;
	public Transform sourceTransform;

	void Awake()
	{
		Player = gameObject;
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
	}

	//http://gyanendushekhar.com/2020/02/06/first-person-movement-in-unity-3d/
	void Update()
	{
		if (photonView.IsMine)
		{
			float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
			float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

			yRotation += mouseX;
			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -90, 90);

			cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
			gameObject.transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);


			float horizontal = Input.GetAxis("Horizontal") * playerSpeed;
			float vertical = Input.GetAxis("Vertical") * playerSpeed;
			controller.Move((cam.transform.right * horizontal + cam.transform.forward * vertical) * Time.deltaTime);

			float move = Mathf.Abs(horizontal + vertical);


			anim.SetFloat("Move", move);

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

			if (Input.GetButtonDown("Fire1"))
			{
				Fire();
			}
		}
	}

	public void Fire()
	{
		photonView.RPC("RPCFire", RpcTarget.All, projectileSpawn.transform.forward * projectileSpeed);
	}


	[PunRPC]
	public void RPCFire(Vector3 velocity)
	{
		GameObject proj = Instantiate(projectile, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
		Rigidbody rb = proj.GetComponent<Rigidbody>();
		rb.velocity = velocity;
		Destroy(proj, projectileLifetime);

		audioSource.PlayOneShot(shoot);
	}

	public void WalkSound()
	{
		Debug.Log("Play Walk");

		if (photonView.IsMine)
		{
			audioSource.Play();
		}
	}
}