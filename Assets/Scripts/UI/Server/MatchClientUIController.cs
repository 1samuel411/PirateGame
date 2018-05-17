using SNetwork.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchClientUIController : MonoBehaviour
{

    public string ip;
    public int port;
    public int newPort;

    public void SetIP(string ip)
    {
        this.ip = ip;
    }

    public void SetPort(string port)
    {
        this.port = int.Parse(port);
    }

    public void SetNewPort(string port)
    {
        this.newPort = int.Parse(port);
    }

    public void Connect()
    {
        MatchClientManager.instance.Connect(ip, port);
    }

    public void SendNewPort()
    {
        MatchClientManager.instance.SendPort(newPort);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
