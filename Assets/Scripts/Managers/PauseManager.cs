using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PirateGame.Managers
{
    public class PauseManager : Base
    {

        public static PauseManager instance;

        private void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if(!UIManager.instance.inGameCanvas.gameObject.activeSelf)
            {
                paused = false;
                return;
            }

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("Pause"))
            {
                TogglePause();
            }

            if(paused)
            {
                InputManager.instance.player = Rewired.ReInput.players.SystemPlayer;
            }
            else
            {
                InputManager.instance.player = Rewired.ReInput.players.GetPlayer(0);
            }
        }

        private bool paused;
        public void TogglePause(bool? newValue = null)
        {
            if (newValue == null)
                paused = !paused;
            else
                paused = newValue.Value;

            if(paused)
            {
                UIManager.instance.ScreenSwitch("InGamePause");
                CameraManager.instance.ChangeDepthOfField(false);
            }
            else
            {
                UIManager.instance.ScreenSwitch("InGameOverlay");
                CameraManager.instance.ChangeDepthOfField(true);
            }
        }

    }
}