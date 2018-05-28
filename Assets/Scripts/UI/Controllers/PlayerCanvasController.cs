using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.Networking;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class PlayerCanvasController : Controller
    {

        public PlayerCanvasView canvasView;

        private NetworkedPlayer networkedPlayer;

        void Awake()
        {
            networkedPlayer = GetComponentInParent<NetworkedPlayer>();
        }

        void Update()
        {
            if (!networkedPlayer)
                return;

            if (networkedPlayer.isLocalPlayer)
                return;

            if (CameraManager.instance == null)
                return;

            if (ServerManager.instance.networkUsers.ContainsKey(networkedPlayer.networkId) == false)
                return;

            canvasView.nameText.text =
                ServerManager.instance.networkUsers[networkedPlayer.networkId].userData.rank +
                " | " + ServerManager.instance.networkUsers[networkedPlayer.networkId]
                    .userData.username;
            canvasView.canvasTransform.LookAt(CameraManager.instance.cameraObject.transform);
        }

    }
}