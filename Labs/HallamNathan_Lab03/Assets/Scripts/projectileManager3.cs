using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class projectileManager3 : MonoBehaviour
{
	public float damage = 2f;
	Rigidbody rb;

	public void Awake()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		rb.useGravity = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<PlayerManager3>())
		{
			DataSync ds = collision.gameObject.GetComponent<DataSync>();

			if (ds)
			{
				ds.TakeDamage(damage);

				PhotonManager.Instance.gameObject.GetPhotonView().RPC("DestroyObject", RpcTarget.All, gameObject.GetInstanceID());
			}
		}
	}
}
