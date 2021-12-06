using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;


public class AsteroidSpawner : MonoBehaviour
{
	[SerializeField] Vector2 noSpawnArea;
	[SerializeField] Vector2 spawnArea;
	[SerializeField] Vector2[] permitAreas = new Vector2[2];
	[SerializeField] Color spawnAreaColor;
	[SerializeField] Color noSpawnAreaColor;

	List<Vector2> edgePoints = new List<Vector2>();
	float x;
	float y;

	float horizontalEdge;
	float verticalEdge;

	int spawnObjectIndex = 0;

	public void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			#region Get Screen Bounds
			Vector3 horizontalOffset = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, -Camera.main.transform.position.z)); //Changed from v
			Vector3 verticalOffset = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.0f, -Camera.main.transform.position.z)); //
			horizontalEdge = horizontalOffset.x;
			verticalEdge = verticalOffset.y;
			#endregion Get Screen Bounds

			#region Add Screen Edges to list
			edgePoints.Add(new Vector2(horizontalEdge, verticalEdge));
			edgePoints.Add(new Vector2(horizontalEdge, -verticalEdge));
			edgePoints.Add(new Vector2(-horizontalEdge, -verticalEdge));
			edgePoints.Add(new Vector2(-horizontalEdge, verticalEdge));
			#endregion region Add Screen Edges to list

			StartCoroutine(GameWaitBeforeSpawn());
		}
	}

	void SpawnAsteroids()
	{
		int spawnLocationIndex = Random.Range(0, 4);
		int spawnObjectTopIndex = GameManager.Instance.asteroids.Count;

		foreach (AsteroidStats stat in GameManager.Instance.asteroids)
		{
			if (stat.weight > GameManager.Instance.availableWeight)
			{
				spawnObjectTopIndex--;
			}
		}

		spawnObjectIndex = Random.Range(1, spawnObjectTopIndex);

		switch (spawnLocationIndex)
		{
			case 0: // Right Side
				x = Random.Range((noSpawnArea.x / 2), (noSpawnArea.x / 2) + (permitAreas[0].x));
				y = Random.Range(-(permitAreas[0].y / 2), (permitAreas[0].y / 2));

				break;
			case 1: // Left Side
				x = Random.Range(-(noSpawnArea.x / 2), -(noSpawnArea.x / 2) - (permitAreas[0].x));
				y = Random.Range(-(permitAreas[0].y / 2), (permitAreas[0].y / 2));

				break;
			case 2: // Top Side
				x = Random.Range(-(permitAreas[1].x / 2), (permitAreas[1].x / 2));
				y = Random.Range((noSpawnArea.y / 2), (noSpawnArea.y / 2) + permitAreas[1].y);

				break;
			case 3: // Bottom Side
				x = Random.Range(-(permitAreas[1].x / 2), (permitAreas[1].x / 2));
				y = Random.Range(-(noSpawnArea.y / 2), -(noSpawnArea.y / 2) - permitAreas[1].y);

				break;
			default:
				break;
		}

		#region Sort Screen edges based on distance
		edgePoints.Sort(((x, y) => Vector2.Distance(gameObject.transform.position, x).
		CompareTo(Vector2.Distance(gameObject.transform.position, y))));
		#endregion Sort Screen edges based on distance

		#region Get available angle to move
		// Law of Cosines (a^2 + b^2 - c^2) / 2ab
		float angle = (Mathf.Pow(Vector2.Distance(gameObject.transform.position, edgePoints[0]), 2) +
					   Mathf.Pow(Vector2.Distance(gameObject.transform.position, edgePoints[1]), 2) -
					   Mathf.Pow((verticalEdge * 2), 2)) / (2 *
					   Vector2.Distance(gameObject.transform.position, edgePoints[0]) *
					   Vector2.Distance(gameObject.transform.position, edgePoints[1]));

		float arccos = Mathf.Acos(angle);
		float rad2degree = Mathf.Rad2Deg * arccos;
		#endregion Get available angle to move

		#region Make asteroid look towards origin and give it a slight angle offset
		Vector3 difference = new Vector3(0, 0, 0) - new Vector3(x, y, 0);
		float rotationZ = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - 90f;
		float newAngle = Random.Range((rotationZ - (angle / 3)), (rotationZ + (angle / 3)));
		#endregion Make asteroid look towards origin and give it a slight angle offset

		RPCManager.Instance.PV.RPC("SpawnAsteroidsRPC", RpcTarget.All, spawnObjectIndex, new Vector3(x, y, 0), newAngle, Random.Range(100000, 999999));
	}

	private void OnDrawGizmosSelected()
	{
		permitAreas[0] = new Vector2((spawnArea.x - noSpawnArea.x) / 2, spawnArea.y); // Right && Left
		permitAreas[1] = new Vector2(spawnArea.x, (spawnArea.y - noSpawnArea.y) / 2); // Top && Bottom

		Vector2 center;

		Gizmos.color = spawnAreaColor;
		center = new Vector2((noSpawnArea.x / 2) + (permitAreas[0].x / 2), 0); // Right
		Gizmos.DrawCube(center, permitAreas[0]);
		center = new Vector2((-noSpawnArea.x / 2) + (-permitAreas[0].x / 2), 0); // Left
		Gizmos.DrawCube(center, permitAreas[0]);
		center = new Vector2(0, (noSpawnArea.y / 4) + (permitAreas[0].y) / 4); // Top
		Gizmos.DrawCube(center, permitAreas[1]);
		center = new Vector2(0, (-noSpawnArea.y / 4) + (-permitAreas[0].y / 4)); // Bottom
		Gizmos.DrawCube(center, permitAreas[1]);

		Gizmos.color = noSpawnAreaColor;
		Gizmos.DrawCube(Vector2.zero, noSpawnArea);

		Gizmos.color = spawnAreaColor;
		Gizmos.DrawCube(Vector2.zero, spawnArea);
	}

	IEnumerator GameWaitBeforeSpawn()
	{
		yield return new WaitForSecondsRealtime(GameSettingsManager.Instance.asteroidSpawnWaitTime);

		StartCoroutine(SpawnTimer());
	}

	IEnumerator SpawnTimer()
	{
		yield return new WaitUntil(() => GameManager.Instance.availableWeight >= (0.75f * GameManager.Instance.maxWeightForLevel));

		do
		{
			SpawnAsteroids();

			yield return new WaitForSecondsRealtime(0.1f);
		}
		while (GameManager.Instance.availableWeight > 0);

		StartCoroutine(SpawnTimer());
	}
}
