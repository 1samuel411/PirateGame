using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class PlayerCanvasController : Controller
    {

        public PlayerCanvasView canvasView;

        void Start()
        {

        }

        void Update()
        {
            if (!PlayerManager.instance || !PlayerManager.instance.playablePlayer)
                return;

            canvasView.nameText.text =
                ServerManager.instance.networkUsers[PlayerManager.instance.playablePlayer.playerId].userData.rank +
                " | " + ServerManager.instance.networkUsers[PlayerManager.instance.playablePlayer.playerId]
                    .userData.username;
            canvasView.canvasTransform.LookAt(CameraManager.instance.cameraObject.transform);
        }

    }
}