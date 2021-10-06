using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class DataSync : MonoBehaviourPun, IPunObservable
{
	public Vector3 ObjPosition;
	public Quaternion ObjRotation;
	public Quaternion CameraRotation;
	public Vector3 ObjScale;

	public float LerpSpeed = 3f;


	public float Health;
	public float MaxHealth;
	public float MinHealth;

	Camera camera;

	public TMP_Text username;
	public Image healthbar;
	float fillAmount;

	private void Awake()
	{
		camera = GetComponentInChildren<Camera>();
		username.text = PlayerPrefs.GetString("Username");
	}

	private void Update()
	{
		if (!photonView.IsMine)
		{
			UpdateTransform();
		}

		if (Health > MaxHealth)
		{
			Health = MaxHealth;
		}
		else if (Health <= MinHealth)
		{
			Health = MinHealth;
			PhotonNetwork.Destroy(gameObject);
		}

		fillAmount = (Health / MaxHealth);
		healthbar.fillAmount = fillAmount;
	}

	public void UpdateTransform()
	{
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, ObjPosition, LerpSpeed * Time.deltaTime);
		gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, ObjRotation, LerpSpeed * Time.deltaTime);
		camera.gameObject.transform.rotation = Quaternion.Lerp(camera.gameObject.transform.rotation, CameraRotation, LerpSpeed * Time.deltaTime);
		gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, ObjScale, LerpSpeed * Time.deltaTime);
	}

	public void TakeDamage(float amount)
	{
		Debug.Log(gameObject.name + " Attempt Damage");

		if (!photonView.IsMine)
		{
			Debug.Log(gameObject.name + " Damage Taken");

			photonView.RPC("Damage", RpcTarget.All, amount);
		}
	}

	[PunRPC]
	void Damage(float amount)
	{
		Health -= amount;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(gameObject.transform.position);
			stream.SendNext(gameObject.transform.rotation);
			stream.SendNext(camera.gameObject.transform.rotation);
			stream.SendNext(gameObject.transform.localScale);
			stream.SendNext(Health);
			stream.SendNext(username.text);
		}
		else if (stream.IsReading)
		{
			ObjPosition = (Vector3)stream.ReceiveNext();
			ObjRotation = (Quaternion)stream.ReceiveNext();
			CameraRotation = (Quaternion)stream.ReceiveNext();
			ObjScale = (Vector3)stream.ReceiveNext();
			Health = (float)stream.ReceiveNext();
			username.text = (string)stream.ReceiveNext();
		}
	}
}
