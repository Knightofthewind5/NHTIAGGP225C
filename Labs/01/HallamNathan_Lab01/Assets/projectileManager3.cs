using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class projectileManager3 : MonoBehaviour
{
    public float damage = 2f;
    public float speed = 10f;
    public float lifetime = 5f;
    Rigidbody rb;

    public void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(rb.position + transform.forward * Time.deltaTime * speed);

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(this.gameObject);
        }    
    }
}
