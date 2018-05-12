using System.Collections;
using System.Collections.Generic;
using PirateGame.Managers;
using PirateGame.UI.Views;
using SNetwork.Client;
using UnityEngine;

namespace PirateGame.UI.Controllers
{
    public class FriendsController : Controller
    {

        public FriendsView friendsView;

        public List<FriendUserController> friendUserController = new List<FriendUserController>();

        void Start()
        {
            PlayerManager.instance.refreshFriendsDelegate += RefreshFriends;
        }

        void OnEnable()
        {
            RespawnFriends();
        }

        void RespawnFriends()
        {
            for (int i = 0; i < friendUserController.Count; i++)
            {
                if (friendUserController[i] != null)
                    GameObject.Destroy(friendUserController[i].gameObject);
            }

            friendUserController.Clear();

            if (PlayerManager.instance.loggedIn == false)
                return;

            for (int i = 0; i < PlayerManager.instance.friends.Count; i++)
            {
                friendUserController.Add(CreateFriendUserController(PlayerManager.instance.friends[i].FriendPlayFabId));
            }

            RefreshFriends();
        }

        void RefreshFriends()
        {
            for(int i = 0; i < friendUserController.Count; i++)
            {
                friendUserController[i].Refresh();
            }
        }

        FriendUserController CreateFriendUserController(string playfabId)
        {
            GameObject friendPrefab = Instantiate(friendsView.friendUserPrefab);
            friendPrefab.transform.SetParent(friendsView.friendsListHolder);
            friendPrefab.transform.localPosition = Vector3.zero;
            friendPrefab.transform.localScale = Vector3.one;
            friendPrefab.transform.rotation = Quaternion.identity;

            FriendUserController friendUserController = friendPrefab.GetComponent<FriendUserController>();
            friendUserController.playfabId = playfabId;
            return friendUserController;
        }

        public void AddFriends()
        {
            UIManager.instance.AddScreenAdditive("AddFriend");
        }

        public void Back()
        {
            UIManager.instance.UnloadScreenAdditive("Friends");
        }

    }
}