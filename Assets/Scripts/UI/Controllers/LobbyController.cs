using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class LobbyController : Controller
    {

        public LobbyView lobbyView;

        public List<UserLobbyController> users = new List<UserLobbyController>();

        void OnEnable()
        {
            PNetworkManager.instance.networkUserChange += RefreshUsers;

            RefreshUsers();
        }

        void RefreshUsers()
        {
            for (int i = 0; i < users.Count; i++)
            {
                GameObject.Destroy(users[i].gameObject);
            }
            users.Clear();

            foreach (KeyValuePair<int, NetworkUser> player in PNetworkManager.instance.networkUsers)
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
            
        }

        public void Quit()
        {
            PNetworkManager.instance.Disconnect();
        }

    }
}
