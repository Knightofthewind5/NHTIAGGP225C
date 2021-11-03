using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class projectileManager3 : MonoBehaviour
{
	public float damage = 2f;
	Rigidbody rb;
	bool hasDealtDamage = false;
	public string owner;

	public void Awake()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		rb.useGravity = false;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<PlayerManager3>())
		{
			DataSync ds = collision.gameObject.GetComponent<DataSync>();

			if (ds && PhotonNetwork.IsMasterClient)
			{
				if (!hasDealtDamage)
				{
					hasDealtDamage = true;
					ds.TakeDamage(damage, owner);
				}
			}
		}

		PhotonManager.Instance.gameObject.GetPhotonView().RPC("DestroyObject", RpcTarget.All, gameObject.GetInstanceID());
	}
}
