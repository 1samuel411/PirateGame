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
            RefreshFriends();
        }

        void RefreshFriends()
        {
            for (int i = 0; i < friendUserController.Count; i++)
            {
                if(friendUserController[i] != null)
                    GameObject.Destroy(friendUserController[i].gameObject);
            }

            friendUserController.Clear();

            if (PlayerManager.instance.loggedIn == false)
                return;

            for (int i = 0; i < PlayerManager.instance.friends.Count; i++)
            {
                FriendUserController controller = CreateFriendUserController();

                controller.tags = PlayerManager.instance.friends[i].Tags;
                controller.online = "false";
                for (int x = 0; x < PlayerManager.instance.friendsData.Count; x++)
                {
                    if (PlayerManager.instance.friendsData[x].playerId ==
                        PlayerManager.instance.friends[i].FriendPlayFabId)
                    {
                        controller.online = PlayerManager.instance.friendsData[x].loggedIn;
                        break;
                    }
                }

                List<Invite> invites = new List<Invite>();
                if (PlayerManager.instance.invites != null)
                {
                    for (int x = 0; x < PlayerManager.instance.invites.Length; x++)
                    {
                        // found a relevant invite
                        if (PlayerManager.instance.invites[x].userFrom == MasterClientManager.instance.getId() ||
                            PlayerManager.instance.invites[x].userTo == MasterClientManager.instance.getId())
                        {
                            // add it
                            invites.Add(PlayerManager.instance.invites[x]);
                        }
                    }
                }
                controller.inviteData = invites.ToArray();

                controller.playfabId = PlayerManager.instance.friends[i].FriendPlayFabId;
                controller.username = PlayerManager.instance.friends[i].TitleDisplayName;

                friendUserController.Add(controller);
            }
        }

        FriendUserController CreateFriendUserController()
        {
            GameObject friendPrefab = Instantiate(friendsView.friendUserPrefab);
            friendPrefab.transform.SetParent(friendsView.friendsListHolder);
            friendPrefab.transform.localPosition = Vector3.zero;
            friendPrefab.transform.localScale = Vector3.one;
            friendPrefab.transform.rotation = Quaternion.identity;

            FriendUserController friendUserController = friendPrefab.GetComponent<FriendUserController>();
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