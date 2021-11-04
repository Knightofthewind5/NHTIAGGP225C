using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Text;

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
	[SerializeField] Transform _RoomList;
	[SerializeField] RoomListing _RoomObjectPrefab;
	[SerializeField] TMP_InputField _username;
	[SerializeField] TMP_InputField _roomName;
	[SerializeField] TMP_InputField _playerCount;

	private List<RoomInfo> _roomList = new List<RoomInfo>();

	public void Awake()
	{
		Instance = this;
	}

	public void SinglePlayer_Click()
	{
		
	}

	public void CreateRoom()
	{
		if (_username.text.Length != 0)
		{
			PhotonManager.Instance.username = _username.text;
			PhotonManager.Instance.CreateRoom(_roomName.text, (byte)int.Parse(_playerCount.text));
		}
		else
		{
			Debug.LogError("No Username Detected! Can not create room");
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
			_OptionsCanvas.SetActive(true);
			_MainCanvas.SetActive(false);
		}
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		_roomList = roomList;


		Debug.Log("[MainMenuButtonManager][UpdateRoomList] Room Count " + PhotonNetwork.CountOfRooms);
		ClearRoomList();


		foreach (RoomInfo roominfo in _roomList)
		{
			RoomListing RoomObject = Instantiate(_RoomObjectPrefab, _RoomList);

			if (RoomObject != null)
			{
				RoomObject.SetRoomInfo(roominfo);
			}
		}

		base.OnRoomListUpdate(_roomList);
	}

	void ClearRoomList()
	{
		for (int i = 0; i < _RoomList.childCount; i++)
		{
			Destroy(_RoomList.GetChild(0).gameObject);
		}
	}

	public void Quit_Click()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif

		Application.Quit();
	}
}
