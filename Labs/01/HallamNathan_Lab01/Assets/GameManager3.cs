using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager3 : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<GameObject> spawnpoints;
    int numOfSpawns = 0;

    public void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("FPSScene");

            return;
        }

        numOfSpawns = spawnpoints.Count;
        

        if (playerPrefab)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, spawnpoints[Random.Range(0, numOfSpawns)].transform.position, spawnpoints[Random.Range(0, numOfSpawns)].transform.rotation);
        }
        else
        {
            Debug.Log("[GameManager3][Start](playerPrefab) No prefab set");
        }
    }
}