using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject[] Entity;
	public Transform[] spawnSpots;
	float timeBtwSpawns;
	float startTimeBtwSpawns = 10;
	GameObject spawn;
	GameObject GM;
	Manager GameManager;

	public void Awake()
	{
		GM = GameObject.FindGameObjectWithTag("Manager");
		GameManager = GM.GetComponent<Manager>();
	}

	private void Start()
	{
		timeBtwSpawns = startTimeBtwSpawns;
	}

	private void Update()
	{
		startTimeBtwSpawns = GameManager.currentTimeBtwSpawns;

		if (timeBtwSpawns <= 0 && GameManager.currentCount < GameManager.maxAsteroids)
		{
			var spawnIndex = Random.Range(0,Entity.Length);
			spawn = Entity[spawnIndex];

			int randPos = Random.Range(0, spawnSpots.Length);

			GameObject go = Instantiate(spawn, spawnSpots[randPos].position, Quaternion.identity);

			go.transform.Rotate(0, 0, Random.Range(0f, 360f));

			timeBtwSpawns = startTimeBtwSpawns;
		}
		else
		{
			timeBtwSpawns -= Time.deltaTime;
		}
	}
}
