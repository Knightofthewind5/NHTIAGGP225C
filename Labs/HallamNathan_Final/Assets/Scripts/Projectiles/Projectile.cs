using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Projectile : MonoBehaviour
{
	private const int UPDATE_HEALTH = 0;
	public string owner;
	public float damage;
	public string ID;
	public float HP;

	private void Start()
	{
		ID = owner + ID;

		PhotonNetwork.NetworkingClient.EventReceived += this.UpdateProjectileHealth;
		HP = 1f;
	}

	private void UpdateProjectileHealth(EventData obj)
	{
		if (obj.Code == UPDATE_HEALTH)
		{
			object[] datas = (object[])obj.CustomData;

			if (datas[0].ToString() == ID)
			{
				HP = (float)datas[1];

				if (HP <= 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D Collision)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (Collision.otherCollider)
			{
				if (Collision.gameObject.transform.TryGetComponent(out Asteroid asteroid))
				{
					HP -= 1;

					object[] datas = new object[] { ID, HP };

					RaiseEventOptions REO = new RaiseEventOptions { Receivers = ReceiverGroup.All };

					PhotonNetwork.RaiseEvent(UPDATE_HEALTH, datas, REO, SendOptions.SendReliable);
				}
			}
		}
	}

	private void OnDestroy()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= this.UpdateProjectileHealth;
	}
}