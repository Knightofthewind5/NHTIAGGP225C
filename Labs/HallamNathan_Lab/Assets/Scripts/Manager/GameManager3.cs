using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager3 : MonoBehaviourPunCallbacks
{
	public static GameManager3 Instance { get; private set; }

	public GameObject playerPrefab;
	public List<GameObject> spawnpoints;
	int numOfSpawns = 0;
	PhotonView photonView;
	public CanvasGroup spawnCanvas;
	public CanvasGroup playCanvas;
	public CanvasGroup Chat;
	public CanvasGroup WinCanvas;
	public TMP_Text WinText;
	public Image HealthBar;
	public TMP_Text GameTime;
	public bool hasSpawned;
	public List<GameObject> players = new List<GameObject>();
	public int playersAlive = 0;
	bool gameDone = false;

	public float gameTime = 60f;
	float timer;

	public AudioSource music;
	float musicTime;

	public void Awake()
	{
		Debug.Log("[GameManager3][Awake]");

		timer = gameTime;

		numOfSpawns = spawnpoints.Count;

		Instance = this;

		Instance.photonView = GetComponent<PhotonView>();

		playersAlive = PhotonNetwork.CurrentRoom.PlayerCount;
	}

	public void Start()
	{
		Debug.Log("[GameManager3][Start]");	
	}


	public void LateUpdate()
	{
		if (playersAlive == 1 && !gameDone)
		{
			gameDone = true;
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			DataSync DS = player.GetComponent<DataSync>();
			WinText.text = "Winner: " + DS.username.text;
			WinCanvas.SetActive(true);
			timer = 5f;
		}


		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			timer = 0;

			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.DestroyAll();
				SceneManager.LoadScene("FPSLobby");
			}
			else
			{
				PhotonNetwork.LoadLevel("FPSLobby");
			}
		}	

		if (PhotonNetwork.IsMasterClient)
		{
			PhotonManager.Instance.photonView.RPC("SetGameHUDTimer", RpcTarget.All, timer);
		}

		if (photonView.IsMine) // Open close chat window
		{
			//Debug.Log("View is Mine " + PlayerPrefs.GetString("Username"));
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				if (Chat.IsActive())
				{
					Chat.SetActive(false);
				}
				else
				{
					Chat.SetActive(true);
				}
			}
		}
	}

	public void SpawnPlayer()
	{
		hasSpawned = true;
		spawnCanvas.SetActive(false);
		playCanvas.SetActive(true);

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