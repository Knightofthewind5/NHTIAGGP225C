using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class GameLobbyManager : MonoBehaviourPunCallbacks
{
	public static GameLobbyManager Instance;

	[SerializeField] Transform _content;
	[SerializeField] PlayerListing _playerListing;
	[SerializeField] TMP_Text _playerCounter;
	[SerializeField] TMP_Text _gameHost;
	[SerializeField] TMP_Text _roomName;
	[SerializeField] CanvasGroup _masterClientSettingsGroup;
	public TMP_Text _lobbyStatus;
	public float gameStartTimer = 5f;
	public float timer { get; private set; }
	bool gameStarting = false;

	public List<PlayerListing> lobbyListing { get; private set; } = new List<PlayerListing>();

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		StartCoroutine(RoomWaitBuffer());
	}

	private void Update()
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			if (PhotonNetwork.CurrentRoom.MasterClientId == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				if (!gameStarting && timer != gameStartTimer)
				{
					timer = gameStartTimer;

					string message = "Waiting for Host...";

					RPCManager.Instance.gameObject.GetPhotonView().RPC("SetLobbyTimerRPC", RpcTarget.All, message);
				}
				else if (gameStarting)
				{
					timer -= Time.deltaTime;

					string message = "Game starts in " + timer.ToString("F2");

					RPCManager.Instance.gameObject.GetPhotonView().RPC("SetLobbyTimerRPC", RpcTarget.All, message);
				}
			}
		}

		if (timer <= 0 && gameStarting)
		{
			gameStarting = false;
			timer = 0;
			PhotonNetwork.LoadLevel("GameScene");
		}
	}

	IEnumerator RoomWaitBuffer()
	{
		yield return new WaitUntil(() => PhotonNetwork.InRoom == true);

		GetCurrentRoomPlayers();
	}

	IEnumerator TextSizeBuffer(PlayerListing listing)
	{
		ContentSizeFitter CSF = listing.gameObject.GetComponent<ContentSizeFitter>();

		listing.gameObject.SetActive(false);
		CSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		yield return new WaitForSeconds(0.1f);

		listing.gameObject.SetActive(true);

		listing.gameObject.SetActive(false);

		yield return new WaitForSecondsRealtime(Random.Range(0f, 0.5f));

		listing.gameObject.SetActive(true);

		UpdateLobbyInfo();
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
		//Spawn player list object
		PlayerListing listing = Instantiate(_playerListing, _content);

		//If the list object is not null
		if (listing != null)
		{
			//Get the list objects photon view
			PhotonView listingPV = listing.GetComponent<PhotonView>();

			//Set the controller of the view to the joined player
			listingPV.ControllerActorNr = player.ActorNumber;

			//Set the players info in the list object
			listing.SetPlayerInfo(player);

			//Add the player to the rooms internal list of players
			lobbyListing.Add(listing);

			StartCoroutine(TextSizeBuffer(listing));
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		AddPlayerListing(newPlayer);

		#region Send Connected Message
		// Get player name from properties
		string playerName = newPlayer.NickName;
		string message = "(Connected)";

		//Get player color from properties
		Color32 playerColor = new Color((float)newPlayer.CustomProperties["colorRed"], (float)newPlayer.CustomProperties["colorGreen"], (float)newPlayer.CustomProperties["colorBlue"], (float)newPlayer.CustomProperties["colorAlpha"]);

		//Change player name color in chat to the chosen color
		playerName = "<color=#" + playerColor.ColorToHex() + ">" + playerName + "</color>";
		message = "<color=#" + new Color32(0, 255, 0, 255).ColorToHex() + ">" + message + "</color>";

		ChatManager.Instance.ChatOnConnectDisconnect(playerName, message);
		#endregion Send Connected Message
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		int index = lobbyListing.FindIndex(x => x.Player == otherPlayer);
		if (index != -1)
		{
			//RPCChat.Instance.chatRoomString.text += "\n" + "(Disconnected)" + _listings[index].gameObject.GetPhotonView().Controller.NickName;

			Destroy(lobbyListing[index].gameObject);
			lobbyListing.RemoveAt(index);
		}

		#region Send Disconnected Message
		// Get player name from properties
		string playerName = otherPlayer.NickName;
		string message = "(Disconnected)";

		//Get player color from properties
		Color32 playerColor = new Color((float)otherPlayer.CustomProperties["colorRed"], (float)otherPlayer.CustomProperties["colorGreen"], (float)otherPlayer.CustomProperties["colorBlue"], (float)otherPlayer.CustomProperties["colorAlpha"]);

		//Change player name color in chat to the chosen color
		playerName = "<color=#" + playerColor.ColorToHex() + ">" + playerName + "</color>";
		message = "<color=#" + new Color32(255, 0, 0, 255).ColorToHex() + ">" + message + "</color>";

		ChatManager.Instance.ChatOnConnectDisconnect(playerName, message);
		#endregion Send Disconnected Message

		UpdateLobbyInfo();
	}

	void UpdateLobbyInfo()
	{
		if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId)
		{
			_masterClientSettingsGroup.SetActive(true);
		}

		_playerCounter.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
		_gameHost.text = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId).NickName.ToString();
		_roomName.text = PhotonNetwork.CurrentRoom.Name;

		foreach (PlayerListing listing in lobbyListing)
		{
			if (listing.gameObject.GetPhotonView().ControllerActorNr == PhotonNetwork.CurrentRoom.MasterClientId)
			{
				listing.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 0);
			}
		}
	}

	public void LeaveRoom()
	{
		if (PhotonNetwork.OfflineMode)
		{
			PhotonNetwork.Disconnect();

			PhotonManager.Instance.Connect();
		}
		else
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	public void StartGame()
	{
		if (gameStarting)
		{
			gameStarting = false;
		}
		else
		{
			gameStarting = true;
		}
	}
}
