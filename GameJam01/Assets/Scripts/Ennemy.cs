﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ennemy : NetworkBehaviour
{

    //Synchronized variables
    [SyncVar] Vector2 synchronizedPosition;
    [SyncVar] Quaternion synchronizedRotation;

    private PlayerControl currentTarget;
    private GameManager gameManager;
    private float contactDist = 0F;

    [Header("Enemy characteristic")]
    public float movementSpeed = 1F;

    // Use this for initialization
    void Start() {
        //movementSpeed = 0.05F;
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (!isServer)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
            transform.position = Vector3.Lerp(transform.position, synchronizedPosition, Time.deltaTime * 10);
            transform.Find("enemy_sprite").rotation = synchronizedRotation;
        }
        else
        {
            if (currentTarget != null)
            {
                if (Vector2.Distance(transform.position, currentTarget.transform.position) >= contactDist)
                {
                    // Init mouvement guide line
                    Vector2 axe = currentTarget.transform.position - gameObject.transform.position;
                    axe.Normalize();
                    gameObject.transform.Translate(axe * movementSpeed * Time.deltaTime);

                    Vector3 difference = currentTarget.transform.position - transform.position;
                    difference.Normalize();
                    float rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    gameObject.transform.Find("enemy_sprite").transform.rotation = Quaternion.Euler(0f, 0f, rotation);
                }
            }
            else
            {
                PlayerControl[] potentialTarget = GameObject.FindObjectsOfType<PlayerControl>();
                if (potentialTarget != null && potentialTarget.Length > 0)
                {
                    int luckyBastard = Random.Range(1, potentialTarget.Length + 1);
                    currentTarget = potentialTarget[luckyBastard - 1];
                }
            }
            //Network synchronisation
            SendPosition();
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Projectiles>()){
            if (collision.gameObject.GetComponent<Projectiles>().isFromMyPlayer) {
                GameManager.nbEnnemiesKilled++;
            }
            Destroy(gameObject);
        }
        if (collision.gameObject.GetComponent<PlayerControl>()) {
            Destroy(gameObject);

        }
    }

        //Network function

    //Client
    [Client]
    void SendPosition()
    {
        CmdSendMyPositionToServer(transform.position);
        CmdSendMyRotationToServer(transform.Find("enemy_sprite").rotation);
    }


    //Command
    [Command]
    void CmdSendMyPositionToServer(Vector3 position)
    {
        synchronizedPosition = position;
    }

    [Command]
    void CmdSendMyRotationToServer(Quaternion rotation)
    {
        synchronizedRotation = rotation;
    }
}
