using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace PirateGame.UI.Controllers
{
    public class MenuController : Controller
    {

        public NetworkingBase networkingBase;

        public string disconnectedController = "Play";
        public string connectedController = "Lobby";

        void Start()
        {
            networkingBase = GetComponent<NetworkingBase>();

            PNetworkManager.instance.disconnectAction += OnDisconnect;
            PNetworkManager.instance.connectAction += OnConnect;
        }

        void Update()
        {

        }

        void OnDisconnect(NetworkConnection conn)
        {
            Debug.Log("Disconnected Menu displaying");
            UIManager.instance.ScreenSwitch(disconnectedController);
        }

        void OnConnect(NetworkConnection conn)
        {
            Debug.Log("Connected Menu displaying");
            UIManager.instance.ScreenSwitch(connectedController);
        }

    }
}