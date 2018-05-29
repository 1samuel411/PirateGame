using PirateGame.Managers;
using PirateGame.UI.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class InGameOverlayController : Controller
    {

        public InGameOverlayView overlayView;

        public Sprite speakingIcon;
        public Sprite muteIcon;
        public Sprite notSpeakingIcon;


        private void Awake()
        {
            UpdateUI();
        }

        private void Update()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            UpdateMinimapUI();

            UpdateCompassUI();

            UpdateResourcesUI();
        }

        void UpdateMinimapUI()
        {
            overlayView.minimapMarker.localEulerAngles = new Vector3(0, 0, PlayerManager.instance.playablePlayer.transform.eulerAngles.y);
        }

        void UpdateCompassUI()
        {
            overlayView.compassRawImage.uvRect = new Rect(PlayerManager.instance.playablePlayer.transform.eulerAngles.y / 360f, 0, 1, 1);
        }

        void UpdateResourcesUI()
        {

        }

    }
}