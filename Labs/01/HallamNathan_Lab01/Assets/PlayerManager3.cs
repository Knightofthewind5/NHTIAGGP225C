using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager3 : MonoBehaviour
{
    public float health = 100f;
    GameObject Player;
    CharacterController controller;
    public GameObject projectileSpawn;
    public GameObject projectile;
    public float playerSpeed = 2;
    public float jumpHeight = 2;
    public Vector3 playerVelocity;
    public float gravityValue = -9.81f;
    bool groundedPlayer = true;

    void Awake()
    {
        Player = gameObject;
        controller = gameObject.GetComponent<CharacterController>();
    }

    //https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
    void Update()
    {
        if (gameObject.GetPhotonView().IsMine)
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * playerSpeed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            // Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (Input.GetButtonDown("Fire1"))
            {
                PhotonNetwork.Instantiate(projectile.name, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}