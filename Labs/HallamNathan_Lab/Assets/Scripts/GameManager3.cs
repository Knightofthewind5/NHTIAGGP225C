using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager3 : MonoBehaviourPunCallbacks, IPunObservable
{
	public static GameManager3 Instance { get; private set; }

	public GameObject playerPrefab;
	public List<GameObject> spawnpoints;
	int numOfSpawns = 0;
	PhotonView photonView;

	public float gameTime = 60f;
	float timer;

	public AudioSource music;
	float musicTime;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{

		}
		else if (stream.IsReading)
		{

		}
	}

	public void Awake()
	{
		Debug.Log("[GameManager3][Awake]");

		timer = gameTime;

		numOfSpawns = spawnpoints.Count;

		Instance = this;

		Instance.photonView = GetComponent<PhotonView>();
	}

	public void Start()
	{
		Debug.Log("[GameManager3][Start]");	
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		SpawnPlayer();
	}

	public void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			timer = 0;

			PhotonNetwork.DestroyAll();
			PhotonNetwork.LeaveRoom();
		}
	}

	void SpawnPlayer()
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