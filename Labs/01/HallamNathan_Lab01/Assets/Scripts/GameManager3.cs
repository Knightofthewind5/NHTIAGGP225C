using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance { get; private set; }

    public GameObject playerPrefab;
    public List<GameObject> spawnpoints;
    int numOfSpawns = 0;

    public void Start()
    {
        Debug.Log("[GameManager3][Start]");

        numOfSpawns = spawnpoints.Count;

        Instance = this;
    }

    public void OnLoadLevel()
    {
        if (playerPrefab)
        {
            Debug.Log("Spawning Player");
            PhotonNetwork.Instantiate(playerPrefab.name, spawnpoints[Random.Range(0, numOfSpawns)].transform.position, spawnpoints[Random.Range(0, numOfSpawns)].transform.rotation);
        }
        else
        {
            Debug.Log("[GameManager3][Start](playerPrefab) No prefab set");
        }
    }
}