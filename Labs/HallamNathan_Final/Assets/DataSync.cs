using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DataSync : MonoBehaviourPun, IPunObservable
{
	public Vector2 ObjPosition;
	public Quaternion ObjRotation;
	public Quaternion CameraRotation;
	public Vector2 ObjScale;
	public Color color;

	public float LerpSpeed = 3f;

	public float Health;
	public float MaxHealth;
	public float MinHealth;

	public TMP_Text username;
	public Image healthbar;
	float fillAmount;
	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

	private void Awake()
	{
		if (photonView.IsMine)
		{
			properties = PhotonNetwork.LocalPlayer.CustomProperties;

			username.text = PhotonNetwork.LocalPlayer.NickName;
			color = new Color((float)properties["colorRed"], (float)properties["colorGreen"], (float)properties["colorBlue"], (float)properties["colorAlpha"]);
		}
	}

	private void Update()
	{
		if (!photonView.IsMine)
		{
			UpdateTransform();
			username.color = color;
		}
		else
		{
			if (MaxHealth > 0)
			{
				if (Health > MaxHealth)
				{
					Health = MaxHealth;
				}
			}
		}

		fillAmount = (Health / MaxHealth);
		healthbar.fillAmount = fillAmount;
	}

	public void UpdateTransform()
	{
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, ObjPosition, LerpSpeed * Time.deltaTime);
		gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, ObjRotation, LerpSpeed * Time.deltaTime);
		gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, ObjScale, LerpSpeed * Time.deltaTime);
	}

	public void TakeDamage(float amount, string dealer)
	{
		if (!photonView.IsMine)
		{
			if (dealer != null)
			{
				Debug.Log(gameObject.GetPhotonView().Owner.NickName + " Damage Taken from " + dealer);
			}

			photonView.RPC("Damage", RpcTarget.AllBuffered, amount);
		}
		else
		{
			Health -= amount;
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
			stream.SendNext(gameObject.transform.localScale);
			stream.SendNext(username.text);
			stream.SendNext(color.r);
			stream.SendNext(color.g);
			stream.SendNext(color.b);
			stream.SendNext(color.a);
			stream.SendNext(Health);
		}
		else if (stream.IsReading)
		{
			ObjPosition = (Vector2)stream.ReceiveNext();
			ObjRotation = (Quaternion)stream.ReceiveNext();
			ObjScale = (Vector2)stream.ReceiveNext();
			username.text = (string)stream.ReceiveNext();
			color.r = (float)stream.ReceiveNext();
			color.g = (float)stream.ReceiveNext();
			color.b = (float)stream.ReceiveNext();
			color.a = (float)stream.ReceiveNext();
			Health = (float)stream.ReceiveNext();
		}
	}
}

public static class Helper
{
	public static GameObject FindInChildren(this GameObject go, string name)
	{
		return (from x in go.GetComponentsInChildren<Transform>()
				where x.gameObject.name == name
				select x.gameObject).First();
	}
}