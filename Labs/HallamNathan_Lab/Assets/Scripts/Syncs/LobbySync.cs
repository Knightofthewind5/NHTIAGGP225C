using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class LobbySync : MonoBehaviourPun, IPunObservable
{
	public Transform parentTransform;
	public Color color;

	[SerializeField] TMP_Text userText;
	public string username;

	private void Awake()
	{
		if (gameObject.GetPhotonView().IsMine)
		{
			userText.text = username;
			color.r = PlayerPrefs.GetFloat("colorRed");
			color.g = PlayerPrefs.GetFloat("colorGreen");
			color.b = PlayerPrefs.GetFloat("colorBlue");
			color.a = PlayerPrefs.GetFloat("colorAlpha");
		}
	}

	private void Start()
	{
		parentTransform = FindObjectOfType<GameplayManager>().playerListHolder.transform;
	}

	private void Update()
	{
		if (!photonView.IsMine)
		{
			if (parentTransform)
			{
				gameObject.transform.SetParent(parentTransform);
				userText.color = color;
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(userText.text);
			stream.SendNext(color.r);
			stream.SendNext(color.g);
			stream.SendNext(color.b);
			stream.SendNext(color.a);
		}
		else if (stream.IsReading)
		{
			userText.text = (string)stream.ReceiveNext();
			color.r = (float)stream.ReceiveNext();
			color.g = (float)stream.ReceiveNext();
			color.b = (float)stream.ReceiveNext();
			color.a = (float)stream.ReceiveNext();
		}
	}
}
