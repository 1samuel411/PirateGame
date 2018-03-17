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

        private PlayablePlayer playablePlayer;

        void Awake()
        {
            playablePlayer = GetComponent<PlayablePlayer>();
        }

        void Update()
        {
            if (!playablePlayer)
                return;

            canvasView.nameText.color = CrewManager.GetCrewColor(ServerManager.instance.networkUsers[playablePlayer.playerId].crew);

            canvasView.nameText.text =
                ServerManager.instance.networkUsers[playablePlayer.playerId].userData.rank +
                " | " + ServerManager.instance.networkUsers[playablePlayer.playerId]
                    .userData.username;
            canvasView.canvasTransform.LookAt(CameraManager.instance.cameraObject.transform);
        }

    }
}