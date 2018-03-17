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

            canvasView.nameText.color = CrewManager.GetCrewColor(ServerManager.instance.networkUsers[networkedPlayer.networkId].crew);

            canvasView.nameText.text =
                ServerManager.instance.networkUsers[networkedPlayer.networkId].userData.rank +
                " | " + ServerManager.instance.networkUsers[networkedPlayer.networkId]
                    .userData.username;
            canvasView.canvasTransform.LookAt(CameraManager.instance.cameraObject.transform);
        }

    }
}