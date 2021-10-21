using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Pickup : MonoBehaviour
{
    Collider collider;

    public void Start()
    {
        collider = gameObject.GetComponent<Collider>();
    }
    
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player " + other.name + " Entered PILL");


            PhotonManager.Instance.gameObject.GetPhotonView().RPC("DestroyObject", RpcTarget.All, gameObject.GetInstanceID());
        }
    }
}
