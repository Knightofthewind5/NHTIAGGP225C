using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    public Color color;

    private void Start()
    {
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void Update()
    {
        //Changes color over the network
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.GetPhotonView().RPC("ChangeColor", RpcTarget.AllBuffered, color.r, color.g, color.b);
        }

        //Changes color locally for user to change
        if (Input.GetKeyDown(KeyCode.C))
        {
            color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }

    [PunRPC]
    void ChangeColor(float R, float G, float B)
    {
        Color c = new Color(R, G, B);
        Camera.main.backgroundColor = c;
    }
}
