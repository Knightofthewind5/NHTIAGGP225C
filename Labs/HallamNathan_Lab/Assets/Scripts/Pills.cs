using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Pills : Pickup
{
    public enum PillType
    {
        Health,
        Speed,
        Cooldown
    }

    public PillType typeOfPill;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (typeOfPill == PillType.Health)
            {
                other.gameObject.GetComponent<DataSync>().TakeDamage(-10, null);
            }
            else if (typeOfPill == PillType.Speed)
            {
                other.gameObject.GetComponent<PlayerManager3>().photonView.RPC("setSpeed", RpcTarget.All, 10f);
            }
            else if (typeOfPill == PillType.Cooldown)
            {
                other.gameObject.GetComponent<PlayerManager3>().photonView.RPC("setFireFoever", RpcTarget.All);
            }
            else
            {
                return;
            }

            base.OnTriggerEnter(other);
        }
    }
}