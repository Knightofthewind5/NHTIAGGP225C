using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private Transform content;

	[SerializeField] private PlayerListing _playerListing;

	[SerializeField] private List<PlayerListing> _listings = new List<PlayerListing>();

	private void Start()
	{
		StartCoroutine(RoomWaitBuffer());	
	}

	IEnumerator RoomWaitBuffer()
	{
		yield return new WaitUntil(() => PhotonNetwork.InRoom == true);

		GetCurrentRoomPlayers();
	}

	IEnumerator TextSizeBuffer(PlayerListing listing)
	{
		listing.gameObject.SetActive(false);

		yield return new WaitForSecondsRealtime(Random.Range(0f, 0.5f));

		RPCChat.Instance.chatRoomString.text += "\n" + "(Connected)" + listing.gameObject.GetPhotonView().Controller.NickName;
		listing.gameObject.SetActive(true);
	}

	private void GetCurrentRoomPlayers()
	{
		foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
		{
			AddPlayerListing(playerInfo.Value);
		}
	}

	private void AddPlayerListing(Player player)
	{
		PlayerListing listing = Instantiate(_playerListing, content);

		if (listing != null)
		{
			PhotonView listingPV = listing.GetComponent<PhotonView>();

			listingPV.ControllerActorNr = player.ActorNumber;

			listing.SetPlayerInfo(player);

			if (!PhotonNetwork.IsMasterClient)
			{
				RPCChat.Instance.gamemodeDropdown.gameObject.SetActive(false);
			}

			_listings.Add(listing);

			StartCoroutine(TextSizeBuffer(listing));
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		AddPlayerListing(newPlayer);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		int index = _listings.FindIndex(x => x.Player == otherPlayer);
		if (index != -1)
		{
			RPCChat.Instance.chatRoomString.text += "\n" + "(Disconnected)" + _listings[index].gameObject.GetPhotonView().Controller.NickName;

			Destroy(_listings[index].gameObject);
			_listings.RemoveAt(index);
		}
	}
}
