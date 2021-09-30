using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager3 : MonoBehaviour
{
    public float health;
    public float maxHealth = 100f;
    GameObject Player;
    CharacterController controller;
    public GameObject projectileSpawn;
    public GameObject projectile;
    public float playerSpeed = 2;
    public float jumpHeight = 2;
    public Vector3 playerVelocity;
    public float gravityValue = -9.81f;
    bool groundedPlayer = true;
    public Image healthbar;
    public TMP_Text user;

    float fillAmount;

    void Awake()
    {
        Player = gameObject;
        controller = gameObject.GetComponent<CharacterController>();
        health = maxHealth;
    }

    //https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
    void Update()
    {
        UpdateHealth();

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
        //gameObject.GetPhotonView().RPC("TakeDamageRPC", RpcTarget.Others, damage);

        Debug.Log("[PM3][TakeDamage]");

        health -= damage;

        if (health <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void TakeDamageRPC(float damage)
    {
        
    }

    public void UpdateHealth()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        fillAmount = (health / maxHealth);

        healthbar.fillAmount = fillAmount;
    }
}