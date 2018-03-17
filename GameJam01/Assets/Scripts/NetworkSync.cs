﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSync : NetworkBehaviour
{

    //Synchronized variables
    [SyncVar] Vector2 synchronizedPosition;
    [SyncVar] Quaternion synchronizedRotation;


    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(0F, 0F, -5F);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            transform.position = Vector2.Lerp(transform.position, synchronizedPosition, Time.deltaTime * 10);
            transform.Find("Body").rotation = synchronizedRotation;
        }
        else
        {
            //Network synchronisation
            SendPosition();
        }
    }

    //Network function

    //Client
    [Client]
    void SendPosition()
    {
        CmdSendMyPositionToServer(transform.position);
        CmdSendMyRotationToServer(transform.Find("Body").rotation);
    }


    //Command
    [Command]
    void CmdSendMyPositionToServer(Vector2 position)
    {
        synchronizedPosition = position;
    }

    [Command]
    void CmdSendMyRotationToServer(Quaternion rotation)
    {
        synchronizedRotation = rotation;
    }
}
