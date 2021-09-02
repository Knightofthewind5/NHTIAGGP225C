using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    public void JoinRoom()
    {
        PhotonManager.Instance.CreateRoom();
    }

    public void JoinRandom()
    {
        PhotonManager.Instance.JoinRandomRoom();
    }
}
