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


        void Awake()
        {
            SetData();
        }

        void Update()
        {
            SetData();
        }

        void SetData()
        {
            userLobbyView.rankImage.sprite = IconManager.instance.rankSprites[ServerManager.instance.networkUsers[user].userData.rank];

            userLobbyView.rankText.text = ServerManager.instance.networkUsers[user].userData.rank.ToString();

            userLobbyView.readyImage.gameObject.SetActive(ServerManager.instance.networkUsers[user].ready);

            userLobbyView.usernameText.text = ServerManager.instance.networkUsers[user].userData.username;

            Crew crew = CrewManager.GetCrew(ServerManager.instance.networkUsers[user].crew);

            userLobbyView.leaderImage.gameObject.SetActive(false);
            userLobbyView.backgroundImage.color = Color.white;

            if (CrewManager.HasCrew(ServerManager.instance.networkUsers[user].crew))
            {
                if (crew.leader == user)
                {
                    userLobbyView.leaderImage.gameObject.SetActive(true);
                }

                userLobbyView.backgroundImage.color = CrewManager.GetCrewColor(crew.crewColor);
            }

            Color backgroundColor = userLobbyView.backgroundImage.color;
            backgroundColor.a = 0.4f;
            userLobbyView.backgroundImage.color = backgroundColor;
        }

        
    }
}