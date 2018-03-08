using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class UserLobbyController : Controller
    {

        public UserLobbyView userLobbyView;

        public int user;


        void Update()
        {
            userLobbyView.rankImage.sprite = IconManager.instance.rankSprites[PNetworkManager.instance.networkUsers[user].userData.rank];

            userLobbyView.rankText.text = PNetworkManager.instance.networkUsers[user].userData.rank.ToString();

            userLobbyView.readyImage.gameObject.SetActive(PNetworkManager.instance.networkUsers[user].ready);

            userLobbyView.usernameText.text = PNetworkManager.instance.networkUsers[user].userData.username;
        }
        
    }
}