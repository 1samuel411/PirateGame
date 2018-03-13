using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.Networking;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class LobbyController : Controller
    {

        public LobbyView lobbyView;

        public List<UserLobbyController> users = new List<UserLobbyController>();
        public List<UserCrewController> usersCrew = new List<UserCrewController>();

        public override void Enabled()
        {
            PNetworkManager.instance.networkUserChange += RefreshUsers;
            PNetworkManager.instance.crewsChange += RefreshCrews;

            RefreshUsers();
            RefreshCrews();

            UpdateReady();
        }

        void RefreshUsers()
        {
            for (int i = 0; i < users.Count; i++)
            {
                GameObject.Destroy(users[i].gameObject);
            }
            users.Clear();

            foreach (KeyValuePair<int, NetworkUser> player in ServerManager.instance.networkUsers)
            {
                AddUser(player.Key);
            }
        }

        void RefreshCrews()
        {
            for (int i = 0; i < usersCrew.Count; i++)
            {
                GameObject.Destroy(usersCrew[i].gameObject);
            }
            usersCrew.Clear();

            if (ServerManager.instance.myNetworkPlayer == null)
                return;

            Crew myCrew = CrewManager.GetCrew(ServerManager.instance.networkUsers[ServerManager.instance.myNetworkPlayer.networkId].crew);
            if (myCrew == null)
                return;

            for(int i = 0; i < myCrew.members.Count; i++)
            {
                AddCrew(myCrew.members[i]);
            }
        }

        void AddUser(int networkUser)
        {
            GameObject newUserGameObject = Instantiate(lobbyView.userLobbyPrefab);
            newUserGameObject.transform.SetParent(lobbyView.playerHolder);
            newUserGameObject.transform.localScale = Vector3.one;
            newUserGameObject.transform.localPosition = Vector3.zero;

            UserLobbyController controller = newUserGameObject.GetComponent<UserLobbyController>();
            controller.user = networkUser;

            users.Add(controller);
        }

        void AddCrew(int networkUser)
        {
            GameObject newUserGameObject = Instantiate(lobbyView.crewUserLobbyPrefab);
            newUserGameObject.transform.SetParent(lobbyView.crewPlayerHolder);
            newUserGameObject.transform.localScale = Vector3.one;
            newUserGameObject.transform.localPosition = Vector3.zero;

            UserCrewController controller = newUserGameObject.GetComponent<UserCrewController>();
            controller.user = networkUser;
            controller.lobbyController = this;

            usersCrew.Add(controller);
        }

        public void KickUser(int userConnectionId)
        {
            Debug.Log("Kicking: " + userConnectionId);   
        }

        public void ReadyToggle()
        {
            ServerManager.instance.ReadyToggle();
            UpdateReady();
        }

        public void Quit()
        {
            PNetworkManager.instance.Disconnect();
        }

        public void ChangeCrew()
        {
            ServerManager.instance.ChangeCrew();
        }

        void UpdateReady()
        {
            lobbyView.readyButton.color = !(ServerManager.instance.networkUsers[ServerManager.instance.myNetworkPlayer.networkId].ready)
                ? lobbyView.readyColor
                : lobbyView.unReadyColor;

            lobbyView.readyText.text = !(ServerManager.instance.networkUsers[ServerManager.instance.myNetworkPlayer.networkId].ready)
                ? "Ready"
                : "Not Ready";
        }

        void Update()
        {
            if (ServerManager.instance.readyToPlay)
            {
                lobbyView.infoText.text = "Game is starting in " + ServerManager.instance.lobbyTimer + " seconds...";
            }
            else
            {
                lobbyView.infoText.text = "";
            }

            lobbyView.crewNameText.text = "Join a crew...";
;
            Crew myCrew = CrewManager.GetCrew(ServerManager.instance.networkUsers[ServerManager.instance.myNetworkPlayer.networkId].crew);
            if (myCrew == null)
                return;

            lobbyView.crewNameText.text = myCrew.crewName;
        }
    }
}
