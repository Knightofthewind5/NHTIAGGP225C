using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DataSync : MonoBehaviourPun, IPunObservable
{
	public Vector3 ObjPosition;
	public Quaternion ObjRotation;
	public Quaternion CameraRotation;
	public Vector3 ObjScale;
	public Color color;

	public float LerpSpeed = 3f;

	public float Health;
	public float MaxHealth;
	public float MinHealth;

	Camera camera;
	int cosmeticIndex;
	string cosmeticName;
	public GameObject cosmetic;
	public GameObject[] cosmetics;
	//bool cosmeticActive = false;

	public TMP_Text username;
	public Image healthbar;
	float fillAmount;
	GameManager3 GM;
	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();



	private void Awake()
	{
		GM = FindObjectOfType<GameManager3>();

		if (photonView.IsMine)
		{
			properties = PhotonNetwork.LocalPlayer.CustomProperties;

			camera = GetComponentInChildren<Camera>();
			username.text = PhotonNetwork.LocalPlayer.NickName;
			color = new Color((float)properties["colorRed"], (float)properties["colorGreen"], (float)properties["colorBlue"], (float)properties["colorAlpha"]);
			cosmeticIndex = PlayerPrefs.GetInt("Cosmetic");
		}
	}

	void Start()
	{
		if (photonView.IsMine)
		{
			if (cosmeticIndex == 0)
			{
				cosmeticName = "TopHat";
			}
			else if (cosmeticIndex == 1)
			{
				cosmeticName = "Monocle";
			}
			else if (cosmeticIndex == 2)
			{
				cosmeticName = "Tie";
			}

			gameObject.GetPhotonView().RPC("SetActiveCosmetic", RpcTarget.AllBuffered, gameObject.GetPhotonView().ViewID, cosmeticName);
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
				else if (Health <= MinHealth)
				{
					Health = MinHealth;
					GM.hasSpawned = false;
					GM.spawnCanvas.SetActive(true);
					GM.spawnCanvas.GetComponentInChildren<Button>().interactable = false;
					GM.playCanvas.SetActive(false);
					PhotonManager.Instance.photonView.RPC("RemoveFromAlivePlayers", RpcTarget.All);
					PhotonNetwork.Destroy(gameObject);
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

		if (camera)
		{
			camera.gameObject.transform.rotation = Quaternion.Lerp(camera.gameObject.transform.rotation, CameraRotation, LerpSpeed * Time.deltaTime);
		}
	}

	public void TakeDamage(float amount)
	{
		Debug.Log(gameObject.name + " Attempt Damage");

		if (!photonView.IsMine)
		{
			Debug.Log(gameObject.name + " Damage Taken");

			photonView.RPC("Damage", RpcTarget.AllViaServer, amount);
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
			stream.SendNext(cosmeticName);

			if (camera)
			{
				stream.SendNext(camera.gameObject.transform.rotation);
			}
		}
		else if (stream.IsReading)
		{
			ObjPosition = (Vector3)stream.ReceiveNext();
			ObjRotation = (Quaternion)stream.ReceiveNext();
			ObjScale = (Vector3)stream.ReceiveNext();
			username.text = (string)stream.ReceiveNext();
			color.r = (float)stream.ReceiveNext();
			color.g = (float)stream.ReceiveNext();
			color.b = (float)stream.ReceiveNext();
			color.a = (float)stream.ReceiveNext();
			Health = (float)stream.ReceiveNext();
			cosmeticName = (string)stream.ReceiveNext();

			if (camera)
			{
				CameraRotation = (Quaternion)stream.ReceiveNext();
			}
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