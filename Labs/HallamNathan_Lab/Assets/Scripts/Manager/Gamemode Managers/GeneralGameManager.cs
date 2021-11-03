using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GeneralGameManager : MonoBehaviour
{
	public static GeneralGameManager Instance { get; private set; }
	public int playersAlive = 0;
	bool listUpdated = false;

	public GameObject playerPrefab;
	public List<GameObject> spawnpoints;
	int numOfSpawns = 0;
	public PhotonView photonView { get; private set; }
	public CanvasGroup spawnCanvas;
	public TMP_Text spawnTimerText;
	public CanvasGroup playCanvas;
	public CanvasGroup Chat;
	public CanvasGroup WinCanvas;
	public TMP_Text WinText;
	public Image HealthBar;
	public TMP_Text GameTime;
	public bool hasSpawned;
	public List<GameObject> players = new List<GameObject>();
	protected bool gameDone = false;
	public int spawnTime = 5;
	
	public float gameTime = 60f;
	protected float timer;

	public AudioSource music;

	public virtual void Awake()
	{
		Instance = this;

		Instance.photonView = GetComponent<PhotonView>();

		timer = gameTime + spawnTime;

		numOfSpawns = spawnpoints.Count;
	}

	public virtual void Start()
	{
		StartCoroutine(SpawnWaitTimer());
	}

	public IEnumerator SpawnWaitTimer()
	{
		for (int i = spawnTime; i > 0; i--)
		{
			spawnTimerText.text = "Spawning in " + i;
			yield return new WaitForSecondsRealtime(1);
		}

		SpawnPlayer();
	}

	public virtual void LateUpdate()
	{
		timer -= Time.deltaTime;

		if (timer <= 0 && gameDone)
		{
			timer = 0;

			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.AutomaticallySyncScene = true;
				SceneManager.LoadScene("FPSLobby");
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

		if (playersAlive == PhotonNetwork.CurrentRoom.PlayerCount && listUpdated == false)
		{
			AddPlayersToAliveList();
		}

		if (playersAlive != PhotonNetwork.CurrentRoom.PlayerCount)
		{
			listUpdated = false;
		}
	}

	void AddPlayersToAliveList()
	{
		PhotonView[] views = null;
		views = FindObjectsOfType<PhotonView>();

		foreach (PhotonView view in views)
		{
			if (view.gameObject.GetComponent<PlayerManager3>())
			{
				players.Add(view.gameObject);
			}
		}

		listUpdated = true;
	}	

	public void SpawnPlayer()
	{
		hasSpawned = true;
		playCanvas.SetActive(true);
		spawnCanvas.SetActive(false);

		if (playerPrefab)
		{
			GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnpoints[Random.Range(0, numOfSpawns)].transform.position, spawnpoints[Random.Range(0, numOfSpawns)].transform.rotation);

			Instance.photonView.RPC("RPCAddPlayerCount", RpcTarget.All);	
		}
		else
		{
			Debug.Log("[GameManager3][Start](playerPrefab) No prefab set");
		}
	}

	[PunRPC]
	public void RPCAddPlayerCount()
	{
		playersAlive++;
	}
}
