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

        public override void Enabled()
        {
            PNetworkManager.instance.networkUserChange += RefreshUsers;
            networkedPlayer = PNetworkManager.instance.client.connection.playerControllers[0].gameObject.GetComponent<NetworkedPlayer>();

            RefreshUsers();

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

        private NetworkedPlayer networkedPlayer = null;
        void UpdateReady()
        {
            lobbyView.readyButton.color = !(ServerManager.instance.networkUsers[networkedPlayer.networkId].ready)
                ? lobbyView.readyColor
                : lobbyView.unReadyColor;

            lobbyView.readyText.text = !(ServerManager.instance.networkUsers[networkedPlayer.networkId].ready)
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
        }
    }
}
