using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CT.Net;

public class ServerConnector : MonoBehaviour {
    public static ServerConnector instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Invoke("Connect", 0.5f);
    }

    void Connect()
    {
        Client.instance.ConnectClientToServer();
    }
}
