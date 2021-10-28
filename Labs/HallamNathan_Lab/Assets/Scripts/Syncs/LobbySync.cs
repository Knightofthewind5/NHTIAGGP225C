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
	public string username;

	string Username;
	Color PlayerColor;


	public TMP_Text userText;

	private void Awake()
	{
		parentTransform = FindObjectOfType<GameplayManager>().playerListHolder.transform;
		if (photonView.IsMine)
		{
			username = PhotonManager.Instance.username;
			color.r = PlayerPrefs.GetFloat("colorRed");
			color.g = PlayerPrefs.GetFloat("colorGreen");
			color.b = PlayerPrefs.GetFloat("colorBlue");
			color.a = PlayerPrefs.GetFloat("colorAlpha");
		}
	}

	private void Update()
	{
		if (!photonView.IsMine)
		{
			gameObject.transform.SetParent(parentTransform);
			userText.color = color;
			userText.text = username;
			gameObject.name = username;		
		}
		else
        {
			userText.color = color;
			userText.text = username;
			gameObject.name = username;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(username);
			stream.SendNext(color.r);
			stream.SendNext(color.g);
			stream.SendNext(color.b);
			stream.SendNext(color.a);
		}
		else if (stream.IsReading)
		{
			username = (string)stream.ReceiveNext();
			color.r = (float)stream.ReceiveNext();
			color.g = (float)stream.ReceiveNext();
			color.b = (float)stream.ReceiveNext();
			color.a = (float)stream.ReceiveNext();
		}
	}
}
