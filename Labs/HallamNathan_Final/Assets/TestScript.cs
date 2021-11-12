using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;


public class TestScript : MonoBehaviour
{
	[SerializeField] GameObject PlayerPrefab;

	private void Awake()
	{
		StartCoroutine(WaitTimer());
	}

	IEnumerator WaitTimer()
	{
		yield return new WaitForSecondsRealtime(2);

		SpawnPlayer();
	}

	void SpawnPlayer()
	{
		PhotonNetwork.Instantiate(PlayerPrefab.name, Vector2.zero, Quaternion.identity);
	}
}
