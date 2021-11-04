using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MainMenuButtonManager : MonoBehaviourPunCallbacks
{
    //Singleton 
    public static MainMenuButtonManager Instance { get; private set; }

    public Button SingleplayerButton;
    public Button MultiplayerButton;
    public Button OptionsButton;

    [SerializeField] CanvasGroup MainCanvas;
    [SerializeField] CanvasGroup OptionsCanvas;
    [SerializeField] CanvasGroup MultiplayerCanvas;
    [SerializeField] Transform RoomList;
    [SerializeField] RoomListing RoomObjectPrefab;

    private List<RoomInfo> _roomList = new List<RoomInfo>();

    public void Awake()
    {
        Instance = this;
    }

    public void SinglePlayer_Click()
    {
        PhotonManager.Instance.CreateRoom();
    }

    public void ToggleMultiplayer()
    {
        if (MultiplayerCanvas.activeInHierarchy())
        {
            MultiplayerCanvas.SetActive(false);
            MainCanvas.SetActive(true);
        }
        else
        {
            MultiplayerCanvas.SetActive(true);
            MainCanvas.SetActive(false);
        }
    }


    public void ToggleOptions()
    {
        if (OptionsCanvas.activeInHierarchy())
        {
            OptionsCanvas.SetActive(false);
            MainCanvas.SetActive(true);
        }
        else
        {
            OptionsCanvas.SetActive(true);
            MainCanvas.SetActive(false);
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;


        Debug.Log("[MainMenuButtonManager][UpdateRoomList] Room Count " + PhotonNetwork.CountOfRooms);
        ClearRoomList();


        foreach (RoomInfo roominfo in _roomList)
        {
            RoomListing RoomObject = Instantiate(RoomObjectPrefab, RoomList);

            if (RoomObject != null)
            {
                RoomObject.SetRoomInfo(roominfo);
            }
        }

        base.OnRoomListUpdate(_roomList);
    }

    void ClearRoomList()
    {
        for (int i = 0; i < RoomList.childCount; i++)
        {
            Destroy(RoomList.GetChild(0).gameObject);
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
