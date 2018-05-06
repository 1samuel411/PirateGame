using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PlayFab;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class MenuOverlayController : Controller
    {

        public PirateGame.UI.Views.MenuOverlayView overlayView;
        public string optionsMenuController;
        public string characterMenuController;
        public string mainMenuController;
        public string friendsMenuController = "Friends";

        void Update()
        {
            //overlayView.rankImage.sprite = IconManager.instance.rankSprites[PlayerManager.instance.user.rank];

            overlayView.rankText.text = PlayerManager.instance.user.rank.ToString();
            overlayView.usernameText.text = PlayerManager.instance.user.username;

            overlayView.xpBarImage.fillAmount =
                (float) (PlayerManager.instance.user.xp - PlayerManager.instance.user.xpToLevel) /
                (float) (PlayerManager.instance.user.xpToRank - PlayerManager.instance.user.xpToLevel);

            overlayView.xpText.text = PlayerManager.instance.user.xp + " / " + PlayerManager.instance.user.xpToRank;

            if (MasterClientManager.instance.isConnected() && PlayerManager.instance.loggedIn)
            {
                if (MasterClientManager.instance.GetServerData("UserCount") != null)
                    overlayView.connectedUsersText.text = "Connected Users: " +
                                                          MasterClientManager.instance.GetServerData("UserCount");
            }

            // Check friend requests, requestee
            int requests = 0;
            for (int i = 0; i < PlayerManager.instance.friends.Count; i++)
            {
                if(PlayerManager.instance.friends[i].Tags != null)
                    for (int x = 0; x < PlayerManager.instance.friends[i].Tags.Count; x++)
                        if (PlayerManager.instance.friends[i].Tags[x].Contains("Requestee"))
                            requests++;
            }

            overlayView.notificationsGameObject.SetActive(requests != 0);
            overlayView.notificationsText.text = requests.ToString();
        }


        public void Friends()
        {
            UIManager.instance.AddScreenAdditive(friendsMenuController);
        }

        public void Options()
        {
            UIManager.instance.ScreenSwitch(optionsMenuController);
        }

        public void MainMenu()
        {
            UIManager.instance.ScreenSwitch(mainMenuController);
        }

        public void Character()
        {
            UIManager.instance.ScreenSwitch(characterMenuController);
        }

        public void Stats()
        {
            
        }

        public void Shop()
        {
            
        }

        public void Logout()
        {
            UIManager.instance.ScreenSwitch("Account");
            MasterClientManager.instance.Disconnect();
        }

        public void Quit()
        {
            UIManager.instance.ScreenSwitch("Account");
            MasterClientManager.instance.Disconnect();
        }

    }
}