using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNetwork.Client;
using System;
using SNetwork;
using System.Net.Sockets;

public class MatchClientManager : MonoBehaviour
{
    private MatchClient _client;
    private MatchClientResponseHandler _clientResponseHandler;

    private static MatchClientManager _instance;
    public static MatchClientManager instance
    {
        get
        {
            if (_instance == null)
            {
                // create an instance
                GameObject newObject = new GameObject();
                _instance = newObject.AddComponent<MatchClientManager>();
                newObject.name = "Client Instance";
            }

            return _instance;
        }
        set { _instance = value; }
    }

    void Awake()
    {
        _client = gameObject.AddComponent<MatchClient>();

        _clientResponseHandler = new MatchClientResponseHandler(_client);

        _clientResponseHandler.Initialize();

        DontDestroyOnLoad(this.gameObject);
    }

    public Socket getSocket()
    {
        return _client.clientSocket;
    }

    public bool isConnected()
    {
        return _client.IsConnectedClient();
    }

    public bool isConnecting()
    {
        return _client.connecting;
    }

    public bool isDisconnecting()
    {
        return _client.disconnecting;
    }

    public int getId()
    {
        return _client.ourId;
    }

    public delegate void OnConnectedDelegate(ResponseMessage message);
    public OnConnectedDelegate onConnectDelegate;
    public delegate void OnCloseDelegate();
    public OnCloseDelegate onCloseDelegate;
    public delegate void OnDisconnectDelege();
    public OnDisconnectDelege onDisconnectDelegate;

    public void Connect(string ip = "", int port = 0)
    {
        _client.Connect(ip, port, OnConnected, OnClose);
    }

    public void OnConnected(ResponseMessage response)
    {
        if (onConnectDelegate != null)
            onConnectDelegate(response);
        if (response.type == ResponseMessage.ResponseType.Success)
        {
            Debug.Log("Success!");
            // Do something
        }
        else if (response.type == ResponseMessage.ResponseType.Failure)
        {
            Debug.Log("Failed!");
            // Do something
        }
        else if (response.type == ResponseMessage.ResponseType.Full)
        {
            Debug.Log("Server full!");
            // Do something
        }
    }

    public void SendPort(int port)
    {
        MatchMessaging.instance.SendPort(port, 2, getId(), 0, _client.clientSocket);
    }

    public void OnClose()
    {
        if (onCloseDelegate != null)
            onCloseDelegate();

        Debug.Log("Connection Closed!");
        // Do something

        Destroy(this.gameObject);
    }

    public void Disconnect()
    {
        if (onDisconnectDelegate != null)
            onDisconnectDelegate();

        _client.Disconnect(OnDisconnected);
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected!");

        // Destroy client
        Destroy(this.gameObject);

        Application.Quit();
    }

    public void SendString(string message)
    {
        _client.SendString(message);
    }
}