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

        void Update()
        {
            if (PlayerManager.instance.roomInfo != null)
            {
                mainMenuView.roomInfoText.text = "Room: " + PlayerManager.instance.roomInfo.roomId;
                for (int i = 0; i < PlayerManager.instance.roomInfo.usersInRoom.Count; i++)
                {
                    mainMenuView.roomInfoText.text += "\nUser " + (i + 1) + ": " + PlayerManager.instance.roomInfo.usersInRoom[i].username;
                }
            }
            else
            {
                mainMenuView.roomInfoText.text = "Room not found";
            }
        }

    }
}