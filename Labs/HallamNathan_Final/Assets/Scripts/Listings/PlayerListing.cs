using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListing : MonoBehaviour
{
	public Player Player { get; private set; }
	public string username { get; private set; }
	[SerializeField] private TMP_Text _user;
	[SerializeField] Color _color;
	private PhotonView _photonView;

	private void Awake()
	{
		_photonView = gameObject.GetPhotonView();
	}

	private void Update()
	{
		if (_user.color != _color)
		{
			_user.color = _color;
			if (PhotonNetwork.LocalPlayer.IsLocal && PhotonNetwork.LocalPlayer.NickName == username)
			{
				if (!_user.text.Contains("(You)"))
				{
					_user.text = "(You) " + _user.text;
				}		
			}

			if (Player.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId)
			{
				if (!_user.text.Contains("[Host]"))
				{
					_user.text = "[Host] " + _user.text;
				}
			}
		}
		if (gameObject.name != "(" + username + ") PlayerListing")
		{
			gameObject.name = "(" + username + ") PlayerListing";
		}
	}

	public void SetPlayerInfo(Player player)
	{
		Player = player;
		username = player.NickName;
		_user.text = username;

		_color = new Color((float)player.CustomProperties["colorRed"],
							(float)player.CustomProperties["colorGreen"],
							(float)player.CustomProperties["colorBlue"],
							(float)player.CustomProperties["colorAlpha"]);

		player.CustomProperties["shuttleName"] = "Vintage";
	}
}
