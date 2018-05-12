using UnityEngine;
using PirateGame.Managers;
using PirateGame.UI.Views;

namespace PirateGame.UI.Controllers
{
    public class MainMenuController : Controller
    {

        public MainMenuView mainMenuView;

        public string playMenuController;

        public void Play()
        {
            UIManager.instance.ScreenSwitch(playMenuController);
        }

        public void Leave()
        {
            MasterClientManager.instance.SendLeave();
        }

        void Update()
        {
            mainMenuView.leaveButton.gameObject.SetActive(false);
            if (PlayerManager.instance.roomInfo != null)
            {
                mainMenuView.roomInfoText.text = "Room: " + PlayerManager.instance.roomInfo.roomId;
                for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
                {
                    mainMenuView.roomInfoText.text += "\nUser " + (i + 1) + ": " + PlayerManager.instance.roomInfo.usersInRoom[i].username;
                }

                if(PlayerManager.instance.roomInfo.usersInRoom.Count > 1)
                {
                    mainMenuView.leaveButton.gameObject.SetActive(true);
                }

                mainMenuView.character1.SetActive(false);
                mainMenuView.character2.SetActive(false);
                mainMenuView.character3.SetActive(false);
                mainMenuView.character4.SetActive(false);
                if (PlayerManager.instance.roomInfo.usersInRoom.Count == 1)
                    mainMenuView.character1.SetActive(true);
                if (PlayerManager.instance.roomInfo.usersInRoom.Count == 2)
                {
                    mainMenuView.character1.SetActive(true);
                    mainMenuView.character2.SetActive(true);
                }
                if (PlayerManager.instance.roomInfo.usersInRoom.Count == 3)
                {
                    mainMenuView.character1.SetActive(true);
                    mainMenuView.character2.SetActive(true);
                    mainMenuView.character3.SetActive(true);
                }
                if (PlayerManager.instance.roomInfo.usersInRoom.Count == 4)
                {
                    mainMenuView.character1.SetActive(true);
                    mainMenuView.character2.SetActive(true);
                    mainMenuView.character3.SetActive(true);
                    mainMenuView.character4.SetActive(true);
                }
            }
            else
            {
                mainMenuView.roomInfoText.text = "Room not found";
            }
        }

    }
}