using DG.Tweening;
using PirateGame.Managers;
using PirateGame.UI;
using PirateGame.UI.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace PirateGame.UI.Controllers
{
    public class PauseController : Controller
    {

        public PauseView pauseView;

        void Start()
        {

        }

        void Update()
        {

        }

        public void Resume()
        {
            PauseManager.instance.TogglePause(false);
        }

        public void Quit()
        {
            PNetworkManager.instance.Disconnect();
        }

        private void OnEnable()
        {
            if(CameraManager.instance)
                CameraManager.instance.ChangeDepthOfField(false);
        }

        private void OnDisable()
        {
            if(CameraManager.instance)
                CameraManager.instance.ChangeDepthOfField(true);
        }
    }
}