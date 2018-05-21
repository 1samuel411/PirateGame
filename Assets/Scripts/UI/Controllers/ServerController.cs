using PirateGame.Managers;
using SNetwork;
using SNetwork.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.UI.Controllers
{
    public class ServerController : Controller
    {

        public string ip;
        public int port;

        public string serverIp;
        public int serverPort;

        public GameObject connectedHolder;
        public GameObject disconnectedHolder;

        private void Start()
        {
            String[] Data = Environment.GetCommandLineArgs();
            bool containsData = false;
            for(int i =0; i < Data.Length; i++)
            {
                if(Data[i] == "-ip")
                {
                    ip = Data[i + 1];
                }
                if(Data[i] == "-port")
                {
                    port = int.Parse(Data[i + 1]);
                }
                if(Data[i] == "-serverIp")
                {
                    serverIp = Data[i + 1];
                }
                if(Data[i] == "-serverPort")
                {
                    serverPort = int.Parse(Data[i + 1]);
                    containsData = true;
                }
            }
            if (containsData)
            {
                Connect();
            }
            else
            {
                //Application.Quit();
            }
        }

        public void SetIP(string ip)
        {
            this.ip = ip;
        }

        public void SetPort(string port)
        {
            this.port = int.Parse(port);
        }

        public void SetServerPort(string port)
        {
            this.serverPort = int.Parse(port);
        }

        public void SetServerIP(string ip)
        {
            this.serverIp = ip;
        }

        public void Connect()
        {
            MatchClientManager.instance.onConnectDelegate += OnConnected;
            MatchClientManager.instance.Connect(ip, port);
        }

        public bool connected = false;
        void OnConnected(ResponseMessage response)
        {
            if(response.type == ResponseMessage.ResponseType.Success)
            {
                connected = true;
            }
        }

        public void StartServer()
        {
            if(!connected)
            {
                return;
            }

            if(MatchClientManager.instance.isConnected())
            {
                MatchClientManager.instance.SendIp(serverIp);
                MatchClientManager.instance.SendPort(serverPort);
                PNetworkManager.instance.networkAddress = serverIp;
                PNetworkManager.instance.networkPort = serverPort;
                PNetworkManager.instance.serverStartAction += Connected;
                PNetworkManager.instance.disconnectAction += Disconnected;
                PNetworkManager.instance.PStartServer();
            }
        }

        void Connected(NetworkConnection con)
        {
            Debug.Log("Connected");
            MatchClientManager.instance.SendServerOpen();
            UIManager.instance.loading = false;

            connectedHolder.gameObject.SetActive(true);
            disconnectedHolder.gameObject.SetActive(false);
        }

        void Disconnected(NetworkConnection con)
        {
            connectedHolder.gameObject.SetActive(false);
            disconnectedHolder.gameObject.SetActive(true);
        }

        public void Disconnect()
        {
            PNetworkManager.instance.Disconnect();
            MatchClientManager.instance.Disconnect();
        }
    }
}