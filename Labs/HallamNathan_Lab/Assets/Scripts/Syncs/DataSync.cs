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

	public TMP_Text username;
	public Image healthbar;
	float fillAmount;
	GeneralGameManager GGM;
	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

	private string lastTookDamageFrom;

	private void Awake()
	{
		GGM = FindObjectOfType<GeneralGameManager>();

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
					Debug.Log(gameObject.GetPhotonView().Owner.NickName + " died from " + lastTookDamageFrom);

					Health = MinHealth;

					if (GGM is BulletRoundManager)
					{
						Debug.Log("GMM is BulletRoundManager");

						GGM.hasSpawned = false;
						GGM.spawnCanvas.SetActive(true);
						GGM.spawnTimerText.fontSize = 50;
						GGM.spawnTimerText.text = "You are dead! You will respawn next game";
						GGM.playCanvas.SetActive(false);
						PhotonManager.Instance.photonView.RPC("RemoveFromAlivePlayers", RpcTarget.All);
						PhotonNetwork.Destroy(gameObject);
					}
					else if (GGM is DeathmatchManager)
					{
						Debug.Log("GMM is DeathmatchManager");

						GGM.hasSpawned = false;
						GGM.spawnCanvas.SetActive(true);
						GGM.playCanvas.SetActive(false);
						
						DeathmatchManager.Instance.photonView.RPC("RPCAddToScore", RpcTarget.All, lastTookDamageFrom);

						GGM.players.Remove(gameObject);
						PhotonManager.Instance.photonView.RPC("RemoveFromAlivePlayers", RpcTarget.All);
						PhotonNetwork.Destroy(gameObject);
					}
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

	public void TakeDamage(float amount, string dealer)
	{
		if (!photonView.IsMine)
		{
			if (dealer != null)
			{
				Debug.Log(gameObject.GetPhotonView().Owner.NickName + " Damage Taken from " + dealer);
				lastTookDamageFrom = dealer;
			}

			photonView.RPC("Damage", RpcTarget.AllBuffered, amount);
		}
		else
		{
			if (dealer != null)
			{
				lastTookDamageFrom = dealer;
			}

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
			if (lastTookDamageFrom != null)
			{
				stream.SendNext(lastTookDamageFrom);
			}

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
			string temp = (string)stream.ReceiveNext();
			if (temp != null)
			{
				lastTookDamageFrom = temp;

				if (camera)
				{
					CameraRotation = (Quaternion)stream.ReceiveNext();
				}
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