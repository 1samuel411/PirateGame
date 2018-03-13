using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class UserCrewController : Controller
    {

        public UserCrewView crewView;

        public int user;
        public LobbyController lobbyController;

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
            crewView.rankImage.sprite = IconManager.instance.rankSprites[ServerManager.instance.networkUsers[user].userData.rank];

            crewView.rankText.text = ServerManager.instance.networkUsers[user].userData.rank.ToString();

            crewView.usernameText.text = ServerManager.instance.networkUsers[user].userData.username;

            Crew crew = CrewManager.GetCrew(ServerManager.instance.networkUsers[user].crew);

            crewView.leaderImage.gameObject.SetActive(false);
            crewView.kickButton.SetActive(false);
            crewView.backgroundImage.color = Color.white;

            if (CrewManager.HasCrew(ServerManager.instance.networkUsers[user].crew))
            {
                if (crew.leader == user)
                {
                    crewView.leaderImage.gameObject.SetActive(true);
                }
                else
                {
                    if (ServerManager.instance.myNetworkPlayer.networkId == crew.leader)
                    {
                        crewView.kickButton.SetActive(true);
                    }
                }

                crewView.backgroundImage.color = CrewManager.GetCrewColor(crew.crewColor);
            }

            Color backgroundColor = crewView.backgroundImage.color;
            backgroundColor.a = 0.4f;
            crewView.backgroundImage.color = backgroundColor;
        }

        public void KickMe()
        {
            lobbyController.KickUser(user);
        }
    }
}