using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class RPCManager : MonoBehaviour
{
	public static RPCManager Instance;
	public PhotonView PV;

	void Awake()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
			PV = gameObject.GetPhotonView();
		}
	}

	[PunRPC]
	public void UsernameRPC(string _username, string _chat)
	{
		ChatManager.Instance.chatRoomString.text += "\n" + _username + ": " + _chat;
		//Debug.Log(string.Format("ChatMessage {0} {1}", _username, _chat));
	}

	[PunRPC]
	public void SetLobbyTimerRPC(string _value)
	{
		GameLobbyManager.Instance._lobbyStatus.text = _value;
	}

	[PunRPC]
	public void SpawnAsteroidsRPC(int spawnIndex, Vector3 location, float angle, int ID)
	{
		GameObject spawn = Instantiate(GameManager.Instance.baseGameObject, location, Quaternion.identity);
		spawn.GetComponent<TestAsteroid>().ASs = new AsteroidStats(GameManager.Instance.asteroids[spawnIndex]);
		spawn.GetComponent<TestAsteroid>().ID = ID;

		spawn.transform.rotation = Quaternion.Euler(0, 0, angle);
	}

	[PunRPC]
	public void ShootProjectile(int viewID)
	{
		GameObject shooter = PhotonNetwork.GetPhotonView(viewID).gameObject;
		if (shooter.TryGetComponent(out PlayerController PC))
		{
			GameObject proj;
			WeaponStats WSS = PC.PrimaryWeapon;

			proj = Instantiate(WSS.projectilePrefab, PC.SpawnLocation.transform.position, PC.SpawnLocation.transform.rotation);
			Projectile script = proj.GetComponent<Projectile>();
			script.owner = PC.photonView.Owner.NickName;
			script.damage = WSS.damage;

			Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
			rb.velocity = PC.gameObject.transform.up * (WSS.speed + PC.rb.velocity.magnitude);
			Destroy(proj, WSS.lifetime);

			PC.AS.PlayOneShot(WSS.fireSound);
		}
	}

	[PunRPC]
	public void DestroyGameObject(int customID)
	{
		GameObject obj = GameObject.Find(customID.ToString());

		Destroy(obj);
	}
}