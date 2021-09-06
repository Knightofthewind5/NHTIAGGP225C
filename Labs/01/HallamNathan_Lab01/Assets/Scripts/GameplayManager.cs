using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;

    public void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("SampleScene");

            return;
        }

        if (playerPrefab)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.Log("[GameplayManager][Start](playerPrefab) No prefab set");
        }
    }
}
