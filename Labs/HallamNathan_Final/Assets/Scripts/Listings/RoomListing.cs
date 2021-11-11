using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField] TMP_Text _roomName;
    [SerializeField] TMP_Text _roomPlayerCount;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomName.text = roomInfo.Name;
        _roomPlayerCount.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;

        gameObject.GetComponent<Button>().onClick.AddListener(delegate { PhotonManager.Instance.JoinRoom(_roomName.text); });
    }
}
