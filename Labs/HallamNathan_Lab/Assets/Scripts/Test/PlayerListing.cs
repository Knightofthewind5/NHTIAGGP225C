using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListing : MonoBehaviour
{
	public Player Player { get; private set; }
	[SerializeField] private TMP_Text _user;
	public string username { get; private set; }
	[SerializeField] Color _color;
	private PhotonView _photonView;
	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

	private void Awake()
	{
		_photonView = this.gameObject.GetPhotonView();
	}

	private void Update()
	{
		_user.color = _color;
		gameObject.name = username;
	}

	public void SetPlayerInfo(Player player)
	{
		Player = player;
		username = player.NickName;
		_user.text = username;

		properties = player.CustomProperties;

		_color = new Color((float)properties["colorRed"], (float)properties["colorGreen"], (float)properties["colorBlue"], (float)properties["colorAlpha"]);
	}
}
