using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviourPunCallbacks
{
	//Singleton 
	public static MainMenuButtonManager Instance { get; private set; }

	public Button SingleplayerButton;
	public Button MultiplayerButton;
	public Button OptionsButton;

	[SerializeField] CanvasGroup _MainCanvas;
	[SerializeField] CanvasGroup _OptionsCanvas;
	[SerializeField] CanvasGroup _MultiplayerCanvas;
	[SerializeField] Transform _content;
	[SerializeField] RoomListing _roomListing;
	[SerializeField] TMP_InputField _username;
	[SerializeField] TMP_InputField _roomName;
	[SerializeField] TMP_InputField _playerCount;
	[SerializeField] Image _chosenColor;
	[SerializeField] Button _btn_createRoom;
	[SerializeField] TMP_Text _roomCount;
	[SerializeField] TMP_Text _totalPlayersInRoomCount;
	private ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();

	private List<RoomListing> _listing = new List<RoomListing>();

	public void Awake()
	{
		Instance = this;

		StartCoroutine(WaitForConnection());
	}

	public void Update()
	{
		if (_username.text.Length > 0)
		{
			_btn_createRoom.interactable = true;
		}
		else
		{
			_btn_createRoom.interactable = false;
		}
	}

	IEnumerator WaitForConnection()
	{
		yield return new WaitUntil(() => PhotonNetwork.InLobby);

		_username.text = PhotonManager.Instance.username;
		_chosenColor.color = PhotonManager.Instance.color;
	}

	IEnumerator JoinSingleplayer()
	{
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();

			yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
		}

		PhotonNetwork.OfflineMode = true;

		yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

		yield return new WaitUntil(() => PhotonNetwork.CreateRoom("Singleplayer", new RoomOptions { MaxPlayers = 1 }, null));

		PhotonNetwork.LocalPlayer.NickName = "Player";

		properties["colorRed"] = 1f;
		properties["colorGreen"] = 1f;
		properties["colorBlue"] = 1f;
		properties["colorAlpha"] = 1f;

		PhotonNetwork.LocalPlayer.CustomProperties = properties;

		PhotonNetwork.LoadLevel("GameLobby");
	}

	public void SinglePlayer_Click()
	{
		StartCoroutine(JoinSingleplayer());
	}

	public void CreateRoom()
	{
		if (_username.text.Length != 0)
		{
			PhotonManager.Instance.username = _username.text;

			if (_roomName.text.Length != 0 && _playerCount.text.Length == 0)
			{
				PhotonManager.Instance.CreateRoom(_roomName.text);

			}
			else if (_roomName.text.Length == 0 && _playerCount.text.Length != 0)
			{
				PhotonManager.Instance.CreateRoom("Default Roomname", (byte)int.Parse(_playerCount.text));

			}
			else if (_roomName.text.Length == 0 && _playerCount.text.Length == 0)
			{
				PhotonManager.Instance.CreateRoom();

			}
			else
			{
				PhotonManager.Instance.CreateRoom(_roomName.text, (byte)int.Parse(_playerCount.text));
			}

			SceneManager.LoadScene("GameLobby");
		}
	}

	public void ToggleMultiplayer()
	{
		if (_MultiplayerCanvas.activeInHierarchy())
		{
			_MultiplayerCanvas.SetActive(false);
			_MainCanvas.SetActive(true);
		}
		else
		{
			_MultiplayerCanvas.SetActive(true);
			_MainCanvas.SetActive(false);
		}
	}

	public void ToggleOptions()
	{
		if (_OptionsCanvas.activeInHierarchy())
		{
			_OptionsCanvas.SetActive(false);
			_MainCanvas.SetActive(true);
		}
		else
		{
			_MainCanvas.SetActive(false);
			_OptionsCanvas.SetActive(true);			
		}
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		ClearRoomList();

		foreach (RoomInfo info in roomList)
		{
			if (info.RemovedFromList)
			{
				int index = _listing.FindIndex(x => x.RoomInfo.Name == info.Name);
				if (index != -1)
				{
					Destroy(_listing[index].gameObject);
					_listing.RemoveAt(index);
				}
			}
			else
			{
				RoomListing listing = Instantiate(_roomListing, _content);

				if (listing != null)
				{
					listing.SetRoomInfo(info);
					_listing.Add(listing);
				}
			}
		}
	}

	void ClearRoomList()
	{
		for (int i = 0; i < _content.childCount; i++)
		{
			Destroy(_content.GetChild(0).gameObject);
			_listing.RemoveAt(0);
		}

		_roomCount.text = PhotonNetwork.CountOfRooms.ToString();
		_totalPlayersInRoomCount.text = PhotonNetwork.CountOfPlayersInRooms.ToString();
	}

	public void Quit_Click()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif

		Application.Quit();
	}
}
